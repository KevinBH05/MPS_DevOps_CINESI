namespace CineSi.Tests;

public class SesionTests
{
    [Fact]
    public void Constructor_CreaLaSesionYUnMapaPorCadaAsientoDeLaSala()
    {
        var inicio = FechaInicio();
        var sala = CrearSala();
        var pelicula = CrearPelicula();

        var sesion = new Sesion(4, inicio, sala, pelicula);

        Assert.Equal(4, sesion.Id);
        Assert.Equal(inicio, sesion.Inicio);
        Assert.Same(sala, sesion.Sala);
        Assert.Same(pelicula, sesion.Pelicula);
        Assert.Equal(2, sesion.ObtenerMapa().Count);
        Assert.All(sesion.ObtenerMapa(), asiento => Assert.Equal(EstadoAsiento.Libre, asiento.Estado));
    }

    [Fact]
    public void ObtenerMapa_DevuelveUnaCopiaDeLaLista()
    {
        var sesion = CrearSesion(out _, out _);

        var mapa = sesion.ObtenerMapa();
        mapa.Clear();

        Assert.Equal(2, sesion.ObtenerMapa().Count);
    }

    [Fact]
    public void ObtenerAsientoSesion_DevuelveElAsientoCorrespondiente()
    {
        var sesion = CrearSesion(out _, out var asientos);

        var resultado = sesion.ObtenerAsientoSesion(asientos[1]);

        Assert.Same(asientos[1], resultado.Asiento);
    }

    [Fact]
    public void ObtenerAsientoSesion_RechazaUnAsientoDeOtraSala()
    {
        var sesion = CrearSesion(out _, out _);
        var asientoAjeno = new Asiento('Z', 1, TipoAsiento.Estandar);

        Assert.Throws<ArgumentException>(() => sesion.ObtenerAsientoSesion(asientoAjeno));
    }

    [Fact]
    public void EstaDisponible_DevuelveFalseMientrasElAsientoEstaBloqueado()
    {
        var ahora = FechaPrueba();
        var sesion = CrearSesion(out _, out var asientos);
        sesion.BloquearAsientos(new[] { asientos[0] }, 10, ahora);

        Assert.False(sesion.EstaDisponible(asientos[0], ahora.AddMinutes(1)));
    }

    [Fact]
    public void EstaDisponible_DevuelveTrueCuandoElBloqueoHaCaducado()
    {
        var ahora = FechaPrueba();
        var sesion = CrearSesion(out _, out var asientos);
        sesion.BloquearAsientos(new[] { asientos[0] }, 10, ahora);

        Assert.True(sesion.EstaDisponible(asientos[0], ahora.Add(AsientoSesion.TTL).AddSeconds(1)));
    }

    [Fact]
    public void BloquearAsientos_BloqueaTodosLosAsientosYDevuelveUnIdentificador()
    {
        var ahora = FechaPrueba();
        var sesion = CrearSesion(out _, out var asientos);

        var bloqueoId = sesion.BloquearAsientos(asientos, 10, ahora);

        Assert.Equal(1, bloqueoId);
        Assert.All(sesion.ObtenerMapa(), asiento =>
        {
            Assert.Equal(EstadoAsiento.Bloqueado, asiento.Estado);
            Assert.Equal(10, asiento.BloqueadoPor);
            Assert.Equal(ahora, asiento.BloqueadoEn);
        });
    }

    [Fact]
    public void BloquearAsientos_EliminaAsientosDuplicados()
    {
        var sesion = CrearSesion(out _, out var asientos);

        var bloqueoId = sesion.BloquearAsientos(
            new[] { asientos[0], asientos[0] }, 10, FechaPrueba());

        sesion.Confirmar(bloqueoId, FechaPrueba());

        Assert.Equal(EstadoAsiento.Vendido, sesion.ObtenerMapa()[0].Estado);
    }

    [Fact]
    public void BloquearAsientos_RechazaUnaSeleccionVacia()
    {
        var sesion = CrearSesion(out _, out _);

        Assert.Throws<ArgumentException>(
            () => sesion.BloquearAsientos(Array.Empty<Asiento>(), 10, FechaPrueba()));
    }

