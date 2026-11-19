namespace CineSi.Tests;

public class AsientoSesionTests
{
    [Fact]
    public void Constructor_GuardaElAsientoYEmpiezaLibre()
    {
        var asiento = CrearAsiento();

        var asientoSesion = new AsientoSesion(asiento);

        Assert.Same(asiento, asientoSesion.Asiento);
        Assert.Equal(EstadoAsiento.Libre, asientoSesion.Estado);
        Assert.Null(asientoSesion.BloqueadoPor);
        Assert.Null(asientoSesion.BloqueadoEn);
    }

    [Fact]
    public void Bloquear_CambiaElEstadoYGuardaElClienteYLaFecha()
    {
        var ahora = FechaPrueba();
        var asientoSesion = CrearAsientoSesion();

        asientoSesion.Bloquear(42, ahora);

        Assert.Equal(EstadoAsiento.Bloqueado, asientoSesion.Estado);
        Assert.Equal(42, asientoSesion.BloqueadoPor);
        Assert.Equal(ahora, asientoSesion.BloqueadoEn);
    }

    [Fact]
    public void Bloquear_RechazaUnAsientoBloqueadoQueNoHaCaducado()
    {
        var ahora = FechaPrueba();
        var asientoSesion = CrearAsientoSesion();
        asientoSesion.Bloquear(42, ahora);

        var excepcion = Assert.Throws<InvalidOperationException>(
            () => asientoSesion.Bloquear(99, ahora.AddMinutes(1)));

        Assert.Contains("está bloqueado por otro cliente", excepcion.Message);
        Assert.Equal(42, asientoSesion.BloqueadoPor);
    }

    [Fact]
    public void Bloquear_PermiteReutilizarUnAsientoCuandoElBloqueoHaCaducado()
    {
        var ahora = FechaPrueba();
        var asientoSesion = CrearAsientoSesion();
        asientoSesion.Bloquear(42, ahora);

        asientoSesion.Bloquear(99, ahora.Add(AsientoSesion.TTL).AddSeconds(1));

        Assert.Equal(EstadoAsiento.Bloqueado, asientoSesion.Estado);
        Assert.Equal(99, asientoSesion.BloqueadoPor);
    }

    [Fact]
    public void Bloquear_RechazaUnAsientoVendido()
    {
        var asientoSesion = CrearAsientoSesion();
        asientoSesion.Bloquear(42, FechaPrueba());
        asientoSesion.Vender();

        var excepcion = Assert.Throws<InvalidOperationException>(
            () => asientoSesion.Bloquear(99, FechaPrueba().AddMinutes(10)));

        Assert.Contains("ya está vendido", excepcion.Message);
    }

    [Fact]
    public void Vender_ConvierteUnAsientoBloqueadoEnVendido()
    {
        var asientoSesion = CrearAsientoSesion();
        asientoSesion.Bloquear(42, FechaPrueba());

        asientoSesion.Vender();

        Assert.Equal(EstadoAsiento.Vendido, asientoSesion.Estado);
    }

    [Theory]
    [InlineData(EstadoAsiento.Libre)]
    [InlineData(EstadoAsiento.Vendido)]
    public void Vender_RechazaUnAsientoQueNoEstaBloqueado(EstadoAsiento estado)
    {
        var asientoSesion = CrearAsientoSesion();
        if (estado == EstadoAsiento.Vendido)
        {
            asientoSesion.Bloquear(42, FechaPrueba());
            asientoSesion.Vender();
        }

        var excepcion = Assert.Throws<InvalidOperationException>(
            () => asientoSesion.Vender());

        Assert.Contains("Solo se puede vender un asiento bloqueado", excepcion.Message);
    }

    [Fact]
    public void Liberar_RestableceElAsientoALibreYEliminaLosDatosDelBloqueo()
    {
        var asientoSesion = CrearAsientoSesion();
        asientoSesion.Bloquear(42, FechaPrueba());

        asientoSesion.Liberar();

        Assert.Equal(EstadoAsiento.Libre, asientoSesion.Estado);
        Assert.Null(asientoSesion.BloqueadoPor);
        Assert.Null(asientoSesion.BloqueadoEn);
    }

    [Fact]
    public void Liberar_RechazaUnAsientoVendido()
    {
        var asientoSesion = CrearAsientoSesion();
        asientoSesion.Bloquear(42, FechaPrueba());
        asientoSesion.Vender();

        var excepcion = Assert.Throws<InvalidOperationException>(
            () => asientoSesion.Liberar());

        Assert.Contains("No se puede liberar un asiento vendido", excepcion.Message);
    }

    [Fact]
    public void HaExpirado_SoloEsVerdaderoDespuesDelTiempoDeBloqueo()
    {
        var ahora = FechaPrueba();
        var asientoSesion = CrearAsientoSesion();

        Assert.False(asientoSesion.HaExpirado(ahora.Add(AsientoSesion.TTL)));

        asientoSesion.Bloquear(42, ahora);

        Assert.False(asientoSesion.HaExpirado(ahora.Add(AsientoSesion.TTL)));
        Assert.True(asientoSesion.HaExpirado(ahora.Add(AsientoSesion.TTL).AddTicks(1)));
    }

    private static AsientoSesion CrearAsientoSesion()
    {
        return new AsientoSesion(CrearAsiento());
    }

    private static Asiento CrearAsiento()
    {
        return new Asiento('A', 1, TipoAsiento.Estandar);
    }

    private static DateTime FechaPrueba()
    {
        return new DateTime(2026, 1, 1, 18, 0, 0);
    }
}
