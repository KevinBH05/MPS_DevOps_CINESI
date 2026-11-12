namespace CineSi;
 
public class Sala
{
    private static int _contador;
    private readonly List<Asiento> _asientos;
 
    public int Id { get; }
    public int Filas { get; }
    public int AsientosPorFila { get; }
    public IReadOnlyList<Asiento> Asientos => _asientos;
 
    public Sala(int filas, int asientosPorFila, IEnumerable<Asiento> asientos)
    {
        Id = ++_contador;
        Filas = filas;
        AsientosPorFila = asientosPorFila;
        _asientos = asientos.ToList();
    }
 
    public int Capacidad() => _asientos.Count;
 
    public int CapacidadMinusvalidos() => _asientos.Count(a => a.EsMinusvalido());
 
    public Asiento ObtenerAsiento(char fila, int numero)
        => _asientos.FirstOrDefault(a => a.Fila == fila && a.Numero == numero)
           ?? throw new ArgumentException($"No existe el asiento {fila}{numero} en la sala {Id}.");
 
    public IEnumerable<Asiento> AsientosPorTipo(TipoAsiento t)
        => _asientos.Where(a => a.Tipo == t);
}