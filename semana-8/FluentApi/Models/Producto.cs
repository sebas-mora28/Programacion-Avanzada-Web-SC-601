namespace FluentApi.Models;

// Entidad sin atributos de persistencia: el mapeo vive en AppDbContext.
public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    
    // FK de la relación 1:N: identifica la categoría a la que pertenece este producto.
    // Al ser int, la relación es obligatoria: cada producto tiene una categoría.
    public int CategoriaId { get; set; }

    // Navegación de referencia: permite acceder al objeto Categoria, además de su ID.
    public Categoria Categoria { get; set; } = null!;

    // Relación M:N: un producto puede tener varias etiquetas (por ejemplo, Oferta y USB).
    // La misma etiqueta puede aparecer en otros productos; no es exclusiva de uno.
    public List<Etiqueta> Etiquetas { get; set; } = [];

    public string Etiqueta => $"{Nombre} - {Precio:N2}";
}
