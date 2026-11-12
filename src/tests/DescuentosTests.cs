namespace CineSi.Tests;

public class DescuentosTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(10, 10)]
    [InlineData(25.50, 25.50)]
    public void SinDescuento_DevuelveElPrecioBase(decimal precioBase, decimal precioEsperado)
    {
        var descuento = new SinDescuento();

        Assert.Equal(precioEsperado, descuento.Calcular(precioBase));
    }

    [Theory]
    [InlineData(10, 7.50)]
    [InlineData(20, 15)]
    [InlineData(25.50, 19.125)]
    public void DescuentoEstudiante_AplicaEl25PorCiento(decimal precioBase, decimal precioEsperado)
    {
        var descuento = new DescuentoEstudiante();

        Assert.Equal(precioEsperado, descuento.Calcular(precioBase));
    }

    [Theory]
    [InlineData(10, 6)]
    [InlineData(20, 12)]
    [InlineData(25.50, 15.30)]
    public void DescuentoSenior_AplicaEl40PorCiento(decimal precioBase, decimal precioEsperado)
    {
        var descuento = new DescuentoSenior();

        Assert.Equal(precioEsperado, descuento.Calcular(precioBase));
    }

    [Theory]
    [InlineData(10, 5)]
    [InlineData(20, 10)]
    [InlineData(25.50, 12.75)]
    public void DescuentoDiaEspectador_AplicaEl50PorCiento(decimal precioBase, decimal precioEsperado)
    {
        var descuento = new DescuentoDiaEspectador();

        Assert.Equal(precioEsperado, descuento.Calcular(precioBase));
    }

    [Fact]
    public void TodasLasEstrategias_ImplementanIDescuentoStrategy()
    {
        var estrategias = new IDescuentoStrategy[]
        {
            new SinDescuento(),
            new DescuentoEstudiante(),
            new DescuentoSenior(),
            new DescuentoDiaEspectador()
        };

        Assert.All(estrategias, estrategia => Assert.IsAssignableFrom<IDescuentoStrategy>(estrategia));
    }
}
