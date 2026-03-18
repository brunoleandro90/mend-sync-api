using MendSync.Application.DTOs.Applications;
using MendSync.Application.DTOs.Common;
using MendSync.Application.DTOs.Labels;
using MendSync.Application.DTOs.Projects;
using MendSync.Application.Interfaces;
using MendSync.Infrastructure.HttpClients;
using Microsoft.Extensions.Logging;

namespace MendSync.Infrastructure.Services;

public class ProjectService : IProjectService
{
    private readonly MendApiClient _client;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(MendApiClient client, ILogger<ProjectService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<PagedResultDto<ProjectDto>> GetProjectsAsync(string orgUuid, PaginationParams pagination)
    {
        var query = $"?pageSize={pagination.PageSize}" + (pagination.Cursor != null ? $"&cursor={pagination.Cursor}" : "");
        return await _client.GetAsync<PagedResultDto<ProjectDto>>($"/api/v3.0/orgs/{orgUuid}/projects{query}")
            ?? new PagedResultDto<ProjectDto>();
    }

    public async Task<IEnumerable<ProjectSummaryDto>> GetProjectStatisticsAsync(string orgUuid, ProjectStatsRequestDto request)
    {
        var result = await _client.PostAsync<ProjectStatsRequestDto, IEnumerable<ProjectSummaryDto>>(
            $"/api/v3.0/orgs/{orgUuid}/projects/summaries", request);
        return result ?? [];
    }

    public async Task<ProjectTotalsDto> GetProjectTotalsAsync(string orgUuid)
    {
        return await _client.GetAsync<ProjectTotalsDto>($"/api/v3.0/orgs/{orgUuid}/projects/summaries/totals")
            ?? new ProjectTotalsDto();
    }

    public async Task<ProjectTotalsByDateDto> GetProjectTotalsByDateAsync(string orgUuid)
    {
        return await _client.GetAsync<ProjectTotalsByDateDto>($"/api/v3.0/orgs/{orgUuid}/projects/summaries/total/date")
            ?? new ProjectTotalsByDateDto();
    }

    public async Task<IEnumerable<LabelDto>> GetProjectLabelsAsync(string orgUuid, string projectUuid)
    {
        return await _client.GetAsync<IEnumerable<LabelDto>>($"/api/v3.0/orgs/{orgUuid}/projects/{projectUuid}/labels")
            ?? [];
    }

    public async Task AddProjectLabelAsync(string orgUuid, string projectUuid, string labelUuid)
    {
        await _client.PutRawAsync($"/api/v3.0/orgs/{orgUuid}/projects/{projectUuid}/labels", new { labelUuid });
    }

    public async Task RemoveProjectLabelAsync(string orgUuid, string projectUuid, string labelUuid)
    {
        await _client.DeleteAsync($"/api/v3.0/orgs/{orgUuid}/projects/{projectUuid}/labels/{labelUuid}");
    }

    public async Task<IEnumerable<ViolationDto>> GetProjectViolationsAsync(string orgUuid, string projectUuid)
    {
        return await _client.GetAsync<IEnumerable<ViolationDto>>($"/api/v3.0/orgs/{orgUuid}/projects/{projectUuid}/violations")
            ?? [];
    }

    public async Task UpdateProjectViolationSlaAsync(string orgUuid, string projectUuid, UpdateViolationSlaDto request)
    {
        await _client.PutRawAsync($"/api/v3.0/orgs/{orgUuid}/projects/{projectUuid}/violations/sla", request);
    }

    public async Task<IEnumerable<EffectiveVulnerabilityDto>> GetProjectEffectiveVulnerabilitiesAsync(string projectUuid)
    {
        return await _client.GetAsync<IEnumerable<EffectiveVulnerabilityDto>>($"/api/v3.0/projects/{projectUuid}/dependencies/effective")
            ?? [];
    }
}
