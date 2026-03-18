using MendSync.Application.DTOs.Common;
using MendSync.Application.DTOs.Scans;
using MendSync.Application.Interfaces;
using MendSync.Infrastructure.Token;
using Microsoft.AspNetCore.Mvc;

namespace MendSync.API.Controllers;

[ApiController]
[Route("api/projects/{projectUuid}/scans")]
public class ScansController : ControllerBase
{
    private readonly IScansService _service;
    private readonly TokenStore _tokenStore;

    public ScansController(IScansService service, TokenStore tokenStore)
    {
        _service = service;
        _tokenStore = tokenStore;
    }

    private string OrgUuid => _tokenStore.GetOrgUuid()
        ?? throw new InvalidOperationException("Not authenticated. Please login first.");

    [HttpGet]
    public async Task<IActionResult> GetScans(string projectUuid, [FromQuery] PaginationParams pagination)
    {
        var result = await _service.GetProjectScansAsync(OrgUuid, projectUuid, pagination);
        return Ok(ApiResponse<PagedResultDto<ScanDto>>.Ok(result));
    }

    [HttpGet("{scanUuid}")]
    public async Task<IActionResult> GetScan(string projectUuid, string scanUuid)
    {
        var result = await _service.GetScanAsync(OrgUuid, projectUuid, scanUuid);
        return Ok(ApiResponse<ScanDetailDto>.Ok(result));
    }

    [HttpGet("{scanUuid}/summary")]
    public async Task<IActionResult> GetScanSummary(string projectUuid, string scanUuid)
    {
        var result = await _service.GetScanSummaryAsync(OrgUuid, projectUuid, scanUuid);
        return Ok(ApiResponse<ScanSummaryDto>.Ok(result));
    }

    [HttpGet("{scanUuid}/tags")]
    public async Task<IActionResult> GetScanTags(string projectUuid, string scanUuid)
    {
        var result = await _service.GetScanTagsAsync(OrgUuid, projectUuid, scanUuid);
        return Ok(ApiResponse<IEnumerable<ScanTagDto>>.Ok(result));
    }
}
