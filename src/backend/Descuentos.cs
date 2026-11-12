namespace CineSi;
 
public interface IDescuentoStrategy
{
    /// <summary>Devuelve el precio final (no el importe descontado).</summary>
    decimal Calcular(decimal precioBase);
}
 
public class SinDescuento : IDescuentoStrategy
{
    public decimal Calcular(decimal precioBase) => precioBase;
}
 
public class DescuentoEstudiante : IDescuentoStrategy
{
    public decimal Calcular(decimal precioBase) => precioBase * 0.75m;
}
 
public class DescuentoSenior : IDescuentoStrategy
{
    public decimal Calcular(decimal precioBase) => precioBase * 0.60m;
}
 
public class DescuentoDiaEspectador : IDescuentoStrategy
{
    public decimal Calcular(decimal precioBase) => precioBase * 0.50m;
}