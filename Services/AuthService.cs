using System.Security.Cryptography;
using System.Text;
using APIPortal.Data;
using APIPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace APIPortal.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private const int Iterations = 10000;
    private const int KeySize = 32;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> ValidateUserAsync(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
            return null;

        if (!VerifyPassword(password, user.PasswordHash, user.Salt))
            return null;

        return user;
    }

    public (string hash, string salt) HashPassword(string password)
    {
        // Generate a random salt
        var saltBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }

        var salt = Convert.ToBase64String(saltBytes);

        // Hash the password with the salt
        var hash = HashPasswordWithSalt(password, salt);

        return (hash, salt);
    }

    public bool VerifyPassword(string password, string hash, string salt)
    {
        var computedHash = HashPasswordWithSalt(password, salt);
        return computedHash == hash;
    }

    private string HashPasswordWithSalt(string password, string salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            Convert.FromBase64String(salt),
            Iterations,
            HashAlgorithmName.SHA256
        );

        var hashBytes = pbkdf2.GetBytes(KeySize);
        return Convert.ToBase64String(hashBytes);
    }
}
