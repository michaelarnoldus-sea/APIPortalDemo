using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using APIPortal.Data;
using APIPortal.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Data Protection (persist keys to database)
// Keys are stored in the database so they survive container restarts
// NOTE: In production, add certificate-based encryption (.ProtectKeysWithCertificate)
// or use Azure Key Vault / AWS KMS for additional security
builder.Services.AddDataProtection()
    .SetApplicationName("APIPortal")
    .PersistKeysToDbContext<AppDbContext>();

// Configure Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;
    });

// Register services
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Migrate and seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var authService = services.GetRequiredService<IAuthService>();

        // Ensure database is created
        context.Database.EnsureCreated();

        // Seed data
        await DbSeeder.SeedAsync(context, authService);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Welcome}/{action=Index}/{id?}");

app.Run();
