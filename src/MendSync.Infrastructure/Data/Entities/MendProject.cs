namespace MendSync.Infrastructure.Data.Entities;

public class MendProject
{
    public string Uuid { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ApplicationUuid { get; set; }
    public string? ApplicationName { get; set; }
    public DateTime? LastScanDate { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime SyncedAt { get; set; }
}
