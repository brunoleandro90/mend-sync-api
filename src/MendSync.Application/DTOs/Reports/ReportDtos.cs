namespace MendSync.Application.DTOs.Reports;

public class ReportDto
{
    public string Uuid { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
    public DateTime? CreatedAt { get; set; }
    public long? SizeBytes { get; set; }
}

public class ReportStatusDto
{
    public string Uuid { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? Progress { get; set; }
    public string? DownloadUrl { get; set; }
}

public class ReportExportRequestDto
{
    public string ReportType { get; set; } = string.Empty;
    public string? Format { get; set; }
    public bool? IncludeSubProjects { get; set; }
}

public class ReportProcessDto
{
    public string ReportUuid { get; set; } = string.Empty;
    public string? Status { get; set; }
}
