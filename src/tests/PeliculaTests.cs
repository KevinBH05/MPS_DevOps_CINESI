namespace CineSi.Tests;

public class PeliculaTests
{
    [Fact]
    public void Constructor_GuardaLosDatosDeLaPelicula()
    {
        var pelicula = new Pelicula(7, "El viaje", 125, "Aventura", 12);

        Assert.Equal(7, pelicula.Id);
        Assert.Equal("El viaje", pelicula.Titulo);
        Assert.Equal(125, pelicula.DuracionMin);
        Assert.Equal("Aventura", pelicula.Genero);
        Assert.Equal(12, pelicula.EdadMinima);
    }

    [Theory]
    [InlineData(12, true)]
    [InlineData(18, true)]
    [InlineData(11, false)]
    [InlineData(0, false)]
    public void EsApta_ComparaLaEdadConLaEdadMinima(int edad, bool resultadoEsperado)
    {
        var pelicula = new Pelicula(7, "El viaje", 125, "Aventura", 12);

        var resultado = pelicula.EsApta(edad);

        Assert.Equal(resultadoEsperado, resultado);
    }

    [Fact]
    public void EsApta_UnaPeliculaSinRestriccionEsAptaParaCualquierEdad()
    {
        var pelicula = new Pelicula(7, "El viaje", 125, "Aventura", 0);

        Assert.True(pelicula.EsApta(0));
    }
}
