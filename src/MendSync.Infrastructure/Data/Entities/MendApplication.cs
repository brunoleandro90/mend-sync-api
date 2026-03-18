namespace MendSync.Infrastructure.Data.Entities;

public class MendApplication
{
    public string Uuid { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime SyncedAt { get; set; }
}
