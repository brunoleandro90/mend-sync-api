using MendSync.Application.DTOs.Common;
using MendSync.Application.DTOs.Scans;
using MendSync.Application.Interfaces;
using MendSync.Infrastructure.HttpClients;
using Microsoft.Extensions.Logging;

namespace MendSync.Infrastructure.Services;

public class ScansService : IScansService
{
    private readonly MendApiClient _client;
    private readonly ILogger<ScansService> _logger;

    public ScansService(MendApiClient client, ILogger<ScansService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<PagedResultDto<ScanDto>> GetProjectScansAsync(string orgUuid, string projectUuid, PaginationParams pagination)
    {
        var query = $"?pageSize={pagination.PageSize}" + (pagination.Cursor != null ? $"&cursor={pagination.Cursor}" : "");
        return await _client.GetAsync<PagedResultDto<ScanDto>>($"/api/v3.0/orgs/{orgUuid}/projects/{projectUuid}/scans{query}")
            ?? new PagedResultDto<ScanDto>();
    }

    public async Task<ScanDetailDto> GetScanAsync(string orgUuid, string projectUuid, string scanUuid)
    {
        return await _client.GetAsync<ScanDetailDto>($"/api/v3.0/orgs/{orgUuid}/projects/{projectUuid}/scans/{scanUuid}")
            ?? new ScanDetailDto();
    }

    public async Task<ScanSummaryDto> GetScanSummaryAsync(string orgUuid, string projectUuid, string scanUuid)
    {
        return await _client.GetAsync<ScanSummaryDto>($"/api/v3.0/orgs/{orgUuid}/projects/{projectUuid}/scans/{scanUuid}/summary")
            ?? new ScanSummaryDto();
    }

    public async Task<IEnumerable<ScanTagDto>> GetScanTagsAsync(string orgUuid, string projectUuid, string scanUuid)
    {
        return await _client.GetAsync<IEnumerable<ScanTagDto>>($"/api/v3.0/orgs/{orgUuid}/projects/{projectUuid}/scans/{scanUuid}/tags")
            ?? [];
    }
}
