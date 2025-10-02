using APIPortal.Models;
using APIPortal.Services;

namespace APIPortal.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context, IAuthService authService)
    {
        // Seed Scopes
        if (!context.Scopes.Any())
        {
            var scopes = new List<Scope>
            {
                new Scope { Name = "CP Data API", Description = "Access to CP Data API endpoints" },
                new Scope { Name = "Carbon API", Description = "Access to Carbon API endpoints" }
            };

            context.Scopes.AddRange(scopes);
            await context.SaveChangesAsync();
        }

        // Seed Users
        if (!context.Users.Any())
        {
            var users = new List<(string username, string password)>
            {
                ("admin", "admin123"),
                ("john", "john123"),
                ("sarah", "sarah123"),
                ("demo", "demo123")
            };

            foreach (var (username, password) in users)
            {
                var (hash, salt) = authService.HashPassword(password);
                var user = new User
                {
                    Username = username,
                    PasswordHash = hash,
                    Salt = salt,
                    CreatedAt = DateTime.UtcNow
                };
                context.Users.Add(user);
            }

            await context.SaveChangesAsync();
        }
    }
}
