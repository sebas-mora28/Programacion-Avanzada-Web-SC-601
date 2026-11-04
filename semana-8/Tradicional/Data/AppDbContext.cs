using Microsoft.EntityFrameworkCore;
using Tradicional.Models;

namespace Tradicional.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Etiqueta> Etiquetas => Set<Etiqueta>();

    // Sin OnModelCreating: EF combina atributos con convenciones para construir el modelo.
    //
    // 1:N Categoria -> Productos:
    // Producto.CategoriaId es la FK obligatoria. Por convención, eliminar una categoría
    // también elimina sus productos (Cascade); eliminar un producto no elimina su categoría.
    //
    // M:N Productos <-> Etiquetas:
    // Las colecciones de ambos modelos originan la tabla intermedia EtiquetaProducto.
    // Su PK compuesta (EtiquetasId, ProductosId) impide repetir una misma asociación.
    // No se necesita un DbSet para esa tabla: EF la administra mediante las colecciones.
    // Eliminar un producto o una etiqueta elimina sus filas intermedias por cascada,
    // pero no elimina las entidades del otro extremo.
}
