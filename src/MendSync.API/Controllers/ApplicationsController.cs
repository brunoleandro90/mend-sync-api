using MendSync.Application.DTOs.Applications;
using MendSync.Application.DTOs.Common;
using MendSync.Application.DTOs.Labels;
using MendSync.Application.Interfaces;
using MendSync.Infrastructure.Token;
using Microsoft.AspNetCore.Mvc;

namespace MendSync.API.Controllers;

[ApiController]
[Route("api/applications")]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _service;
    private readonly TokenStore _tokenStore;

    public ApplicationsController(IApplicationService service, TokenStore tokenStore)
    {
        _service = service;
        _tokenStore = tokenStore;
    }

    private string OrgUuid => _tokenStore.GetOrgUuid()
        ?? throw new InvalidOperationException("Not authenticated. Please login first.");

    [HttpGet]
    public async Task<IActionResult> GetApplications([FromQuery] PaginationParams pagination)
    {
        var result = await _service.GetApplicationsAsync(OrgUuid, pagination);
        return Ok(ApiResponse<PagedResultDto<ApplicationDto>>.Ok(result));
    }

    [HttpPost("summaries")]
    public async Task<IActionResult> GetStatistics([FromBody] ApplicationStatsRequestDto request)
    {
        var result = await _service.GetApplicationStatisticsAsync(OrgUuid, request);
        return Ok(ApiResponse<IEnumerable<ApplicationSummaryDto>>.Ok(result));
    }

    [HttpGet("summaries/totals")]
    public async Task<IActionResult> GetTotals()
    {
        var result = await _service.GetApplicationTotalsAsync(OrgUuid);
        return Ok(ApiResponse<ApplicationTotalsDto>.Ok(result));
    }

    [HttpGet("{applicationUuid}/labels")]
    public async Task<IActionResult> GetLabels(string applicationUuid)
    {
        var result = await _service.GetApplicationLabelsAsync(OrgUuid, applicationUuid);
        return Ok(ApiResponse<IEnumerable<LabelDto>>.Ok(result));
    }

    [HttpPut("{applicationUuid}/labels/{labelUuid}")]
    public async Task<IActionResult> AddLabel(string applicationUuid, string labelUuid)
    {
        await _service.AddApplicationLabelAsync(OrgUuid, applicationUuid, labelUuid);
        return NoContent();
    }

    [HttpDelete("{applicationUuid}/labels/{labelUuid}")]
    public async Task<IActionResult> RemoveLabel(string applicationUuid, string labelUuid)
    {
        await _service.RemoveApplicationLabelAsync(OrgUuid, applicationUuid, labelUuid);
        return NoContent();
    }

    [HttpGet("{applicationUuid}/scans")]
    public async Task<IActionResult> GetScans(string applicationUuid)
    {
        var result = await _service.GetApplicationScansAsync(OrgUuid, applicationUuid);
        return Ok(ApiResponse<IEnumerable<object>>.Ok(result));
    }

    [HttpPut("{applicationUuid}/violations/sla")]
    public async Task<IActionResult> UpdateViolationSla(string applicationUuid, [FromBody] UpdateViolationSlaDto request)
    {
        await _service.UpdateApplicationViolationSlaAsync(OrgUuid, applicationUuid, request);
        return NoContent();
    }
}
