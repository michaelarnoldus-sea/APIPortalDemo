using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIPortal.Data;
using APIPortal.Models;

namespace APIPortal.Controllers;

[Authorize]
public class DbViewerController : Controller
{
    private readonly AppDbContext _context;

    public DbViewerController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetData()
    {
        var viewModel = new DbViewerViewModel
        {
            Users = await _context.Users
                .Select(u => new UserViewModel
                {
                    Id = u.Id,
                    Username = u.Username,
                    PasswordHashPreview = u.PasswordHash.Substring(0, Math.Min(20, u.PasswordHash.Length)) + "...",
                    SaltPreview = u.Salt.Substring(0, Math.Min(10, u.Salt.Length)) + "...",
                    CreatedAt = u.CreatedAt,
                    ApiKeyCount = u.ApiKeys.Count
                })
                .ToListAsync(),

            ApiKeys = await _context.ApiKeys
                .Include(k => k.User)
                .OrderByDescending(k => k.CreatedAt)
                .Select(k => new ApiKeyViewModel
                {
                    Id = k.Id,
                    UserId = k.UserId,
                    Username = k.User.Username,
                    KeyPrefix = k.KeyPrefix,
                    KeyHashPreview = k.KeyHash.Substring(0, Math.Min(20, k.KeyHash.Length)) + "...",
                    KeyPreview = k.KeyPreview,
                    Scopes = k.Scopes,
                    CreatedAt = k.CreatedAt,
                    ExpiresAt = k.ExpiresAt,
                    RevokedAt = k.RevokedAt,
                    LastUsedAt = k.LastUsedAt,
                    Status = k.Status,
                    IsActive = k.IsActive
                })
                .ToListAsync(),

            Scopes = await _context.Scopes.ToListAsync(),

            DataProtectionKeys = await _context.DataProtectionKeys
                .Select(k => new DataProtectionKeyViewModel
                {
                    Id = k.Id,
                    FriendlyName = k.FriendlyName,
                    XmlPreview = k.Xml != null && k.Xml.Length > 50
                        ? k.Xml.Substring(0, 50) + "... (encrypted)"
                        : k.Xml
                })
                .ToListAsync(),

            LastRefresh = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
        };

        return PartialView("_DataTables", viewModel);
    }
}
