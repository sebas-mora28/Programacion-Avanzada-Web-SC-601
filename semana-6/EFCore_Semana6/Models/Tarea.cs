using System.ComponentModel.DataAnnotations;

namespace EFCore_Semana6.Models;

public class Tarea
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Descripcion { get; set; }

    public bool Completada { get; set; }

    public string Prioridad { get; set; } = string.Empty;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;

    public int? ResponsableId { get; set; }
    public Usuario? Responsable { get; set; }
}
