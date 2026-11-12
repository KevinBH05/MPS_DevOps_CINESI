namespace CineFrontend.Models;

public class Pelicula
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public int DuracionMin { get; set; }
    public string Genero { get; set; } = string.Empty;
    public int EdadMinima {get; set; }
}