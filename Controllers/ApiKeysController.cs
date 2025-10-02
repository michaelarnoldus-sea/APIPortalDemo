using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using APIPortal.Data;
using APIPortal.Services;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace APIPortal.Controllers;

[Authorize]
public class ApiKeysController : Controller
{
    private readonly IApiKeyService _apiKeyService;
    private readonly AppDbContext _context;

    public ApiKeysController(IApiKeyService apiKeyService, AppDbContext context)
    {
        _apiKeyService = apiKeyService;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> CreateForm()
    {
        var scopes = await _context.Scopes.ToListAsync();
        ViewBag.Scopes = scopes;
        return PartialView("_CreateForm");
    }

    [HttpPost]
    public async Task<IActionResult> Create(string prefix, string environment, List<string> scopes)
    {
        var userId = GetCurrentUserId();

        // Generate the API key
        var (fullKey, apiKey) = _apiKeyService.GenerateApiKey(userId, prefix, environment, scopes ?? new List<string>());

        // Save to database
        _context.ApiKeys.Add(apiKey);
        await _context.SaveChangesAsync();

        // Return the full key (shown only once)
        ViewBag.FullKey = fullKey;
        ViewBag.ApiKey = apiKey;
        return PartialView("_KeyCreated");
    }

    [HttpPost]
    public async Task<IActionResult> Revoke(int id)
    {
        var userId = GetCurrentUserId();
        await _apiKeyService.RevokeKeyAsync(id, userId);

        var apiKey = await _apiKeyService.GetKeyByIdAsync(id, userId);
        return PartialView("_KeyRow", apiKey);
    }

    [HttpGet]
    public IActionResult List()
    {
        return RedirectToAction("Index", "Home");
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }
}
