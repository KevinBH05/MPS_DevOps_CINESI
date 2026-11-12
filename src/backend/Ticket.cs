namespace CineSi;
 
/// <summary>Justificante de una entrada vendida. Guarda el precio pagado como copia.</summary>
public class Ticket
{
    private readonly Sesion _sesion;
    private readonly Asiento _asiento;
 
    public string Codigo { get; }
    public decimal PrecioPagado { get; }
 
    public Ticket(Sesion sesion, Asiento asiento, decimal precioPagado)
    {
        _sesion = sesion;
        _asiento = asiento;
        PrecioPagado = precioPagado;
        Codigo = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
    }
 
    public string Pelicula() => _sesion.Pelicula.Titulo;
 
    public DateTime Hora() => _sesion.Inicio;
 
    public int Sala() => _sesion.Sala.Id;
 
    public string Asiento() => _asiento.Etiqueta();
}