namespace MendSync.Infrastructure.Data.Entities;

public class MendGroup
{
    public string Uuid { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? MemberCount { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime SyncedAt { get; set; }
}
