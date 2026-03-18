using MendSync.Application.DTOs.Applications;
using MendSync.Application.DTOs.Common;
using MendSync.Application.DTOs.Labels;
using MendSync.Application.DTOs.Scans;
using MendSync.Application.Interfaces;
using MendSync.Infrastructure.HttpClients;
using Microsoft.Extensions.Logging;

namespace MendSync.Infrastructure.Services;

public class ApplicationService : IApplicationService
{
    private readonly MendApiClient _client;
    private readonly ILogger<ApplicationService> _logger;

    public ApplicationService(MendApiClient client, ILogger<ApplicationService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<PagedResultDto<ApplicationDto>> GetApplicationsAsync(string orgUuid, PaginationParams pagination)
    {
        var query = $"?limit={pagination.PageSize}" + (pagination.Cursor != null ? $"&cursor={pagination.Cursor}" : "");
        return await _client.GetAsync<PagedResultDto<ApplicationDto>>($"/api/v3.0/orgs/{orgUuid}/applications{query}")
            ?? new PagedResultDto<ApplicationDto>();
    }

    public async Task<IEnumerable<ApplicationSummaryDto>> GetApplicationStatisticsAsync(string orgUuid, ApplicationStatsRequestDto request)
    {
        var result = await _client.PostAsync<ApplicationStatsRequestDto, IEnumerable<ApplicationSummaryDto>>(
            $"/api/v3.0/orgs/{orgUuid}/applications/summaries", request);
        return result ?? [];
    }

    public async Task<ApplicationTotalsDto> GetApplicationTotalsAsync(string orgUuid)
    {
        return await _client.GetAsync<ApplicationTotalsDto>($"/api/v3.0/orgs/{orgUuid}/applications/summaries/totals")
            ?? new ApplicationTotalsDto();
    }

    public async Task<IEnumerable<LabelDto>> GetApplicationLabelsAsync(string orgUuid, string applicationUuid)
    {
        return await _client.GetAsync<IEnumerable<LabelDto>>($"/api/v3.0/orgs/{orgUuid}/applications/{applicationUuid}/labels")
            ?? [];
    }

    public async Task AddApplicationLabelAsync(string orgUuid, string applicationUuid, string labelUuid)
    {
        await _client.PutRawAsync($"/api/v3.0/orgs/{orgUuid}/applications/{applicationUuid}/labels",
            new { labelUuid });
    }

    public async Task RemoveApplicationLabelAsync(string orgUuid, string applicationUuid, string labelUuid)
    {
        await _client.DeleteAsync($"/api/v3.0/orgs/{orgUuid}/applications/{applicationUuid}/labels/{labelUuid}");
    }

    public async Task<IEnumerable<ScanDto>> GetApplicationScansAsync(string orgUuid, string applicationUuid)
    {
        return await _client.GetAsync<IEnumerable<ScanDto>>($"/api/v3.0/orgs/{orgUuid}/applications/{applicationUuid}/scans")
            ?? [];
    }

    public async Task UpdateApplicationViolationSlaAsync(string orgUuid, string applicationUuid, UpdateViolationSlaDto request)
    {
        await _client.PutRawAsync($"/api/v3.0/orgs/{orgUuid}/applications/{applicationUuid}/violations/sla", request);
    }
}
