namespace Semana05HttpRouting.Models;

// DTO: solo los campos que acepta la operación. El cliente no asigna el Id.
public record CrearAuto(string? Marca, string? Modelo, decimal Precio);
public record Auto(int Id, string Marca, string Modelo, decimal Precio);
public record MensajeEntrada(string? Mensaje);
public record CotizacionEntrada(decimal Precio, decimal PorcentajeDescuento);
