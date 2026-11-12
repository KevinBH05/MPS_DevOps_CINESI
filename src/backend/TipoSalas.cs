namespace CineSi;
 
public interface ISalaFactory
{
    Sala CrearSala();
}
 
/// <summary>Lógica común de las factories: las plazas minusválidas van al inicio de la fila A.</summary>
internal static class ConstructorSala
{
    public static Sala Construir(int filas, int asientosPorFila, int plazasMinusvalidos)
    {
        var asientos = new List<Asiento>();
        int minusvalidosRestantes = plazasMinusvalidos;
 
        for (int f = 0; f < filas; f++)
        {
            for (int n = 1; n <= asientosPorFila; n++)
            {
                var tipo = TipoAsiento.Estandar;
                if (minusvalidosRestantes > 0)
                {
                    tipo = TipoAsiento.Minusvalido;
                    minusvalidosRestantes--;
                }
                asientos.Add(new Asiento((char)('A' + f), n, tipo));
            }
        }
 
        return new Sala(filas, asientosPorFila, asientos);
    }
}
 
public class SalaGrandeFactory : ISalaFactory
{
    private const int Filas = 10;
    private const int AsientosPorFila = 20;
    private const int PlazasMinusvalidos = 10;
 
    public Sala CrearSala() => ConstructorSala.Construir(Filas, AsientosPorFila, PlazasMinusvalidos);
}
 
public class SalaPequenaFactory : ISalaFactory
{
    private const int Filas = 5;
    private const int AsientosPorFila = 10;
    private const int PlazasMinusvalidos = 5;
 
    public Sala CrearSala() => ConstructorSala.Construir(Filas, AsientosPorFila, PlazasMinusvalidos);
}