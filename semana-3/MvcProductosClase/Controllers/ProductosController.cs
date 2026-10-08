using Microsoft.AspNetCore.Mvc;
using MvcProductosClase.Models;
using MvcProductosClase.Services;

namespace MvcProductosClase.Controllers;

public class ProductosController(ProductoRepository repositorio) : Controller
{
    // GET /Productos
    public IActionResult Index() => View(repositorio.Todos());

    // GET /Productos/Detalle/1
    public IActionResult Detalle(int id)
    {
        var producto = repositorio.Buscar(id);
        return producto is null ? NotFound() : View(producto);
    }

    // GET /Productos/Crear
    public IActionResult Crear() => View(new Producto());

    // POST /Productos/Crear
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear([Bind("Nombre,Precio")] Producto producto)
    {
        if (!ModelState.IsValid) return View(producto);
        repositorio.Crear(producto);
        return RedirectToAction(nameof(Index));
    }

    // GET /Productos/Editar/1
    public IActionResult Editar(int id)
    {
        var producto = repositorio.Buscar(id);
        return producto is null ? NotFound() : View(producto);
    }

    // POST /Productos/Editar/1
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(int id, [Bind("Nombre,Precio")] Producto producto)
    {
        if (repositorio.Buscar(id) is null) return NotFound();
        if (!ModelState.IsValid) return View(producto);
        repositorio.Actualizar(id, producto);
        return RedirectToAction(nameof(Detalle), new { id });
    }

    // POST /Productos/Eliminar/1
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Eliminar(int id)
    {
        if (!repositorio.Eliminar(id)) return NotFound();
        return RedirectToAction(nameof(Index));
    }
}
