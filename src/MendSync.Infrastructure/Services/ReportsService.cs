using MendSync.Application.DTOs.Reports;
using MendSync.Application.Interfaces;
using MendSync.Infrastructure.HttpClients;
using Microsoft.Extensions.Logging;

namespace MendSync.Infrastructure.Services;

public class ReportsService : IReportsService
{
    private readonly MendApiClient _client;
    private readonly ILogger<ReportsService> _logger;

    public ReportsService(MendApiClient client, ILogger<ReportsService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<ReportStatusDto> GetReportStatusAsync(string orgUuid, string reportUuid)
    {
        return await _client.GetAsync<ReportStatusDto>($"/api/v3.0/orgs/{orgUuid}/reports/{reportUuid}/status")
            ?? new ReportStatusDto { Uuid = reportUuid, Status = "UNKNOWN" };
    }

    public async Task<IEnumerable<ReportDto>> GetReportsAsync(string orgUuid)
    {
        return await _client.GetAsync<IEnumerable<ReportDto>>($"/api/v3.0/orgs/{orgUuid}/reports")
            ?? [];
    }

    public async Task<byte[]> DownloadReportAsync(string orgUuid, string reportUuid)
    {
        return await _client.GetBytesAsync($"/api/v3.0/orgs/{orgUuid}/reports/{reportUuid}/download");
    }

    public async Task DeleteReportAsync(string orgUuid, string reportUuid)
    {
        await _client.DeleteAsync($"/api/v3.0/orgs/{orgUuid}/reports/{reportUuid}");
    }

    public async Task<ReportProcessDto> ExportDependencySecurityFindingsAsync(string scope, string scopeUuid, ReportExportRequestDto request)
    {
        return await _client.PostAsync<ReportExportRequestDto, ReportProcessDto>(
            $"/api/v3.0/{scope}s/{scopeUuid}/reports/dependency-security", request)
            ?? new ReportProcessDto();
    }

    public async Task<ReportProcessDto> ExportSbomAsync(string scope, string scopeUuid, ReportExportRequestDto request)
    {
        return await _client.PostAsync<ReportExportRequestDto, ReportProcessDto>(
            $"/api/v3.0/{scope}s/{scopeUuid}/reports/sbom", request)
            ?? new ReportProcessDto();
    }

    public async Task<ReportProcessDto> ExportDueDiligenceAsync(string scope, string scopeUuid, ReportExportRequestDto request)
    {
        return await _client.PostAsync<ReportExportRequestDto, ReportProcessDto>(
            $"/api/v3.0/{scope}s/{scopeUuid}/reports/due-diligence", request)
            ?? new ReportProcessDto();
    }

    public async Task<ReportProcessDto> ExportSastFindingsAsync(string scope, string scopeUuid, ReportExportRequestDto request)
    {
        return await _client.PostAsync<ReportExportRequestDto, ReportProcessDto>(
            $"/api/v3.0/{scope}s/{scopeUuid}/reports/sast-findings", request)
            ?? new ReportProcessDto();
    }

    public async Task<ReportProcessDto> ExportSastComplianceAsync(string scope, string scopeUuid, ReportExportRequestDto request)
    {
        return await _client.PostAsync<ReportExportRequestDto, ReportProcessDto>(
            $"/api/v3.0/{scope}s/{scopeUuid}/reports/sast-compliance", request)
            ?? new ReportProcessDto();
    }

    public async Task<ReportStatusDto> WaitForReportAsync(string orgUuid, string reportUuid, CancellationToken ct = default)
    {
        var timeout = TimeSpan.FromMinutes(5);
        var interval = TimeSpan.FromSeconds(5);
        var deadline = DateTime.UtcNow.Add(timeout);

        while (DateTime.UtcNow < deadline)
        {
            var status = await GetReportStatusAsync(orgUuid, reportUuid);
            _logger.LogInformation("Report {ReportUuid} status: {Status}", reportUuid, status.Status);

            if (status.Status == "COMPLETED")
                return status;

            if (status.Status == "FAILED" || status.Status == "ERROR")
                throw new InvalidOperationException($"Report generation failed with status: {status.Status}");

            await Task.Delay(interval, ct);
        }

        throw new TimeoutException($"Report {reportUuid} did not complete within 5 minutes.");
    }
}
