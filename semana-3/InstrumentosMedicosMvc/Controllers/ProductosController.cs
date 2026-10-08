using InstrumentosMedicosMvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentosMedicosMvc.Controllers;

public class ProductosController : Controller
{
    public IActionResult Index() => View(Producto.ObtenerTodos());

    public IActionResult Detalle(int id)
    {
        var producto = Producto.ObtenerPorId(id);
        return producto is null ? NotFound() : View(producto);
    }
}
