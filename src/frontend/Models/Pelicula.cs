namespace CineFrontend.Models;

public class Pelicula
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Horario { get; set; } = string.Empty;
    public decimal Precio { get; set; }
}