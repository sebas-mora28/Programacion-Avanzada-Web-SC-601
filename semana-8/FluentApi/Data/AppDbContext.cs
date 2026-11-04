using FluentApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FluentApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Etiqueta> Etiquetas => Set<Etiqueta>();

    // Configura el modelo de EF; no ejecuta consultas ni crea tablas por sí solo.
    // Las migraciones convierten estos metadatos en cambios del esquema PostgreSQL.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Etiqueta>(entity =>
        {
            entity.ToTable("Etiquetas");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).HasColumnName("nombre").IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Nombre).IsUnique();
            entity.HasMany(e => e.Productos).WithMany(p => p.Etiquetas);
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("Categorias");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).HasColumnName("id");
            entity.Property(c => c.Nombre).HasColumnName("nombre").IsRequired().HasMaxLength(80);
            entity.HasIndex(c => c.Nombre).IsUnique();
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("Productos");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Nombre)
                .HasColumnName("nombre_producto")
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(p => p.Descripcion).HasColumnName("descripcion").HasMaxLength(300);
            entity.Property(p => p.Precio).HasColumnName("precio").HasPrecision(10, 2);
            entity.Ignore(p => p.Etiqueta);

            // M:N: un producto tiene muchas etiquetas y cada etiqueta tiene muchos productos.
            // HasMany selecciona la colección de Producto; WithMany selecciona la inversa.
            // entity.HasMany(p => p.Etiquetas)
            //     .WithMany(e => e.Productos)
            //     // Tabla de unión administrada por EF, sin una clase de entidad propia.
            //     // Cada fila representa UNA asociación, por ejemplo (Oferta, Teclado).
            //     .UsingEntity<Dictionary<string, object>>(
            //         "EtiquetaProducto",
            //         // Primera FK: cada fila de la unión apunta a una etiqueta.
            //         // WithMany() no especifica una navegación hacia la entidad de unión.
            //         derecha => derecha.HasOne<Etiqueta>().WithMany()
            //             .HasForeignKey("EtiquetasId").OnDelete(DeleteBehavior.Cascade),
            //         // Segunda FK: cada fila de la unión apunta a un producto.
            //         // Estas dos relaciones 1:N implementan la relación M:N.
            //         izquierda => izquierda.HasOne<Producto>().WithMany()
            //             .HasForeignKey("ProductosId").OnDelete(DeleteBehavior.Cascade),
            //         union =>
            //         {
            //             union.ToTable("EtiquetaProducto");
            //             // La pareja es única: un producto no puede tener dos veces la misma etiqueta.
            //             union.HasKey("EtiquetasId", "ProductosId");
            //         });
            // Cascade en la unión solo borra asociaciones al eliminar un extremo.
            // Eliminar Teclado no elimina Oferta ni los otros productos que tienen Oferta.

            // 1:N: cada producto pertenece a una categoría; una categoría agrupa muchos productos.
            entity.HasOne(p => p.Categoria)
                .WithMany(c => c.Productos)
                // La FK está en Producto (lado dependiente), no en Categoria.
                .HasForeignKey(p => p.CategoriaId)
                // Aquí Cascade sí elimina productos si se elimina su categoría.
                // CategoriaId es int: la relación no admite productos sin categoría.
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
