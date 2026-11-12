namespace CineSi.Tests;

public class TipoSalasTests
{
    [Fact]
    public void SalaGrandeFactory_CreaUnaSalaConLasDimensionesEsperadas()
    {
        var sala = new SalaGrandeFactory().CrearSala();

        Assert.Equal(10, sala.Filas);
        Assert.Equal(20, sala.AsientosPorFila);
        Assert.Equal(200, sala.Capacidad());
        Assert.Equal(10, sala.CapacidadMinusvalidos());
    }

    [Fact]
    public void SalaPequenaFactory_CreaUnaSalaConLasDimensionesEsperadas()
    {
        var sala = new SalaPequenaFactory().CrearSala();

        Assert.Equal(5, sala.Filas);
        Assert.Equal(10, sala.AsientosPorFila);
        Assert.Equal(50, sala.Capacidad());
        Assert.Equal(5, sala.CapacidadMinusvalidos());
    }

    [Theory]
    [InlineData(10, 20, 10)]
    [InlineData(5, 10, 5)]
    public void ConstructorSala_ColocaLasPlazasMinusvalidasAlInicio(
        int filas,
        int asientosPorFila,
        int plazasMinusvalidos)
    {
        var sala = ConstructorSala.Construir(filas, asientosPorFila, plazasMinusvalidos);

        var asientos = sala.Asientos.ToList();
        Assert.All(
            asientos.Take(plazasMinusvalidos),
            asiento => Assert.Equal(TipoAsiento.Minusvalido, asiento.Tipo));
        Assert.All(
            asientos.Skip(plazasMinusvalidos),
            asiento => Assert.Equal(TipoAsiento.Estandar, asiento.Tipo));
    }

    [Fact]
    public void ConstructorSala_GeneraFilasYNumerosEnOrden()
    {
        var sala = ConstructorSala.Construir(2, 3, 0);

        Assert.Equal(
            new[] { "A1", "A2", "A3", "B1", "B2", "B3" },
            sala.Asientos.Select(asiento => asiento.Etiqueta()));
    }

    [Fact]
    public void Factories_ImplementanISalaFactory()
    {
        Assert.IsAssignableFrom<ISalaFactory>(new SalaGrandeFactory());
        Assert.IsAssignableFrom<ISalaFactory>(new SalaPequenaFactory());
    }
}
