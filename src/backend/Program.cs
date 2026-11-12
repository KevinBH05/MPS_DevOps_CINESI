using CineSi;
 
Console.OutputEncoding = System.Text.Encoding.UTF8;
 
// 1. Sala (Factory), película y sesión
ISalaFactory factory = new SalaGrandeFactory();
Sala sala = factory.CrearSala();
var pelicula = new Pelicula(1, "Dune", 155, "Ciencia ficción", 12);
var sesion = new Sesion(1, new DateTime(2026, 11, 14, 18, 0, 0), sala, pelicula);
 
Console.WriteLine($"Sala {sala.Id}: {sala.Capacidad()} asientos ({sala.CapacidadMinusvalidos()} minusválidos)");
Console.WriteLine($"{pelicula.Titulo}: {sesion.Inicio:HH:mm} - {sesion.Fin():HH:mm}\n");
 
// 2. El cliente 42 bloquea dos asientos y compra con descuentos distintos (Strategy)
var e5 = sala.ObtenerAsiento('E', 5);
var e6 = sala.ObtenerAsiento('E', 6);
sesion.BloquearAsientos(new[] { e5, e6 }, clienteId: 42);
 
var compra = new Compra(42, sesion);
compra.AgregarEntrada(sesion.ObtenerAsientoSesion(e5), new SinDescuento());
compra.AgregarEntrada(sesion.ObtenerAsientoSesion(e6), new DescuentoEstudiante());
 
Console.WriteLine($"Subtotal:   {compra.Subtotal():0.00} €");
Console.WriteLine($"Descuentos: {compra.TotalDescuentos():0.00} €");
Console.WriteLine($"Total:      {compra.Total():0.00} €\n");
 
compra.Confirmar();
foreach (var t in compra.EmitirTickets())
    Console.WriteLine($"Ticket {t.Codigo}: {t.Pelicula()} | Sala {t.Sala()} | {t.Hora():dd/MM HH:mm} | Asiento {t.Asiento()} | {t.PrecioPagado:0.00} €");
 
// 3. Un asiento vendido ya no está disponible
Console.WriteLine($"\n¿E5 disponible? {sesion.EstaDisponible(e5)}");
try
{
    sesion.BloquearAsientos(new[] { e5 }, clienteId: 7);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Error esperado: {ex.Message}");
}
 
// 4. Expiración del bloqueo (TTL de 5 minutos)
var f1 = sala.ObtenerAsiento('F', 1);
sesion.BloquearAsientos(new[] { f1 }, clienteId: 7);
Console.WriteLine($"\nF1 disponible justo tras bloquear: {sesion.EstaDisponible(f1)}");
 
int liberados = sesion.ExpirarBloqueos(DateTime.Now.AddMinutes(6));
Console.WriteLine($"Bloqueos expirados a los 6 min: {liberados}");
Console.WriteLine($"F1 disponible tras expirar: {sesion.EstaDisponible(f1)}");