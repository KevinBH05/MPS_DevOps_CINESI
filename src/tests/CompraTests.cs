namespace CineSi.Tests;

public class CompraTests
{
    [Fact]
    public void Constructor_CreaCompraPendienteConLosDatosIndicados()
    {
        var fecha = new DateTime(2026, 1, 1, 18, 0, 0);
        var sesion = CrearSesion();

        var compra = new Compra(25, sesion, fecha);

        Assert.Equal(25, compra.ClienteId);
        Assert.Equal(EstadoCompra.Pendiente, compra.Estado);
        Assert.Equal(fecha, compra.Fecha);
        Assert.Same(sesion, compra.Sesion);
        Assert.Equal(0m, compra.Subtotal());
        Assert.Equal(0m, compra.Total());
    }

    [Fact]
    public void AgregarEntrada_UsaElDescuentoYCalculaLosTotales()
    {
        var ahora = FechaPrueba();
        var (sesion, asientos) = CrearSesionConAsientos();
        sesion.BloquearAsientos(asientos, 25, ahora);
        var compra = new Compra(25, sesion, ahora);

        compra.AgregarEntrada(sesion.ObtenerAsientoSesion(asientos[0]), new DescuentoEstudiante());
        compra.AgregarEntrada(sesion.ObtenerAsientoSesion(asientos[1]), new DescuentoSenior());

        Assert.Equal(20m, compra.Subtotal());
        Assert.Equal(13.5m, compra.Total());
        Assert.Equal(6.5m, compra.TotalDescuentos());
    }

    [Fact]
    public void AgregarEntrada_RechazaAsientoNoBloqueado()
    {
        var sesion = CrearSesion();
        var compra = new Compra(25, sesion);

        Assert.Throws<InvalidOperationException>(
            () => compra.AgregarEntrada(sesion.ObtenerMapa()[0], new SinDescuento()));
    }

    [Fact]
    public void AgregarEntrada_RechazaAsientoBloqueadoPorOtroCliente()
    {
        var ahora = FechaPrueba();
        var (sesion, asientos) = CrearSesionConAsientos();
        sesion.BloquearAsientos(new[] { asientos[0] }, 99, ahora);
        var compra = new Compra(25, sesion, ahora);

        Assert.Throws<InvalidOperationException>(
            () => compra.AgregarEntrada(sesion.ObtenerMapa()[0], new SinDescuento()));
    }

    [Fact]
    public void AgregarEntrada_RechazaAsientoDeOtraSesion()
    {
        var sesion = CrearSesion();
        var otraSesion = CrearSesion();
        var asientoAjeno = otraSesion.ObtenerMapa()[0];
        var compra = new Compra(25, sesion);

        Assert.Throws<ArgumentException>(
            () => compra.AgregarEntrada(asientoAjeno, new SinDescuento()));
    }

    [Fact]
    public void AgregarEntrada_RechazaElMismoAsientoDosVeces()
    {
        var ahora = FechaPrueba();
        var (sesion, asientos) = CrearSesionConAsientos();
        sesion.BloquearAsientos(new[] { asientos[0] }, 25, ahora);
        var compra = new Compra(25, sesion, ahora);
        var asientoSesion = sesion.ObtenerAsientoSesion(asientos[0]);
        compra.AgregarEntrada(asientoSesion, new SinDescuento());

        Assert.Throws<InvalidOperationException>(
            () => compra.AgregarEntrada(asientoSesion, new SinDescuento()));
    }

    [Fact]
    public void Confirmar_SinEntradasLanzaExcepcion()
    {
        var compra = new Compra(25, CrearSesion(), FechaPrueba());

        Assert.Throws<InvalidOperationException>(() => compra.Confirmar(FechaPrueba()));
        Assert.Equal(EstadoCompra.Pendiente, compra.Estado);
    }

    [Fact]
    public void Confirmar_ConvierteLasEntradasEnVendidas()
    {
        var ahora = FechaPrueba();
        var (sesion, asientos) = CrearSesionConAsientos();
        sesion.BloquearAsientos(asientos, 25, ahora);
        var compra = new Compra(25, sesion, ahora);
        foreach (var asiento in sesion.ObtenerMapa())
            compra.AgregarEntrada(asiento, new SinDescuento());

        compra.Confirmar(ahora.AddMinutes(AsientoSesion.TTL.TotalMinutes));

        Assert.Equal(EstadoCompra.Confirmada, compra.Estado);
        Assert.All(sesion.ObtenerMapa(), asiento => Assert.Equal(EstadoAsiento.Vendido, asiento.Estado));
    }

