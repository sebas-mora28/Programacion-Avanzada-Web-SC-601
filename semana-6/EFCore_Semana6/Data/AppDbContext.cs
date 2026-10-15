using EFCore_Semana6.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCore_Semana6.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Tarea> Tareas => Set<Tarea>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tarea>(entity =>
        {
            entity.ToTable("Tareas");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Titulo).HasMaxLength(100).IsRequired();
            entity.Property(t => t.Descripcion).HasMaxLength(500);
            entity.Property(t => t.Prioridad).HasColumnName("prioridad").HasMaxLength(20).IsRequired();

            entity.HasOne(t => t.Categoria)
                .WithMany(c => c.Tareas)
                .HasForeignKey(t => t.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Responsable)
                .WithMany(u => u.TareasAsignadas)
                .HasForeignKey(t => t.ResponsableId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("Categorias");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Nombre).HasMaxLength(80).IsRequired();
            entity.HasIndex(c => c.Nombre).IsUnique();
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(u => u.Email).HasMaxLength(150).IsRequired();
            entity.HasIndex(u => u.Email).IsUnique();
        });
    }
}
