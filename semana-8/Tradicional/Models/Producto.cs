using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Tradicional.Models;

// El mapeo está junto a las propiedades mediante atributos.
[Table("Productos")]
public class Producto
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    [Column("nombre_producto")]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Descripcion { get; set; }

    [Precision(10, 2)]
    public decimal Precio { get; set; }

    // FK de la relación 1:N: identifica la categoría a la que pertenece este producto.
    // Al ser int (no int?), la relación es obligatoria: cada producto tiene una categoría.
    public int CategoriaId { get; set; }

    // Vincula la navegación Categoria con la propiedad que almacena su FK.
    // null! solo silencia la advertencia de C#: no carga ni crea una categoría.
    [ForeignKey(nameof(CategoriaId))]
    public Categoria Categoria { get; set; } = null!;

    // Relación M:N: un producto puede tener varias etiquetas (por ejemplo, Oferta y USB).
    // La misma etiqueta puede aparecer en otros productos; no es exclusiva de uno.
    // EF guarda cada asociación en EtiquetaProducto, no en una columna EtiquetaId.
    // La colección se carga explícitamente con Include o mediante una proyección LINQ.
    public List<Etiqueta> Etiquetas { get; set; } = [];

    [NotMapped]
    public string Etiqueta => $"{Nombre} - {Precio:N2}";
}
