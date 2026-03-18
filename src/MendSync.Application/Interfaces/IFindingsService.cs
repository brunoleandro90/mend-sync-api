using MendSync.Application.DTOs.Common;
using MendSync.Application.DTOs.Findings;

namespace MendSync.Application.Interfaces;

public interface IFindingsService
{
    // SCA
    Task<PagedResultDto<SecurityFindingDto>> GetProjectSecurityFindingsAsync(string projectUuid, FindingsFilterParams filters);
    Task<IEnumerable<RootLibraryFindingDto>> GetRootLibraryFindingsAsync(string projectUuid);
    Task UpdateRootLibraryFindingAsync(string projectUuid, string rootLibraryUuid, UpdateFindingDto request);
    Task<IEnumerable<LibraryDto>> GetProjectLibrariesAsync(string projectUuid);
    Task<IEnumerable<LibraryLicenseDto>> GetProjectLicensesAsync(string projectUuid);

    // SAST
    Task<PagedResultDto<CodeFindingDto>> GetProjectCodeFindingsAsync(string projectUuid, CodeFindingsFilterParams filters);
    Task BulkUpdateCodeFindingsAsync(string projectUuid, BulkUpdateCodeFindingsDto request);
    Task UpdateCodeFindingAsync(string projectUuid, string findingSnapshotId, UpdateCodeFindingDto request);
    Task<CodeFindingDetailDto> GetCodeFindingDetailAsync(string projectUuid, string findingUuid);

    // Containers
    Task<IEnumerable<ImageSecurityFindingDto>> GetImageSecurityFindingsAsync(string projectUuid);
    Task UpdateMultipleImageFindingsAsync(string projectUuid, UpdateMultipleImageFindingsDto request);
    Task UpdateImageFindingAsync(string projectUuid, string findingId, UpdateImageFindingDto request);
    Task<IEnumerable<SecretFindingDto>> GetImageSecretFindingsAsync(string projectUuid);
    Task<IEnumerable<ImagePackageDto>> GetImagePackagesAsync(string projectUuid);

    // AI
    Task<IEnumerable<AiTechnologyDto>> GetAiTechnologiesAsync(string projectUuid);
    Task<IEnumerable<AiModelDto>> GetAiModelsAsync(string projectUuid);
    Task<IEnumerable<AiVulnerabilityDto>> GetAiVulnerabilitiesAsync(string projectUuid);
    Task<AiVulnerabilityDetailDto> GetAiVulnerabilityDetailAsync(string projectUuid, string vulnerabilityId);
}
