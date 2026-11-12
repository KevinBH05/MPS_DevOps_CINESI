namespace CineSi;
 
public class Compra
{
    private readonly List<Entrada> _entradas = new();
 
    public int ClienteId { get; }
    public EstadoCompra Estado { get; private set; } = EstadoCompra.Pendiente;
    public DateTime Fecha { get; }
    public Sesion Sesion { get; }
 
    public Compra(int clienteId, Sesion sesion, DateTime? fecha = null)
    {
        ClienteId = clienteId;
        Sesion = sesion;
        Fecha = fecha ?? DateTime.Now;
    }
 
    /// <summary>El asiento debe estar ya bloqueado por este cliente (Sesion.BloquearAsientos).</summary>
    public void AgregarEntrada(AsientoSesion a, IDescuentoStrategy d)
    {
        ExigirPendiente();
 
        if (!Sesion.ObtenerMapa().Contains(a))
            throw new ArgumentException("El asiento no pertenece a la sesión de esta compra.");
 
        if (a.Estado != EstadoAsiento.Bloqueado || a.BloqueadoPor != ClienteId)
            throw new InvalidOperationException($"El asiento {a.Asiento.Etiqueta()} no está bloqueado por este cliente.");
 
        if (_entradas.Any(e => e.AsientoSesion == a))
            throw new InvalidOperationException($"El asiento {a.Asiento.Etiqueta()} ya está en la compra.");
 
        var entrada = new Entrada(a);
        entrada.ElegirDescuento(d);
        _entradas.Add(entrada);
    }
 
    public decimal Subtotal() => _entradas.Sum(_ => Entrada.PrecioBase);
 
    public decimal Total() => _entradas.Sum(e => e.PrecioFinal());
 
    public decimal TotalDescuentos() => Subtotal() - Total();
 
    public void Confirmar(DateTime? ahora = null)
    {
        ExigirPendiente();
        var t = ahora ?? DateTime.Now;
 
        if (_entradas.Count == 0)
            throw new InvalidOperationException("La compra no tiene entradas.");
 
        foreach (var e in _entradas)
        {
            var a = e.AsientoSesion;
            if (a.Estado != EstadoAsiento.Bloqueado || a.HaExpirado(t))
                throw new InvalidOperationException($"El bloqueo del asiento {a.Asiento.Etiqueta()} ha expirado.");
        }
 
        foreach (var e in _entradas)
            e.AsientoSesion.Vender();
 
        Estado = EstadoCompra.Confirmada;
    }
 
    public void Cancelar()
    {
        if (Estado == EstadoCompra.Confirmada)
            throw new InvalidOperationException("No se puede cancelar una compra confirmada.");
        if (Estado == EstadoCompra.Cancelada)
            return;
 
        foreach (var e in _entradas)
        {
            var a = e.AsientoSesion;
            if (a.Estado == EstadoAsiento.Bloqueado && a.BloqueadoPor == ClienteId)
                a.Liberar();
        }
 
        Estado = EstadoCompra.Cancelada;
    }
 
    public IEnumerable<Ticket> EmitirTickets()
    {
        if (Estado != EstadoCompra.Confirmada)
            throw new InvalidOperationException("La compra no está confirmada.");
 
        return _entradas
            .Select(e => new Ticket(Sesion, e.AsientoSesion.Asiento, e.PrecioFinal()))
            .ToList();
    }
 
    private void ExigirPendiente()
    {
        if (Estado != EstadoCompra.Pendiente)
            throw new InvalidOperationException($"La compra está {Estado}, no se puede modificar.");
    }
}