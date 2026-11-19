namespace CineSi.Tests;

public class AsientoTests
{
    [Fact]
    public void Constructor_GuardaLaFilaElNumeroYElTipo()
    {
        var asiento = new Asiento('B', 12, TipoAsiento.Minusvalido);

        Assert.Equal('B', asiento.Fila);
        Assert.Equal(12, asiento.Numero);
        Assert.Equal(TipoAsiento.Minusvalido, asiento.Tipo);
    }

    [Theory]
    [InlineData('A', 1, "A1")]
    [InlineData('C', 25, "C25")]
    public void Etiqueta_CombinaLaFilaYElNumero(
        char fila,
        int numero,
        string etiquetaEsperada)
    {
        var asiento = new Asiento(fila, numero, TipoAsiento.Estandar);

        Assert.Equal(etiquetaEsperada, asiento.Etiqueta());
    }

    [Theory]
    [InlineData(TipoAsiento.Estandar, false)]
    [InlineData(TipoAsiento.Minusvalido, true)]
    public void EsMinusvalido_IndicaSiElAsientoEsAdaptado(
        TipoAsiento tipo,
        bool resultadoEsperado)
    {
        var asiento = new Asiento('A', 1, tipo);

        Assert.Equal(resultadoEsperado, asiento.EsMinusvalido());
    }
}
