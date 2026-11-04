using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Tradicional.Models;

[Table("Etiquetas")]
[Index(nameof(Nombre), IsUnique = true)]
public class Etiqueta
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Nombre { get; set; } = string.Empty;

    // Lado inverso de Producto.Etiquetas: una etiqueta puede identificar muchos productos.
    // InverseProperty indica qué navegación del otro modelo forma pareja con esta.
    // Las dos colecciones permiten a EF reconocer la relación M:N por convención;
    // el atributo por sí solo no define una tabla intermedia ni sus claves.
    // EF crea EtiquetaProducto con EtiquetasId y ProductosId como clave compuesta.
    [InverseProperty(nameof(Producto.Etiquetas))]
    public List<Producto> Productos { get; set; } = [];
}
