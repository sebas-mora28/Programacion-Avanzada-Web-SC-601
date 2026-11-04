using System.ComponentModel.DataAnnotations;

namespace FluentApi.Models;

public class Etiqueta
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    // Lado inverso de Producto.Etiquetas: permite consultar los productos de esta etiqueta.
    // HasMany(...).WithMany(...) conecta ambas colecciones en AppDbContext.
    // La tabla intermedia almacena las asociaciones; esta entidad no necesita ProductoId.
    public List<Producto> Productos { get; set; } = [];
}
