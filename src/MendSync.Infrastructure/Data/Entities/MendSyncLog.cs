namespace MendSync.Infrastructure.Data.Entities;

public class MendSyncLog
{
    public int Id { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public DateTime SyncedAt { get; set; }
    public int RecordCount { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
