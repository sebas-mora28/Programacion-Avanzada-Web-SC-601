namespace PruebasDemo.Api.Models;

// EF Core mapea las propiedades y usa este constructor al materializar una fila de Productos.
public record Producto(int Id, string Nombre, decimal Precio);
