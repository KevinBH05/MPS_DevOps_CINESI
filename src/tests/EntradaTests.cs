namespace CineSi.Tests;

public class EntradaTests
{
    [Fact]
    public void Constructor_GuardaElAsientoYAplicaSinDescuentoPorDefecto()
    {
        var asientoSesion = CrearAsientoSesion();

        var entrada = new Entrada(asientoSesion);

        Assert.Same(asientoSesion, entrada.AsientoSesion);
        Assert.Equal(Entrada.PrecioBase, entrada.PrecioFinal());
    }

    [Fact]
    public void ElegirDescuento_AplicaElDescuentoSeleccionado()
    {
        var entrada = new Entrada(CrearAsientoSesion());

        entrada.ElegirDescuento(new DescuentoEstudiante());

        Assert.Equal(7.50m, entrada.PrecioFinal());
    }

    [Fact]
    public void ElegirDescuento_PuedeCambiarLaEstrategia()
    {
        var entrada = new Entrada(CrearAsientoSesion());

        entrada.ElegirDescuento(new DescuentoSenior());
        entrada.ElegirDescuento(new DescuentoDiaEspectador());

        Assert.Equal(5m, entrada.PrecioFinal());
    }

    [Fact]
    public void ElegirDescuento_RechazaUnaEstrategiaNula()
    {
        var entrada = new Entrada(CrearAsientoSesion());

        Assert.Throws<ArgumentNullException>(
            () => entrada.ElegirDescuento(null!));
    }

    [Fact]
    public void PrecioFinal_RedondeaElResultadoADosDecimales()
    {
        var entrada = new Entrada(CrearAsientoSesion());
        entrada.ElegirDescuento(new DescuentoPersonalizado(10.123m));

        Assert.Equal(10.12m, entrada.PrecioFinal());
    }

    private static AsientoSesion CrearAsientoSesion()
    {
        return new AsientoSesion(new Asiento('A', 1, TipoAsiento.Estandar));
    }

    private sealed class DescuentoPersonalizado(decimal precioFinal) : IDescuentoStrategy
    {
        public decimal Calcular(decimal precioBase) => precioFinal;
    }
}
