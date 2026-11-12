namespace CineSi;
 
public class Sesion
{
    private readonly List<AsientoSesion> _mapa;
    private readonly Dictionary<int, List<AsientoSesion>> _bloqueos = new();
    private int _siguienteBloqueoId = 1;
 
    public int Id { get; }
    public DateTime Inicio { get; }
    public Sala Sala { get; }
    public Pelicula Pelicula { get; }
 
    public Sesion(int id, DateTime inicio, Sala sala, Pelicula pelicula)
    {
        Id = id;
        Inicio = inicio;
        Sala = sala;
        Pelicula = pelicula;
        _mapa = sala.Asientos.Select(a => new AsientoSesion(a)).ToList();
    }
 
    public List<AsientoSesion> ObtenerMapa() => new(_mapa);
 
    public AsientoSesion ObtenerAsientoSesion(Asiento a)
        => _mapa.FirstOrDefault(x => x.Asiento == a)
           ?? throw new ArgumentException($"El asiento {a.Etiqueta()} no pertenece a la sala de esta sesión.");
 
    public bool EstaDisponible(Asiento a, DateTime? ahora = null)
    {
        var t = ahora ?? DateTime.Now;
        var s = ObtenerAsientoSesion(a);
        return s.Estado == EstadoAsiento.Libre || s.HaExpirado(t);
    }
 
    /// <summary>Bloquea todos los asientos o ninguno. Devuelve el identificador del bloqueo.</summary>
    public int BloquearAsientos(IEnumerable<Asiento> asientos, int clienteId, DateTime? ahora = null)
    {
        var t = ahora ?? DateTime.Now;
        ExpirarBloqueos(t); // limpia bloqueos caducados antes de comprobar
 
        var seleccion = asientos.Distinct().Select(ObtenerAsientoSesion).ToList();
        if (seleccion.Count == 0)
            throw new ArgumentException("Hay que indicar al menos un asiento.");
 
        var noDisponible = seleccion.FirstOrDefault(s => s.Estado != EstadoAsiento.Libre);
        if (noDisponible != null)
            throw new InvalidOperationException($"El asiento {noDisponible.Asiento.Etiqueta()} no está disponible.");
 
        foreach (var s in seleccion)
            s.Bloquear(clienteId, t);
 
        int bloqueoId = _siguienteBloqueoId++;
        _bloqueos[bloqueoId] = seleccion;
        return bloqueoId;
    }
 
    public void Liberar(int bloqueoId)
    {
        if (!_bloqueos.Remove(bloqueoId, out var seleccion))
            throw new InvalidOperationException($"El bloqueo {bloqueoId} no existe o ya expiró.");
 
        foreach (var s in seleccion.Where(s => s.Estado == EstadoAsiento.Bloqueado))
            s.Liberar();
    }
 
    public void Confirmar(int bloqueoId, DateTime? ahora = null)
    {
        var t = ahora ?? DateTime.Now;
 
        if (!_bloqueos.TryGetValue(bloqueoId, out var seleccion))
            throw new InvalidOperationException($"El bloqueo {bloqueoId} no existe o ya expiró.");
 
        if (seleccion.Any(s => s.Estado != EstadoAsiento.Bloqueado || s.HaExpirado(t)))
            throw new InvalidOperationException("Algún asiento del bloqueo ha expirado o ya no está bloqueado.");
 
        foreach (var s in seleccion)
            s.Vender();
 
        _bloqueos.Remove(bloqueoId);
    }
 
    /// <summary>Libera los asientos con bloqueo caducado. Devuelve cuántos liberó.</summary>
    public int ExpirarBloqueos(DateTime ahora)
    {
        int liberados = 0;
 
        foreach (var s in _mapa.Where(s => s.HaExpirado(ahora)).ToList())
        {
            s.Liberar();
            liberados++;
        }
 
        // Elimina registros de bloqueo que ya no tienen ningún asiento bloqueado
        var vacios = _bloqueos
            .Where(b => !b.Value.Any(s => s.Estado == EstadoAsiento.Bloqueado))
            .Select(b => b.Key)
            .ToList();
        foreach (var id in vacios)
            _bloqueos.Remove(id);
 
        return liberados;
    }
 
    public DateTime Fin() => Inicio.AddMinutes(Pelicula.DuracionMin);
}