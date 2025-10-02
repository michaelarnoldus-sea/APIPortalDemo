using APIPortal.Models;

namespace APIPortal.Services;

public interface IAuthService
{
    Task<User?> ValidateUserAsync(string username, string password);
    (string hash, string salt) HashPassword(string password);
    bool VerifyPassword(string password, string hash, string salt);
}
