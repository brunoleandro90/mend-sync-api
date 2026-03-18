using MendSync.Application.DTOs.Common;
using MendSync.Application.DTOs.Findings;
using MendSync.Application.Interfaces;
using MendSync.Infrastructure.HttpClients;
using Microsoft.Extensions.Logging;

namespace MendSync.Infrastructure.Services;

public class FindingsService : IFindingsService
{
    private readonly MendApiClient _client;
    private readonly ILogger<FindingsService> _logger;

    public FindingsService(MendApiClient client, ILogger<FindingsService> logger)
    {
        _client = client;
        _logger = logger;
    }

    // ── SCA ──────────────────────────────────────────────────────────────────

    public async Task<PagedResultDto<SecurityFindingDto>> GetProjectSecurityFindingsAsync(string projectUuid, FindingsFilterParams filters)
    {
        var query = $"?pageSize={filters.PageSize}"
            + (filters.Cursor != null ? $"&cursor={filters.Cursor}" : "")
            + (filters.Severity != null ? $"&severity={filters.Severity}" : "")
            + (filters.Status != null ? $"&status={filters.Status}" : "");
        return await _client.GetAsync<PagedResultDto<SecurityFindingDto>>($"/api/v3.0/projects/{projectUuid}/findings/security{query}")
            ?? new PagedResultDto<SecurityFindingDto>();
    }

    public async Task<IEnumerable<RootLibraryFindingDto>> GetRootLibraryFindingsAsync(string projectUuid)
    {
        return await _client.GetAsync<IEnumerable<RootLibraryFindingDto>>($"/api/v3.0/projects/{projectUuid}/findings/libraries/root")
            ?? [];
    }

    public async Task UpdateRootLibraryFindingAsync(string projectUuid, string rootLibraryUuid, UpdateFindingDto request)
    {
        await _client.PutRawAsync($"/api/v3.0/projects/{projectUuid}/findings/libraries/root/{rootLibraryUuid}", request);
    }

    public async Task<IEnumerable<LibraryDto>> GetProjectLibrariesAsync(string projectUuid)
    {
        return await _client.GetAsync<IEnumerable<LibraryDto>>($"/api/v3.0/projects/{projectUuid}/libraries")
            ?? [];
    }

    public async Task<IEnumerable<LibraryLicenseDto>> GetProjectLicensesAsync(string projectUuid)
    {
        return await _client.GetAsync<IEnumerable<LibraryLicenseDto>>($"/api/v3.0/projects/{projectUuid}/licenses")
            ?? [];
    }

    // ── SAST ─────────────────────────────────────────────────────────────────

    public async Task<PagedResultDto<CodeFindingDto>> GetProjectCodeFindingsAsync(string projectUuid, CodeFindingsFilterParams filters)
    {
        var query = $"?pageSize={filters.PageSize}"
            + (filters.Cursor != null ? $"&cursor={filters.Cursor}" : "")
            + (filters.Severity != null ? $"&severity={filters.Severity}" : "")
            + (filters.Status != null ? $"&status={filters.Status}" : "")
            + (filters.Category != null ? $"&category={filters.Category}" : "");
        return await _client.GetAsync<PagedResultDto<CodeFindingDto>>($"/api/v3.0/projects/{projectUuid}/findings/sast{query}")
            ?? new PagedResultDto<CodeFindingDto>();
    }

    public async Task BulkUpdateCodeFindingsAsync(string projectUuid, BulkUpdateCodeFindingsDto request)
    {
        await _client.PutRawAsync($"/api/v3.0/projects/{projectUuid}/findings/sast/bulk", request);
    }

    public async Task UpdateCodeFindingAsync(string projectUuid, string findingSnapshotId, UpdateCodeFindingDto request)
    {
        await _client.PutRawAsync($"/api/v3.0/projects/{projectUuid}/findings/sast/{findingSnapshotId}", request);
    }

    public async Task<CodeFindingDetailDto> GetCodeFindingDetailAsync(string projectUuid, string findingUuid)
    {
        return await _client.GetAsync<CodeFindingDetailDto>($"/api/v3.0/projects/{projectUuid}/findings/sast/{findingUuid}/details")
            ?? new CodeFindingDetailDto();
    }

    // ── Containers ───────────────────────────────────────────────────────────

    public async Task<IEnumerable<ImageSecurityFindingDto>> GetImageSecurityFindingsAsync(string projectUuid)
    {
        return await _client.GetAsync<IEnumerable<ImageSecurityFindingDto>>($"/api/v3.0/projects/{projectUuid}/findings/images/security")
            ?? [];
    }

    public async Task UpdateMultipleImageFindingsAsync(string projectUuid, UpdateMultipleImageFindingsDto request)
    {
        await _client.PutRawAsync($"/api/v3.0/projects/{projectUuid}/findings/images/security/bulk", request);
    }

    public async Task UpdateImageFindingAsync(string projectUuid, string findingId, UpdateImageFindingDto request)
    {
        await _client.PutRawAsync($"/api/v3.0/projects/{projectUuid}/findings/images/security/{findingId}", request);
    }

    public async Task<IEnumerable<SecretFindingDto>> GetImageSecretFindingsAsync(string projectUuid)
    {
        return await _client.GetAsync<IEnumerable<SecretFindingDto>>($"/api/v3.0/projects/{projectUuid}/findings/images/secrets")
            ?? [];
    }

    public async Task<IEnumerable<ImagePackageDto>> GetImagePackagesAsync(string projectUuid)
    {
        return await _client.GetAsync<IEnumerable<ImagePackageDto>>($"/api/v3.0/projects/{projectUuid}/findings/images/packages")
            ?? [];
    }

    // ── AI ───────────────────────────────────────────────────────────────────

    public async Task<IEnumerable<AiTechnologyDto>> GetAiTechnologiesAsync(string projectUuid)
    {
        return await _client.GetAsync<IEnumerable<AiTechnologyDto>>($"/api/v3.0/projects/{projectUuid}/ai/technologies")
            ?? [];
    }

    public async Task<IEnumerable<AiModelDto>> GetAiModelsAsync(string projectUuid)
    {
        return await _client.GetAsync<IEnumerable<AiModelDto>>($"/api/v3.0/projects/{projectUuid}/ai/models")
            ?? [];
    }

    public async Task<IEnumerable<AiVulnerabilityDto>> GetAiVulnerabilitiesAsync(string projectUuid)
    {
        return await _client.GetAsync<IEnumerable<AiVulnerabilityDto>>($"/api/v3.0/projects/{projectUuid}/ai/vulnerabilities")
            ?? [];
    }

    public async Task<AiVulnerabilityDetailDto> GetAiVulnerabilityDetailAsync(string projectUuid, string vulnerabilityId)
    {
        return await _client.GetAsync<AiVulnerabilityDetailDto>($"/api/v3.0/projects/{projectUuid}/ai/vulnerabilities/{vulnerabilityId}")
            ?? new AiVulnerabilityDetailDto();
    }
}
