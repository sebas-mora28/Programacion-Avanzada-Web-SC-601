using System.ComponentModel.DataAnnotations;

namespace EFCore_Semana6.Models;

public class TareaRequest
{
    [Required, MaxLength(100)]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Descripcion { get; set; }

    public bool Completada { get; set; }

    [Required, MaxLength(20)]
    public string Prioridad { get; set; } = "Media";

    [Range(1, int.MaxValue)]
    public int CategoriaId { get; set; }

    [Range(1, int.MaxValue)]
    public int? ResponsableId { get; set; }
}
