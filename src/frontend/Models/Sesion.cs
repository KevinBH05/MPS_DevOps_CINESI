namespace CineFrontend.Models;

public class Sesion
{
    public int Id { get; set; }
    public Pelicula Pelicula { get; set; } = new();
    public string NombreSala { get; set; } = string.Empty;
    public DateTime Inicio { get; set; }
    public decimal TarifaBase { get; set; }
    public string Formato { get; set; } = "2D";
}