    [Fact]
    public void Confirmar_RechazaEntradasCaducadas()
    {
        var ahora = FechaPrueba();
        var (sesion, asientos) = CrearSesionConAsientos();
        sesion.BloquearAsientos(new[] { asientos[0] }, 25, ahora);
        var compra = new Compra(25, sesion, ahora);
        compra.AgregarEntrada(sesion.ObtenerMapa()[0], new SinDescuento());

        Assert.Throws<InvalidOperationException>(
            () => compra.Confirmar(ahora.Add(AsientoSesion.TTL).AddSeconds(1)));
        Assert.Equal(EstadoCompra.Pendiente, compra.Estado);
        Assert.Equal(EstadoAsiento.Bloqueado, sesion.ObtenerMapa()[0].Estado);
    }

    [Fact]
    public void EmitirTickets_DevuelveUnTicketPorEntradaConfirmada()
    {
        var ahora = FechaPrueba();
        var (sesion, asientos) = CrearSesionConAsientos();
        sesion.BloquearAsientos(asientos, 25, ahora);
        var compra = new Compra(25, sesion, ahora);
        foreach (var asiento in sesion.ObtenerMapa())
            compra.AgregarEntrada(asiento, new DescuentoDiaEspectador());
        compra.Confirmar(ahora);

        var tickets = compra.EmitirTickets().ToList();

        Assert.Equal(2, tickets.Count);
        Assert.All(tickets, ticket =>
        {
            Assert.Equal(5m, ticket.PrecioPagado);
            Assert.Equal("Prueba", ticket.Pelicula());
            Assert.Equal(FechaInicioSesion(), ticket.Hora());
        });
        Assert.Equal(new[] { "A1", "A2" }, tickets.Select(ticket => ticket.Asiento()));
        Assert.All(tickets, ticket => Assert.Matches("^[A-Z0-9]{8}$", ticket.Codigo));
    }

    [Fact]
    public void Cancelar_LiberaLasEntradasYMarcaLaCompraComoCancelada()
    {
        var ahora = FechaPrueba();
        var (sesion, asientos) = CrearSesionConAsientos();
        sesion.BloquearAsientos(new[] { asientos[0] }, 25, ahora);
        var compra = new Compra(25, sesion, ahora);
        compra.AgregarEntrada(sesion.ObtenerMapa()[0], new SinDescuento());

        compra.Cancelar();
        compra.Cancelar();

        Assert.Equal(EstadoCompra.Cancelada, compra.Estado);
        Assert.Equal(EstadoAsiento.Libre, sesion.ObtenerMapa()[0].Estado);
    }

    [Fact]
    public void Cancelar_RechazaUnaCompraConfirmada()
    {
        var ahora = FechaPrueba();
        var (sesion, asientos) = CrearSesionConAsientos();
        sesion.BloquearAsientos(new[] { asientos[0] }, 25, ahora);
        var compra = new Compra(25, sesion, ahora);
        compra.AgregarEntrada(sesion.ObtenerMapa()[0], new SinDescuento());
        compra.Confirmar(ahora);

        Assert.Throws<InvalidOperationException>(() => compra.Cancelar());
    }

    [Fact]
    public void EmitirTickets_RechazaUnaCompraNoConfirmada()
    {
        var compra = new Compra(25, CrearSesion(), FechaPrueba());

        Assert.Throws<InvalidOperationException>(() => compra.EmitirTickets());
    }

    private static Sesion CrearSesion()
    {
        return CrearSesionConAsientos().Sesion;
    }

    private static (Sesion Sesion, Asiento[] Asientos) CrearSesionConAsientos()
    {
        var asientos = new[]
        {
            new Asiento('A', 1, TipoAsiento.Estandar),
            new Asiento('A', 2, TipoAsiento.Estandar)
        };
        var sala = new Sala(1, 2, asientos);
        var pelicula = new Pelicula(1, "Prueba", 90, "Drama", 12);
        var sesion = new Sesion(1, FechaInicioSesion(), sala, pelicula);

        return (sesion, asientos);
    }

    private static DateTime FechaPrueba() => new(2026, 1, 1, 18, 0, 0);

    private static DateTime FechaInicioSesion() => new(2026, 1, 1, 20, 0, 0);
}
