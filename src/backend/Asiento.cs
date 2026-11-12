namespace CineSi;
 
/// <summary>Asiento físico de una sala. No guarda si está libre u ocupado.</summary>
public class Asiento
{
    public char Fila { get; }
    public int Numero { get; }
    public TipoAsiento Tipo { get; }
 
    public Asiento(char fila, int numero, TipoAsiento tipo)
    {
        Fila = fila;
        Numero = numero;
        Tipo = tipo;
    }
 
    public string Etiqueta() => $"{Fila}{Numero}";
 
    public bool EsMinusvalido() => Tipo == TipoAsiento.Minusvalido;
}