    [Fact]
    public void BloquearAsientos_NoDejaBloqueosParcialesSiUnAsientoNoEstaDisponible()
    {
        var ahora = FechaPrueba();
        var sesion = CrearSesion(out _, out var asientos);
        sesion.BloquearAsientos(new[] { asientos[0] }, 10, ahora);

        Assert.Throws<InvalidOperationException>(
            () => sesion.BloquearAsientos(asientos, 20, ahora));
        Assert.Equal(EstadoAsiento.Bloqueado, sesion.ObtenerMapa()[0].Estado);
        Assert.Equal(EstadoAsiento.Libre, sesion.ObtenerMapa()[1].Estado);
    }

    [Fact]
    public void Liberar_DevuelveLosAsientosAlEstadoLibre()
    {
        var ahora = FechaPrueba();
        var sesion = CrearSesion(out _, out var asientos);
        var bloqueoId = sesion.BloquearAsientos(asientos, 10, ahora);

        sesion.Liberar(bloqueoId);

        Assert.All(sesion.ObtenerMapa(), asiento =>
        {
            Assert.Equal(EstadoAsiento.Libre, asiento.Estado);
            Assert.Null(asiento.BloqueadoPor);
            Assert.Null(asiento.BloqueadoEn);
        });
    }

    [Fact]
    public void Liberar_RechazaUnBloqueoInexistente()
    {
        var sesion = CrearSesion(out _, out _);

        Assert.Throws<InvalidOperationException>(() => sesion.Liberar(99));
    }

    [Fact]
    public void Confirmar_ConvierteLosAsientosBloqueadosEnVendidos()
    {
        var ahora = FechaPrueba();
        var sesion = CrearSesion(out _, out var asientos);
        var bloqueoId = sesion.BloquearAsientos(asientos, 10, ahora);

        sesion.Confirmar(bloqueoId, ahora.AddMinutes(1));

        Assert.All(sesion.ObtenerMapa(), asiento => Assert.Equal(EstadoAsiento.Vendido, asiento.Estado));
    }

    [Fact]
    public void Confirmar_RechazaUnBloqueoCaducado()
    {
        var ahora = FechaPrueba();
        var sesion = CrearSesion(out _, out var asientos);
        var bloqueoId = sesion.BloquearAsientos(asientos, 10, ahora);

        Assert.Throws<InvalidOperationException>(
            () => sesion.Confirmar(bloqueoId, ahora.Add(AsientoSesion.TTL).AddSeconds(1)));
        Assert.All(sesion.ObtenerMapa(), asiento => Assert.Equal(EstadoAsiento.Bloqueado, asiento.Estado));
    }

    [Fact]
    public void ExpirarBloqueos_LiberaSoloLosAsientosCaducados()
    {
        var ahora = FechaPrueba();
        var sesion = CrearSesion(out _, out var asientos);
        sesion.BloquearAsientos(new[] { asientos[0] }, 10, ahora);

        var liberados = sesion.ExpirarBloqueos(ahora.Add(AsientoSesion.TTL).AddSeconds(1));

        Assert.Equal(1, liberados);
        Assert.Equal(EstadoAsiento.Libre, sesion.ObtenerMapa()[0].Estado);
        Assert.Equal(EstadoAsiento.Libre, sesion.ObtenerMapa()[1].Estado);
    }

    [Fact]
    public void Fin_SumaLaDuracionDeLaPeliculaAlInicio()
    {
        var inicio = FechaInicio();
        var pelicula = new Pelicula(1, "Prueba", 100, "Drama", 12);
        var sala = CrearSala();

        var sesion = new Sesion(1, inicio, sala, pelicula);

        Assert.Equal(inicio.AddMinutes(100), sesion.Fin());
    }

    private static Sesion CrearSesion(out Sala sala, out Asiento[] asientos)
    {
        asientos =
        [
            new Asiento('A', 1, TipoAsiento.Estandar),
            new Asiento('A', 2, TipoAsiento.Estandar)
        ];
        sala = new Sala(1, 2, asientos);
        return new Sesion(1, FechaInicio(), sala, CrearPelicula());
    }

    private static Sala CrearSala()
    {
        return new Sala(1, 2,
        [
            new Asiento('A', 1, TipoAsiento.Estandar),
            new Asiento('A', 2, TipoAsiento.Estandar)
        ]);
    }

    private static Pelicula CrearPelicula()
    {
        return new Pelicula(1, "Prueba", 90, "Drama", 12);
    }

    private static DateTime FechaPrueba() => new(2026, 1, 1, 18, 0, 0);

    private static DateTime FechaInicio() => new(2026, 1, 1, 20, 0, 0);
}
