using APIPortal.Models;

namespace APIPortal.Services;

public interface IApiKeyService
{
    (string fullKey, ApiKey apiKey) GenerateApiKey(int userId, string prefix, string environment, List<string> scopes);
    string HashKey(string key);
    Task<ApiKey?> GetKeyByIdAsync(int keyId, int userId);
    Task<List<ApiKey>> GetUserKeysAsync(int userId);
    Task RevokeKeyAsync(int keyId, int userId);
}
