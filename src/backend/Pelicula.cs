namespace CineSi;
 
public class Pelicula
{
    public int Id { get; }
    public string Titulo { get; }
    public int DuracionMin { get; }
    public string Genero { get; }
    public int EdadMinima { get; }
 
    public Pelicula(int id, string titulo, int duracionMin, string genero, int edadMinima)
    {
        Id = id;
        Titulo = titulo;
        DuracionMin = duracionMin;
        Genero = genero;
        EdadMinima = edadMinima;
    }
 
    public bool EsApta(int edad) => edad >= EdadMinima;
}