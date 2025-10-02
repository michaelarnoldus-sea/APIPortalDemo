using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using APIPortal.Services;
using System.Security.Claims;

namespace APIPortal.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IApiKeyService _apiKeyService;

    public HomeController(IApiKeyService apiKeyService)
    {
        _apiKeyService = apiKeyService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = GetCurrentUserId();
        var keys = await _apiKeyService.GetUserKeysAsync(userId);
        return View(keys);
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }
}
