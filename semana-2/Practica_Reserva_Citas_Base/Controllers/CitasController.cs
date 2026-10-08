using Microsoft.AspNetCore.Mvc;
using PracticaReservaCitasBase.Models;

namespace PracticaReservaCitasBase.Controllers;

public class CitasController : Controller
{
    [HttpGet]
    public IActionResult Crear()
    {
        return View(new Cita());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(Cita cita)
    {
        // TODO 1: Verificar ModelState.IsValid.
        // Si el modelo no es valido, regresar a la vista Crear mostrando los errores.

        // TODO 2: Cuando la cita sea valida, asignar desde el servidor:
        // - Id
        // - Estado = "Pendiente"

        // TODO 3: Mostrar la vista Confirmacion con la cita registrada.

        return View(cita);
    }
}
