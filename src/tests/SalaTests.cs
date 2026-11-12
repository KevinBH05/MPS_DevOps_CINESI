namespace CineSi.Tests;

public class SalaTests
{
    [Fact]
    public void Constructor_AlmacenaLaConfiguracionYLosAsientos()
    {
        var asientos = CrearAsientos();

        var sala = new Sala(2, 3, asientos);

        Assert.Equal(2, sala.Filas);
        Assert.Equal(3, sala.AsientosPorFila);
        Assert.Equal(asientos.Length, sala.Asientos.Count);
        Assert.Equal(asientos, sala.Asientos);
    }

    [Fact]
    public void Capacidad_DevuelveElNumeroDeAsientos()
    {
        var sala = CrearSala();

        Assert.Equal(4, sala.Capacidad());
    }

    [Fact]
    public void CapacidadMinusvalidos_CuentaSoloLosAsientosAdaptados()
    {
        var sala = CrearSala();

        Assert.Equal(1, sala.CapacidadMinusvalidos());
    }

    [Fact]
    public void ObtenerAsiento_DevuelveElAsientoDeLaFilaYNumeroIndicados()
    {
        var sala = CrearSala();

        var asiento = sala.ObtenerAsiento('B', 1);

        Assert.Equal('B', asiento.Fila);
        Assert.Equal(1, asiento.Numero);
        Assert.Equal(TipoAsiento.Minusvalido, asiento.Tipo);
    }

    [Fact]
    public void ObtenerAsiento_LanzaArgumentExceptionCuandoNoExiste()
    {
        var sala = CrearSala();

        var excepcion = Assert.Throws<ArgumentException>(
            () => sala.ObtenerAsiento('C', 1));

        Assert.Contains("No existe el asiento C1", excepcion.Message);
    }

    [Theory]
    [InlineData(TipoAsiento.Estandar, 3)]
    [InlineData(TipoAsiento.Minusvalido, 1)]
    public void AsientosPorTipo_FiltraPorTipo(TipoAsiento tipo, int cantidadEsperada)
    {
        var sala = CrearSala();

        var resultado = sala.AsientosPorTipo(tipo).ToList();

        Assert.Equal(cantidadEsperada, resultado.Count);
        Assert.All(resultado, asiento => Assert.Equal(tipo, asiento.Tipo));
    }

    private static Sala CrearSala()
    {
        return new Sala(2, 2, CrearAsientos());
    }

    private static Asiento[] CrearAsientos()
    {
        return
        [
            new Asiento('A', 1, TipoAsiento.Estandar),
            new Asiento('A', 2, TipoAsiento.Estandar),
            new Asiento('B', 1, TipoAsiento.Minusvalido),
            new Asiento('B', 2, TipoAsiento.Estandar)
        ];
    }
}
