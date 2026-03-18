using MendSync.Application.DTOs.Reports;

namespace MendSync.Application.Interfaces;

public interface IReportsService
{
    Task<ReportStatusDto> GetReportStatusAsync(string orgUuid, string reportUuid);
    Task<IEnumerable<ReportDto>> GetReportsAsync(string orgUuid);
    Task<byte[]> DownloadReportAsync(string orgUuid, string reportUuid);
    Task DeleteReportAsync(string orgUuid, string reportUuid);

    Task<ReportProcessDto> ExportDependencySecurityFindingsAsync(string scope, string scopeUuid, ReportExportRequestDto request);
    Task<ReportProcessDto> ExportSbomAsync(string scope, string scopeUuid, ReportExportRequestDto request);
    Task<ReportProcessDto> ExportDueDiligenceAsync(string scope, string scopeUuid, ReportExportRequestDto request);
    Task<ReportProcessDto> ExportSastFindingsAsync(string scope, string scopeUuid, ReportExportRequestDto request);
    Task<ReportProcessDto> ExportSastComplianceAsync(string scope, string scopeUuid, ReportExportRequestDto request);

    Task<ReportStatusDto> WaitForReportAsync(string orgUuid, string reportUuid, CancellationToken ct = default);
}
