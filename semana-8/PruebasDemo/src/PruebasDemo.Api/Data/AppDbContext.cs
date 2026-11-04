using Microsoft.EntityFrameworkCore;
using PruebasDemo.Api.Models;

namespace PruebasDemo.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Producto> Productos => Set<Producto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("Productos");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Precio).HasPrecision(10, 2);

            // Datos fijos para la clase. PostgreSQL los recibe con la migración inicial;
            // SQLite los recibe al crear su esquema con EnsureCreated en la fábrica de pruebas.
            entity.HasData(new Producto(1, "Teclado", 100m), new Producto(2, "Mouse", 25m));
        });
    }
}
