using MendSync.Application.DTOs.Labels;

namespace MendSync.Application.DTOs.Applications;

public class ApplicationDto
{
    public string Uuid { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public IEnumerable<LabelDto> Labels { get; set; } = [];
}

public class ApplicationSummaryDto
{
    public string Uuid { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int? CriticalVulnerabilities { get; set; }
    public int? HighVulnerabilities { get; set; }
    public int? MediumVulnerabilities { get; set; }
    public int? LowVulnerabilities { get; set; }
    public int? ProjectCount { get; set; }
}

public class ApplicationStatsRequestDto
{
    public IEnumerable<string>? ApplicationUuids { get; set; }
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public int PageSize { get; set; } = 25;
    public string? Cursor { get; set; }
}

public class ApplicationTotalsDto
{
    public int Total { get; set; }
    public int? WithVulnerabilities { get; set; }
    public int? WithPolicyViolations { get; set; }
}

public class UpdateViolationSlaDto
{
    public string SlaLevel { get; set; } = string.Empty;
    public int? Days { get; set; }
}
