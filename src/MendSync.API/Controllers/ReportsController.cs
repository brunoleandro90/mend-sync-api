using MendSync.Application.DTOs.Common;
using MendSync.Application.DTOs.Reports;
using MendSync.Application.Interfaces;
using MendSync.Infrastructure.Token;
using Microsoft.AspNetCore.Mvc;

namespace MendSync.API.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly IReportsService _service;
    private readonly TokenStore _tokenStore;

    public ReportsController(IReportsService service, TokenStore tokenStore)
    {
        _service = service;
        _tokenStore = tokenStore;
    }

    private string OrgUuid => _tokenStore.GetOrgUuid()
        ?? throw new InvalidOperationException("Not authenticated. Please login first.");

    [HttpGet]
    public async Task<IActionResult> GetReports()
    {
        var result = await _service.GetReportsAsync(OrgUuid);
        return Ok(ApiResponse<IEnumerable<ReportDto>>.Ok(result));
    }

    [HttpGet("{reportUuid}/status")]
    public async Task<IActionResult> GetStatus(string reportUuid)
    {
        var result = await _service.GetReportStatusAsync(OrgUuid, reportUuid);
        return Ok(ApiResponse<ReportStatusDto>.Ok(result));
    }

    [HttpGet("{reportUuid}/download")]
    public async Task<IActionResult> Download(string reportUuid)
    {
        var bytes = await _service.DownloadReportAsync(OrgUuid, reportUuid);
        return File(bytes, "application/octet-stream", $"report-{reportUuid}.zip");
    }

    [HttpDelete("{reportUuid}")]
    public async Task<IActionResult> Delete(string reportUuid)
    {
        await _service.DeleteReportAsync(OrgUuid, reportUuid);
        return NoContent();
    }

    [HttpPost("{scope}/{scopeUuid}/dependency-security")]
    public async Task<IActionResult> ExportDependencySecurity(string scope, string scopeUuid, [FromBody] ReportExportRequestDto request)
    {
        var result = await _service.ExportDependencySecurityFindingsAsync(scope, scopeUuid, request);
        return Ok(ApiResponse<ReportProcessDto>.Ok(result));
    }

    [HttpPost("{scope}/{scopeUuid}/sbom")]
    public async Task<IActionResult> ExportSbom(string scope, string scopeUuid, [FromBody] ReportExportRequestDto request)
    {
        var result = await _service.ExportSbomAsync(scope, scopeUuid, request);
        return Ok(ApiResponse<ReportProcessDto>.Ok(result));
    }

    [HttpPost("{scope}/{scopeUuid}/due-diligence")]
    public async Task<IActionResult> ExportDueDiligence(string scope, string scopeUuid, [FromBody] ReportExportRequestDto request)
    {
        var result = await _service.ExportDueDiligenceAsync(scope, scopeUuid, request);
        return Ok(ApiResponse<ReportProcessDto>.Ok(result));
    }

    [HttpPost("{scope}/{scopeUuid}/sast-findings")]
    public async Task<IActionResult> ExportSastFindings(string scope, string scopeUuid, [FromBody] ReportExportRequestDto request)
    {
        var result = await _service.ExportSastFindingsAsync(scope, scopeUuid, request);
        return Ok(ApiResponse<ReportProcessDto>.Ok(result));
    }

    [HttpPost("{scope}/{scopeUuid}/sast-compliance")]
    public async Task<IActionResult> ExportSastCompliance(string scope, string scopeUuid, [FromBody] ReportExportRequestDto request)
    {
        var result = await _service.ExportSastComplianceAsync(scope, scopeUuid, request);
        return Ok(ApiResponse<ReportProcessDto>.Ok(result));
    }

    /// <summary>Dispara geração e aguarda conclusão (polling automático com timeout de 5 min).</summary>
    [HttpPost("{scope}/{scopeUuid}/dependency-security/wait")]
    public async Task<IActionResult> ExportDependencySecurityAndWait(string scope, string scopeUuid, [FromBody] ReportExportRequestDto request, CancellationToken ct)
    {
        var process = await _service.ExportDependencySecurityFindingsAsync(scope, scopeUuid, request);
        var status = await _service.WaitForReportAsync(OrgUuid, process.ReportUuid, ct);
        return Ok(ApiResponse<ReportStatusDto>.Ok(status));
    }
}
