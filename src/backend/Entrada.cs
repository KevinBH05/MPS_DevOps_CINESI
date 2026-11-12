namespace CineSi;
 
public class Entrada
{
    public const decimal PrecioBase = 10m;
 
    private IDescuentoStrategy _descuento = new SinDescuento();
 
    public AsientoSesion AsientoSesion { get; }
 
    public Entrada(AsientoSesion asientoSesion)
    {
        AsientoSesion = asientoSesion;
    }
 
    public void ElegirDescuento(IDescuentoStrategy tipo)
    {
        _descuento = tipo ?? throw new ArgumentNullException(nameof(tipo));
    }
 
    public decimal PrecioFinal()
        => decimal.Round(_descuento.Calcular(PrecioBase), 2);
}