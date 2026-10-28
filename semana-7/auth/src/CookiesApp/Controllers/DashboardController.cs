using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CookiesApp.Controllers;

[Authorize]
public sealed class DashboardController : Controller
{
    [HttpGet("/Dashboard")]
    public IActionResult Index()
    {
        Response.Headers.CacheControl = "no-store";
        return View();
    }
}
