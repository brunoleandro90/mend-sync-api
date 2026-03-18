using MendSync.Application.DTOs.Applications;
using MendSync.Application.DTOs.Common;
using MendSync.Application.DTOs.Labels;
using MendSync.Application.DTOs.Projects;

namespace MendSync.Application.Interfaces;

public interface IProjectService
{
    Task<PagedResultDto<ProjectDto>> GetProjectsAsync(string orgUuid, PaginationParams pagination);
    Task<IEnumerable<ProjectSummaryDto>> GetProjectStatisticsAsync(string orgUuid, ProjectStatsRequestDto request);
    Task<ProjectTotalsDto> GetProjectTotalsAsync(string orgUuid);
    Task<ProjectTotalsByDateDto> GetProjectTotalsByDateAsync(string orgUuid);
    Task<IEnumerable<LabelDto>> GetProjectLabelsAsync(string orgUuid, string projectUuid);
    Task AddProjectLabelAsync(string orgUuid, string projectUuid, string labelUuid);
    Task RemoveProjectLabelAsync(string orgUuid, string projectUuid, string labelUuid);
    Task<IEnumerable<ViolationDto>> GetProjectViolationsAsync(string orgUuid, string projectUuid);
    Task UpdateProjectViolationSlaAsync(string orgUuid, string projectUuid, UpdateViolationSlaDto request);
    Task<IEnumerable<EffectiveVulnerabilityDto>> GetProjectEffectiveVulnerabilitiesAsync(string projectUuid);
}
