using Microsoft.EntityFrameworkCore;
using PruebasDemo.Api.Data;
using PruebasDemo.Api.Models;

namespace PruebasDemo.Api.Repositories;

public class ProductoRepository(AppDbContext db) : IProductoRepository
{
    // La misma consulta LINQ se traduce a SQL según el proveedor configurado:
    // PostgreSQL en la aplicación y SQLite en las pruebas de integración.
    // AsNoTracking es adecuado porque la cotización solo consulta el producto.
    public Task<Producto?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.Productos.AsNoTracking().SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
}
