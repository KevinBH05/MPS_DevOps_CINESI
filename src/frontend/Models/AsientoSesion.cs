namespace CineFrontend.Models;

public enum TipoAsiento
{
    Estandar,
    Minusvalido
}

public enum EstadoAsiento
{
    Libre,
    Bloqueado,
    Vendido
}

public class AsientoSesion
{
    public char Fila { get; set; }
    public int Numero { get; set; }
    public TipoAsiento Tipo { get; set; } = TipoAsiento.Estandar;
    public EstadoAsiento Estado { get; set; } = EstadoAsiento.Libre;

    // Propiedad auxiliar útil para pintar la etiqueta en el botón (ej: "A1", "B4")
    public string Etiqueta => $"{Fila}{Numero}";
}