namespace MendSync.Application.DTOs.Sync;

public class SyncStatusDto
{
    public bool IsSeeded { get; set; }
    public bool IsSyncing { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public List<SyncEntityStatusDto> Entities { get; set; } = [];
}

public class SyncEntityStatusDto
{
    public string EntityType { get; set; } = string.Empty;
    public int RecordCount { get; set; }
    public DateTime? SyncedAt { get; set; }
    public bool Success { get; set; }
}
