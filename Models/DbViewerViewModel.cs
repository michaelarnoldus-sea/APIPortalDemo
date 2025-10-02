namespace APIPortal.Models;

public class DbViewerViewModel
{
    public List<UserViewModel> Users { get; set; } = new();
    public List<ApiKeyViewModel> ApiKeys { get; set; } = new();
    public List<Scope> Scopes { get; set; } = new();
    public List<DataProtectionKeyViewModel> DataProtectionKeys { get; set; } = new();
    public string LastRefresh { get; set; } = string.Empty;
}

public class UserViewModel
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHashPreview { get; set; } = string.Empty;
    public string SaltPreview { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int ApiKeyCount { get; set; }
}

public class ApiKeyViewModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string KeyPrefix { get; set; } = string.Empty;
    public string KeyHashPreview { get; set; } = string.Empty;
    public string KeyPreview { get; set; } = string.Empty;
    public string Scopes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class DataProtectionKeyViewModel
{
    public int Id { get; set; }
    public string? FriendlyName { get; set; }
    public string? XmlPreview { get; set; }
}
