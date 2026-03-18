namespace MendSync.Infrastructure.Data.Entities;

public class MendUser
{
    public string Uuid { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Role { get; set; }
    public bool? IsBlocked { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime SyncedAt { get; set; }
}
