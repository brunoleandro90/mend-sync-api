namespace MendSync.Infrastructure.Data.Entities;

public class MendLabel
{
    public string Uuid { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
    public DateTime SyncedAt { get; set; }
}
