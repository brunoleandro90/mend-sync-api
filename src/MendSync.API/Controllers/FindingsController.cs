using MendSync.Application.DTOs.Common;
using MendSync.Application.DTOs.Findings;
using MendSync.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MendSync.API.Controllers;

[ApiController]
[Route("api/projects/{projectUuid}/findings")]
public class FindingsController : ControllerBase
{
    private readonly IFindingsService _service;

    public FindingsController(IFindingsService service)
    {
        _service = service;
    }

    // ── SCA ──────────────────────────────────────────────────────────────────

    [HttpGet("security")]
    public async Task<IActionResult> GetSecurityFindings(string projectUuid, [FromQuery] FindingsFilterParams filters)
    {
        var result = await _service.GetProjectSecurityFindingsAsync(projectUuid, filters);
        return Ok(ApiResponse<PagedResultDto<SecurityFindingDto>>.Ok(result));
    }

    [HttpGet("libraries/root")]
    public async Task<IActionResult> GetRootLibraryFindings(string projectUuid)
    {
        var result = await _service.GetRootLibraryFindingsAsync(projectUuid);
        return Ok(ApiResponse<IEnumerable<RootLibraryFindingDto>>.Ok(result));
    }

    [HttpPut("libraries/root/{rootLibraryUuid}")]
    public async Task<IActionResult> UpdateRootLibraryFinding(string projectUuid, string rootLibraryUuid, [FromBody] UpdateFindingDto request)
    {
        await _service.UpdateRootLibraryFindingAsync(projectUuid, rootLibraryUuid, request);
        return NoContent();
    }

    [HttpGet("libraries")]
    public async Task<IActionResult> GetLibraries(string projectUuid)
    {
        var result = await _service.GetProjectLibrariesAsync(projectUuid);
        return Ok(ApiResponse<IEnumerable<LibraryDto>>.Ok(result));
    }

    [HttpGet("licenses")]
    public async Task<IActionResult> GetLicenses(string projectUuid)
    {
        var result = await _service.GetProjectLicensesAsync(projectUuid);
        return Ok(ApiResponse<IEnumerable<LibraryLicenseDto>>.Ok(result));
    }

    // ── SAST ─────────────────────────────────────────────────────────────────

    [HttpGet("sast")]
    public async Task<IActionResult> GetCodeFindings(string projectUuid, [FromQuery] CodeFindingsFilterParams filters)
    {
        var result = await _service.GetProjectCodeFindingsAsync(projectUuid, filters);
        return Ok(ApiResponse<PagedResultDto<CodeFindingDto>>.Ok(result));
    }

    [HttpPut("sast/bulk")]
    public async Task<IActionResult> BulkUpdateCodeFindings(string projectUuid, [FromBody] BulkUpdateCodeFindingsDto request)
    {
        await _service.BulkUpdateCodeFindingsAsync(projectUuid, request);
        return NoContent();
    }

    [HttpPut("sast/{findingSnapshotId}")]
    public async Task<IActionResult> UpdateCodeFinding(string projectUuid, string findingSnapshotId, [FromBody] UpdateCodeFindingDto request)
    {
        await _service.UpdateCodeFindingAsync(projectUuid, findingSnapshotId, request);
        return NoContent();
    }

    [HttpGet("sast/{findingUuid}/details")]
    public async Task<IActionResult> GetCodeFindingDetail(string projectUuid, string findingUuid)
    {
        var result = await _service.GetCodeFindingDetailAsync(projectUuid, findingUuid);
        return Ok(ApiResponse<CodeFindingDetailDto>.Ok(result));
    }

    // ── Containers ───────────────────────────────────────────────────────────

    [HttpGet("images/security")]
    public async Task<IActionResult> GetImageSecurityFindings(string projectUuid)
    {
        var result = await _service.GetImageSecurityFindingsAsync(projectUuid);
        return Ok(ApiResponse<IEnumerable<ImageSecurityFindingDto>>.Ok(result));
    }

    [HttpPut("images/security/bulk")]
    public async Task<IActionResult> UpdateMultipleImageFindings(string projectUuid, [FromBody] UpdateMultipleImageFindingsDto request)
    {
        await _service.UpdateMultipleImageFindingsAsync(projectUuid, request);
        return NoContent();
    }

    [HttpPut("images/security/{findingId}")]
    public async Task<IActionResult> UpdateImageFinding(string projectUuid, string findingId, [FromBody] UpdateImageFindingDto request)
    {
        await _service.UpdateImageFindingAsync(projectUuid, findingId, request);
        return NoContent();
    }

    [HttpGet("images/secrets")]
    public async Task<IActionResult> GetImageSecretFindings(string projectUuid)
    {
        var result = await _service.GetImageSecretFindingsAsync(projectUuid);
        return Ok(ApiResponse<IEnumerable<SecretFindingDto>>.Ok(result));
    }

    [HttpGet("images/packages")]
    public async Task<IActionResult> GetImagePackages(string projectUuid)
    {
        var result = await _service.GetImagePackagesAsync(projectUuid);
        return Ok(ApiResponse<IEnumerable<ImagePackageDto>>.Ok(result));
    }

    // ── AI ───────────────────────────────────────────────────────────────────

    [HttpGet("ai/technologies")]
    public async Task<IActionResult> GetAiTechnologies(string projectUuid)
    {
        var result = await _service.GetAiTechnologiesAsync(projectUuid);
        return Ok(ApiResponse<IEnumerable<AiTechnologyDto>>.Ok(result));
    }

    [HttpGet("ai/models")]
    public async Task<IActionResult> GetAiModels(string projectUuid)
    {
        var result = await _service.GetAiModelsAsync(projectUuid);
        return Ok(ApiResponse<IEnumerable<AiModelDto>>.Ok(result));
    }

    [HttpGet("ai/vulnerabilities")]
    public async Task<IActionResult> GetAiVulnerabilities(string projectUuid)
    {
        var result = await _service.GetAiVulnerabilitiesAsync(projectUuid);
        return Ok(ApiResponse<IEnumerable<AiVulnerabilityDto>>.Ok(result));
    }

    [HttpGet("ai/vulnerabilities/{vulnerabilityId}")]
    public async Task<IActionResult> GetAiVulnerabilityDetail(string projectUuid, string vulnerabilityId)
    {
        var result = await _service.GetAiVulnerabilityDetailAsync(projectUuid, vulnerabilityId);
        return Ok(ApiResponse<AiVulnerabilityDetailDto>.Ok(result));
    }
}
