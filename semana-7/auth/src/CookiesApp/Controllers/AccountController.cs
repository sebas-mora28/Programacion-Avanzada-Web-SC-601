using AuthLab;
using CookiesApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace CookiesApp.Controllers;

public sealed class AccountController(UserService users) : Controller
{
    [HttpGet("/")]
    public IActionResult Index() => RedirectToAction(nameof(Login));

    [HttpGet("/Login")]
    public IActionResult Login(string? returnUrl) => View(new LoginViewModel { ReturnUrl = returnUrl });

    [HttpPost("/Login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = users.ValidateCredentials(model.Username, model.Password);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
            return View(model);
        }

        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        }, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        return Url.IsLocalUrl(model.ReturnUrl)
            ? LocalRedirect(model.ReturnUrl!)
            : Redirect("/Dashboard");
    }

    [HttpGet("/Logout")]
    public IActionResult Logout() => View(); // GET no modifica la sesión.

    [HttpPost("/Logout")]
    [ActionName(nameof(Logout))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogoutPost()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }
}
