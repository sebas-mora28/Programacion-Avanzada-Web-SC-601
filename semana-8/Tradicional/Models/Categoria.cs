using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Tradicional.Models;

[Table("Categorias")]
[Index(nameof(Nombre), IsUnique = true)]
public class Categoria
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string Nombre { get; set; } = string.Empty;

    // Relación 1:N: una categoría puede tener cero, uno o muchos productos.
    // Esta navegación es el lado inverso de Producto.Categoria.
    // La FK se guarda en Productos.CategoriaId, no como una lista en la tabla Categorias.
    public List<Producto> Productos { get; set; } = [];
}
