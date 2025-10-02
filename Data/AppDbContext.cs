using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using APIPortal.Models;

namespace APIPortal.Data;

public class AppDbContext : DbContext, IDataProtectionKeyContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }
    public DbSet<Scope> Scopes { get; set; }
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Salt).IsRequired();
        });

        // ApiKey configuration
        modelBuilder.Entity<ApiKey>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.KeyPrefix).IsRequired().HasMaxLength(50);
            entity.Property(e => e.KeyHash).IsRequired();
            entity.Property(e => e.KeyPreview).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Scopes).IsRequired();
            entity.HasIndex(e => e.KeyHash);

            entity.HasOne(e => e.User)
                .WithMany(u => u.ApiKeys)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Scope configuration
        modelBuilder.Entity<Scope>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Name).IsUnique();
        });
    }
}
