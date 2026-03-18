namespace MendSync.Application.DTOs.Findings;

// ── SCA (Dependencies) ──────────────────────────────────────────────────────

public class SecurityFindingDto
{
    public string Uuid { get; set; } = string.Empty;
    public string? CveName { get; set; }
    public double? Score { get; set; }
    public string? Severity { get; set; }
    public string? LibraryName { get; set; }
    public string? LibraryVersion { get; set; }
    public string? FixedVersion { get; set; }
    public string? Status { get; set; }
    public DateTime? DetectedAt { get; set; }
}

public class FindingsFilterParams
{
    public int PageSize { get; set; } = 25;
    public string? Cursor { get; set; }
    public string? Severity { get; set; }
    public string? Status { get; set; }
}

public class RootLibraryFindingDto
{
    public string Uuid { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Version { get; set; }
    public string? Licenses { get; set; }
    public string? Status { get; set; }
    public int? VulnerabilityCount { get; set; }
}

public class UpdateFindingDto
{
    public string? Status { get; set; }
    public string? Comment { get; set; }
}

public class LibraryDto
{
    public string Uuid { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Version { get; set; }
    public string? Type { get; set; }
    public IEnumerable<string> Licenses { get; set; } = [];
}

public class LibraryLicenseDto
{
    public string LibraryUuid { get; set; } = string.Empty;
    public string LibraryName { get; set; } = string.Empty;
    public string LicenseName { get; set; } = string.Empty;
    public string? LicenseType { get; set; }
}

// ── SAST (Code) ──────────────────────────────────────────────────────────────

public class CodeFindingDto
{
    public string FindingSnapshotId { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Severity { get; set; }
    public string? Status { get; set; }
    public string? Category { get; set; }
    public string? FilePath { get; set; }
    public int? LineNumber { get; set; }
    public DateTime? DetectedAt { get; set; }
}

public class CodeFindingsFilterParams
{
    public int PageSize { get; set; } = 25;
    public string? Cursor { get; set; }
    public string? Severity { get; set; }
    public string? Status { get; set; }
    public string? Category { get; set; }
}

public class BulkUpdateCodeFindingsDto
{
    public IEnumerable<string> FindingSnapshotIds { get; set; } = [];
    public string Status { get; set; } = string.Empty;
    public string? Comment { get; set; }
}

public class UpdateCodeFindingDto
{
    public string? Status { get; set; }
    public string? Comment { get; set; }
    public string? Severity { get; set; }
}

public class CodeFindingDetailDto : CodeFindingDto
{
    public string? Description { get; set; }
    public string? Remediation { get; set; }
    public string? CodeSnippet { get; set; }
    public IEnumerable<string> References { get; set; } = [];
}

// ── Containers (Images) ───────────────────────────────────────────────────────

public class ImageSecurityFindingDto
{
    public string FindingId { get; set; } = string.Empty;
    public string? CveName { get; set; }
    public double? Score { get; set; }
    public string? Severity { get; set; }
    public string? PackageName { get; set; }
    public string? PackageVersion { get; set; }
    public string? FixedVersion { get; set; }
    public string? Status { get; set; }
}

public class UpdateMultipleImageFindingsDto
{
    public IEnumerable<string> FindingIds { get; set; } = [];
    public string Status { get; set; } = string.Empty;
    public string? Comment { get; set; }
}

public class UpdateImageFindingDto
{
    public string? Status { get; set; }
    public string? Comment { get; set; }
}

public class SecretFindingDto
{
    public string FindingId { get; set; } = string.Empty;
    public string? SecretType { get; set; }
    public string? FilePath { get; set; }
    public int? LineNumber { get; set; }
    public string? Status { get; set; }
}

public class ImagePackageDto
{
    public string Name { get; set; } = string.Empty;
    public string? Version { get; set; }
    public string? License { get; set; }
    public string? PackageManager { get; set; }
}

// ── AI ────────────────────────────────────────────────────────────────────────

public class AiTechnologyDto
{
    public string Name { get; set; } = string.Empty;
    public string? Version { get; set; }
    public string? Category { get; set; }
}

public class AiModelDto
{
    public string ModelId { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Provider { get; set; }
    public string? RiskLevel { get; set; }
}

public class AiVulnerabilityDto
{
    public string VulnerabilityId { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Severity { get; set; }
    public string? AffectedComponent { get; set; }
}

public class AiVulnerabilityDetailDto : AiVulnerabilityDto
{
    public string? Description { get; set; }
    public string? Remediation { get; set; }
    public IEnumerable<string> References { get; set; } = [];
}
