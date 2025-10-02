using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using APIPortal.Data;
using APIPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace APIPortal.Services;

public class ApiKeyService : IApiKeyService
{
    private readonly AppDbContext _context;
    private const int KeyLength = 32;
    private const string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public ApiKeyService(AppDbContext context)
    {
        _context = context;
    }

    public (string fullKey, ApiKey apiKey) GenerateApiKey(int userId, string prefix, string environment, List<string> scopes)
    {
        // Generate cryptographically secure random string
        var randomPart = GenerateSecureRandomString(KeyLength);

        // Create Stripe-like format: prefix_environment_randomstring
        var fullKey = $"{prefix}_{environment}_{randomPart}";

        // Get preview (last 4 characters)
        var preview = randomPart.Substring(randomPart.Length - 4);

        // Hash the key
        var keyHash = HashKey(fullKey);

        // Create ApiKey entity
        var apiKey = new ApiKey
        {
            UserId = userId,
            KeyPrefix = $"{prefix}_{environment}",
            KeyHash = keyHash,
            KeyPreview = preview,
            Scopes = JsonSerializer.Serialize(scopes),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(30) // 30 day expiration
        };

        return (fullKey, apiKey);
    }

    public string HashKey(string key)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
        return Convert.ToBase64String(hashBytes);
    }

    public async Task<ApiKey?> GetKeyByIdAsync(int keyId, int userId)
    {
        return await _context.ApiKeys
            .FirstOrDefaultAsync(k => k.Id == keyId && k.UserId == userId);
    }

    public async Task<List<ApiKey>> GetUserKeysAsync(int userId)
    {
        return await _context.ApiKeys
            .Where(k => k.UserId == userId)
            .OrderByDescending(k => k.CreatedAt)
            .ToListAsync();
    }

    public async Task RevokeKeyAsync(int keyId, int userId)
    {
        var apiKey = await GetKeyByIdAsync(keyId, userId);
        if (apiKey != null && !apiKey.IsRevoked)
        {
            apiKey.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    private string GenerateSecureRandomString(int length)
    {
        var result = new char[length];
        var randomBytes = new byte[length];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        for (int i = 0; i < length; i++)
        {
            result[i] = AllowedChars[randomBytes[i] % AllowedChars.Length];
        }

        return new string(result);
    }
}
