namespace APIPortal.Models;

public class ApiKey
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string KeyPrefix { get; set; } = string.Empty;
    public string KeyHash { get; set; } = string.Empty;
    public string KeyPreview { get; set; } = string.Empty; // Last 4 characters for display
    public string Scopes { get; set; } = "[]"; // JSON array of scopes
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public DateTime? LastUsedAt { get; set; }

    // Navigation property
    public User User { get; set; } = null!;

    // Computed properties
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;

    public string Status
    {
        get
        {
            if (IsRevoked) return "Revoked";
            if (IsExpired) return "Expired";
            return "Active";
        }
    }
}
