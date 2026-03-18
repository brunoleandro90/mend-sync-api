namespace MendSync.Application.DTOs.Scans;

public class ScanDto
{
    public string Uuid { get; set; } = string.Empty;
    public string? Status { get; set; }
    public string? Type { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public string? ProjectUuid { get; set; }
}

public class ScanDetailDto : ScanDto
{
    public string? AgentVersion { get; set; }
    public string? ScannedFiles { get; set; }
    public int? LibrariesFound { get; set; }
    public int? VulnerabilitiesFound { get; set; }
}

public class ScanSummaryDto
{
    public string ScanUuid { get; set; } = string.Empty;
    public int? TotalLibraries { get; set; }
    public int? VulnerableLibraries { get; set; }
    public int? CriticalVulnerabilities { get; set; }
    public int? HighVulnerabilities { get; set; }
    public int? MediumVulnerabilities { get; set; }
    public int? LowVulnerabilities { get; set; }
}

public class ScanTagDto
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
