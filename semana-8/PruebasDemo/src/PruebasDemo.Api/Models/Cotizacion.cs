namespace PruebasDemo.Api.Models;

public record Cotizacion(
    int ProductoId,
    string Nombre,
    int Cantidad,
    decimal PrecioUnitario,
    decimal Subtotal,
    decimal Descuento,
    decimal Total);
