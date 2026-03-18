using MendSync.Application.DTOs.Applications;
using MendSync.Application.DTOs.Common;
using MendSync.Application.DTOs.Labels;
using MendSync.Application.DTOs.Projects;
using MendSync.Application.Interfaces;
using MendSync.Infrastructure.Token;
using Microsoft.AspNetCore.Mvc;

namespace MendSync.API.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _service;
    private readonly TokenStore _tokenStore;

    public ProjectsController(IProjectService service, TokenStore tokenStore)
    {
        _service = service;
        _tokenStore = tokenStore;
    }

    private string OrgUuid => _tokenStore.GetOrgUuid()
        ?? throw new InvalidOperationException("Not authenticated. Please login first.");

    [HttpGet]
    public async Task<IActionResult> GetProjects([FromQuery] PaginationParams pagination)
    {
        var result = await _service.GetProjectsAsync(OrgUuid, pagination);
        return Ok(ApiResponse<PagedResultDto<ProjectDto>>.Ok(result));
    }

    [HttpPost("summaries")]
    public async Task<IActionResult> GetStatistics([FromBody] ProjectStatsRequestDto request)
    {
        var result = await _service.GetProjectStatisticsAsync(OrgUuid, request);
        return Ok(ApiResponse<IEnumerable<ProjectSummaryDto>>.Ok(result));
    }

    [HttpGet("summaries/totals")]
    public async Task<IActionResult> GetTotals()
    {
        var result = await _service.GetProjectTotalsAsync(OrgUuid);
        return Ok(ApiResponse<ProjectTotalsDto>.Ok(result));
    }

    [HttpGet("summaries/total/date")]
    public async Task<IActionResult> GetTotalsByDate()
    {
        var result = await _service.GetProjectTotalsByDateAsync(OrgUuid);
        return Ok(ApiResponse<ProjectTotalsByDateDto>.Ok(result));
    }

    [HttpGet("{projectUuid}/labels")]
    public async Task<IActionResult> GetLabels(string projectUuid)
    {
        var result = await _service.GetProjectLabelsAsync(OrgUuid, projectUuid);
        return Ok(ApiResponse<IEnumerable<LabelDto>>.Ok(result));
    }

    [HttpPut("{projectUuid}/labels/{labelUuid}")]
    public async Task<IActionResult> AddLabel(string projectUuid, string labelUuid)
    {
        await _service.AddProjectLabelAsync(OrgUuid, projectUuid, labelUuid);
        return NoContent();
    }

    [HttpDelete("{projectUuid}/labels/{labelUuid}")]
    public async Task<IActionResult> RemoveLabel(string projectUuid, string labelUuid)
    {
        await _service.RemoveProjectLabelAsync(OrgUuid, projectUuid, labelUuid);
        return NoContent();
    }

    [HttpGet("{projectUuid}/violations")]
    public async Task<IActionResult> GetViolations(string projectUuid)
    {
        var result = await _service.GetProjectViolationsAsync(OrgUuid, projectUuid);
        return Ok(ApiResponse<IEnumerable<ViolationDto>>.Ok(result));
    }

    [HttpPut("{projectUuid}/violations/sla")]
    public async Task<IActionResult> UpdateViolationSla(string projectUuid, [FromBody] UpdateViolationSlaDto request)
    {
        await _service.UpdateProjectViolationSlaAsync(OrgUuid, projectUuid, request);
        return NoContent();
    }

    [HttpGet("{projectUuid}/dependencies/effective")]
    public async Task<IActionResult> GetEffectiveVulnerabilities(string projectUuid)
    {
        var result = await _service.GetProjectEffectiveVulnerabilitiesAsync(projectUuid);
        return Ok(ApiResponse<IEnumerable<EffectiveVulnerabilityDto>>.Ok(result));
    }
}
