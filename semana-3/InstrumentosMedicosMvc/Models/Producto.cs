namespace InstrumentosMedicosMvc.Models;

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Categoria { get; set; } = "";
    public decimal Precio { get; set; }
    public int Existencias { get; set; }
    public string Descripcion { get; set; } = "";

    // Para esta demostración los datos viven en el Modelo, sin base de datos.
    private static readonly List<Producto> Catalogo =
    [
        new() { Id = 1, Nombre = "Estetoscopio", Categoria = "Diagnóstico", Precio = 72m, Existencias = 8, Descripcion = "Estetoscopio para consulta general." },
        new() { Id = 2, Nombre = "Oxímetro", Categoria = "Monitoreo", Precio = 45m, Existencias = 3, Descripcion = "Oxímetro de pulso portátil." },
        new() { Id = 3, Nombre = "Termómetro digital", Categoria = "Diagnóstico", Precio = 18m, Existencias = 0, Descripcion = "Termómetro de lectura rápida." },
        new() { Id = 4, Nombre = "Guantes de examen", Categoria = "Insumos", Precio = 12m, Existencias = 20, Descripcion = "Caja de guantes de examen." }
    ];

    public static IReadOnlyList<Producto> ObtenerTodos() => Catalogo;
    public static Producto? ObtenerPorId(int id) => Catalogo.FirstOrDefault(p => p.Id == id);
}
