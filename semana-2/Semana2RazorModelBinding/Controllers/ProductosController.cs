using Microsoft.AspNetCore.Mvc;
using Semana2RazorModelBinding.Models;

namespace Semana2RazorModelBinding.Controllers;

public class ProductosController : Controller
{
    // Almacenamiento en memoria para mantener el ejemplo simple.
    private static readonly List<Producto> Productos =
    [
        new Producto
        {
            Id = 1,
            Nombre = "Teclado mecánico",
            Precio = 85.50m,
            FechaCreacion = DateTime.Now
        },
        new Producto
        {
            Id = 2,
            Nombre = "Monitor 27 pulgadas",
            Precio = 325.00m,
            FechaCreacion = DateTime.Now
        }
    ];

    public IActionResult Index()
    {
        return View(Productos);
    }

    public IActionResult Details(int id)
    {
        var producto = Productos.FirstOrDefault(p => p.Id == id);

        if (producto is null)
        {
            return NotFound();
        }

        return View(producto);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("Nombre,Precio")] Producto producto)
    {
        // Model Binding ya creó el objeto Producto con los datos del formulario.
        // Las Data Annotations y la validación personalizada ya fueron ejecutadas.
        if (!ModelState.IsValid)
        {
            return View(producto);
        }

        // Id y FechaCreacion NO se toman del cliente.
        // Se asignan en el servidor porque tienen BindNever.
        producto.Id = Productos.Count == 0
            ? 1
            : Productos.Max(p => p.Id) + 1;
        producto.FechaCreacion = DateTime.Now;

        Productos.Add(producto);

        return RedirectToAction(nameof(Details), new { id = producto.Id });
    }
}
