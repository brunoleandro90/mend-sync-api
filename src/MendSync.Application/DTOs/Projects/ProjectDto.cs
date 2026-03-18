using MendSync.Application.DTOs.Labels;

namespace MendSync.Application.DTOs.Projects;

public class ProjectDto
{
    public string Uuid { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ApplicationUuid { get; set; }
    public string? ApplicationName { get; set; }
    public DateTime? LastScanDate { get; set; }
    public DateTime? CreatedAt { get; set; }
    public IEnumerable<LabelDto> Labels { get; set; } = [];
}

public class ProjectSummaryDto
{
    public string Uuid { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int? CriticalVulnerabilities { get; set; }
    public int? HighVulnerabilities { get; set; }
    public int? MediumVulnerabilities { get; set; }
    public int? LowVulnerabilities { get; set; }
    public DateTime? LastScanDate { get; set; }
}

public class ProjectStatsRequestDto
{
    public IEnumerable<string>? ProjectUuids { get; set; }
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public int PageSize { get; set; } = 25;
    public string? Cursor { get; set; }
}

public class ProjectTotalsDto
{
    public int Total { get; set; }
    public int? WithVulnerabilities { get; set; }
    public int? OutdatedLibraries { get; set; }
}

public class ProjectTotalsByDateDto
{
    public IEnumerable<DateTotalEntry> Entries { get; set; } = [];
}

public class DateTotalEntry
{
    public string Date { get; set; } = string.Empty;
    public int Total { get; set; }
}

public class ViolationDto
{
    public string Uuid { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string? LibraryName { get; set; }
    public string? PolicyName { get; set; }
    public DateTime? DetectedAt { get; set; }
}

public class EffectiveVulnerabilityDto
{
    public string Uuid { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double? Score { get; set; }
    public string? Severity { get; set; }
    public string? LibraryName { get; set; }
    public string? FixedVersion { get; set; }
}
