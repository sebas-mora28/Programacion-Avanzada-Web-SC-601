using Microsoft.EntityFrameworkCore;
using FluentAPI.Relaciones.Models;

namespace FluentAPI.Relaciones.Data;

public class StoreDbContext : DbContext
{
    public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // TODO: configurar con Fluent API las claves y relaciones
        // Customer (1) -> Order (N), Order (N) <-> Product (M).
        // No utilizar Data Annotations para configurar las relaciones.
    }
}
