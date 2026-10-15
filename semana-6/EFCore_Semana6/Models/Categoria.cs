using System.ComponentModel.DataAnnotations;

namespace EFCore_Semana6.Models;

public class Categoria
{
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string Nombre { get; set; } = string.Empty;


    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public ICollection<Tarea> Tareas { get; set; } = [];
}
