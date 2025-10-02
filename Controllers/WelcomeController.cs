using Microsoft.AspNetCore.Mvc;

namespace APIPortal.Controllers;

public class WelcomeController : Controller
{
    public IActionResult Index()
    {
        // Pass authentication status to view
        ViewBag.IsAuthenticated = User.Identity?.IsAuthenticated ?? false;
        ViewBag.Username = User.Identity?.Name;

        return View();
    }
}
