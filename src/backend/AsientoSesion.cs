namespace CineSi;
 
/// <summary>Estado de un asiento concreto dentro de una sesión.</summary>
public class AsientoSesion
{
    public static readonly TimeSpan TTL = TimeSpan.FromMinutes(5);
 
    public Asiento Asiento { get; }
    public EstadoAsiento Estado { get; private set; } = EstadoAsiento.Libre;
    public int? BloqueadoPor { get; private set; }
    public DateTime? BloqueadoEn { get; private set; }
 
    public AsientoSesion(Asiento asiento)
    {
        Asiento = asiento;
    }
 
    public void Bloquear(int clienteId, DateTime ahora)
    {
        if (Estado == EstadoAsiento.Vendido)
            throw new InvalidOperationException($"El asiento {Asiento.Etiqueta()} ya está vendido.");
 
        if (Estado == EstadoAsiento.Bloqueado && !HaExpirado(ahora))
            throw new InvalidOperationException($"El asiento {Asiento.Etiqueta()} está bloqueado por otro cliente.");
 
        Estado = EstadoAsiento.Bloqueado;
        BloqueadoPor = clienteId;
        BloqueadoEn = ahora;
    }
 
    public void Vender()
    {
        if (Estado != EstadoAsiento.Bloqueado)
            throw new InvalidOperationException($"Solo se puede vender un asiento bloqueado ({Asiento.Etiqueta()}).");
 
        Estado = EstadoAsiento.Vendido;
    }
 
    public void Liberar()
    {
        if (Estado == EstadoAsiento.Vendido)
            throw new InvalidOperationException($"No se puede liberar un asiento vendido ({Asiento.Etiqueta()}).");
 
        Estado = EstadoAsiento.Libre;
        BloqueadoPor = null;
        BloqueadoEn = null;
    }
 
    public bool HaExpirado(DateTime ahora)
        => Estado == EstadoAsiento.Bloqueado
           && BloqueadoEn is DateTime desde
           && ahora - desde > TTL;
}
 