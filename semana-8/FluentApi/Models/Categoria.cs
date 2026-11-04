namespace FluentApi.Models;

public class Categoria
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    // Relación 1:N: una categoría puede tener cero, uno o muchos productos.
    // Esta navegación es el lado inverso de Producto.Categoria.
    // La FK se guarda en Productos.CategoriaId, no como una lista en la tabla Categorias.
    public List<Producto> Productos { get; set; } = [];
}
