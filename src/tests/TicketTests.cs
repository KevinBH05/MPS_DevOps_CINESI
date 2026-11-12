namespace CineSi.Tests;

public class TicketTests
{
    [Fact]
    public void Constructor_GuardaElPrecioPagado()
    {
        var ticket = CrearTicket(7.50m, out _, out _);

        Assert.Equal(7.50m, ticket.PrecioPagado);
    }

    [Fact]
    public void Constructor_GeneraUnCodigoDeOchoCaracteresEnMayusculas()
    {
        var ticket = CrearTicket(10m, out _, out _);

        Assert.Matches("^[A-Z0-9]{8}$", ticket.Codigo);
    }

    [Fact]
    public void Constructor_GeneraCodigosDiferentesParaTicketsDistintos()
    {
        var primerTicket = CrearTicket(10m, out var sesion, out var asiento);
        var segundoTicket = new Ticket(sesion, asiento, 10m);

        Assert.NotEqual(primerTicket.Codigo, segundoTicket.Codigo);
    }

    [Fact]
    public void Pelicula_DevuelveElTituloDeLaSesion()
    {
        var ticket = CrearTicket(10m, out _, out _);

        Assert.Equal("El viaje", ticket.Pelicula());
    }

    [Fact]
    public void Hora_DevuelveElInicioDeLaSesion()
    {
        var ticket = CrearTicket(10m, out _, out _);

        Assert.Equal(new DateTime(2026, 1, 1, 20, 0, 0), ticket.Hora());
    }

    [Fact]
    public void Sala_DevuelveElIdentificadorDeLaSala()
    {
        var ticket = CrearTicket(10m, out var sesion, out _);

        Assert.Equal(sesion.Sala.Id, ticket.Sala());
    }

    [Fact]
    public void Asiento_DevuelveLaEtiquetaDelAsiento()
    {
        var ticket = CrearTicket(10m, out _, out var asiento);

        Assert.Equal(asiento.Etiqueta(), ticket.Asiento());
        Assert.Equal("B3", ticket.Asiento());
    }

    private static Ticket CrearTicket(decimal precio, out Sesion sesion, out Asiento asiento)
    {
        asiento = new Asiento('B', 3, TipoAsiento.Estandar);
        var sala = new Sala(1, 1, new[] { asiento });
        var pelicula = new Pelicula(1, "El viaje", 120, "Aventura", 12);
        sesion = new Sesion(1, new DateTime(2026, 1, 1, 20, 0, 0), sala, pelicula);

        return new Ticket(sesion, asiento, precio);
    }
}
