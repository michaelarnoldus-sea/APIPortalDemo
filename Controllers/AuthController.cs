using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using APIPortal.Services;

namespace APIPortal.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("/login")]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    [HttpPost("/login")]
    public async Task<IActionResult> Login(string username, string password)
    {
        var user = await _authService.ValidateUserAsync(username, password);
        if (user == null)
        {
            ViewBag.Error = "Invalid username or password";
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties
        );

        return RedirectToAction("Index", "Home");
    }

    [HttpPost("/logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}
