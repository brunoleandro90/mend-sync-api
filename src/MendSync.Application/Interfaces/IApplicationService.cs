using MendSync.Application.DTOs.Applications;
using MendSync.Application.DTOs.Common;
using MendSync.Application.DTOs.Labels;
using MendSync.Application.DTOs.Scans;

namespace MendSync.Application.Interfaces;

public interface IApplicationService
{
    Task<PagedResultDto<ApplicationDto>> GetApplicationsAsync(string orgUuid, PaginationParams pagination);
    Task<IEnumerable<ApplicationSummaryDto>> GetApplicationStatisticsAsync(string orgUuid, ApplicationStatsRequestDto request);
    Task<ApplicationTotalsDto> GetApplicationTotalsAsync(string orgUuid);
    Task<IEnumerable<LabelDto>> GetApplicationLabelsAsync(string orgUuid, string applicationUuid);
    Task AddApplicationLabelAsync(string orgUuid, string applicationUuid, string labelUuid);
    Task RemoveApplicationLabelAsync(string orgUuid, string applicationUuid, string labelUuid);
    Task<IEnumerable<ScanDto>> GetApplicationScansAsync(string orgUuid, string applicationUuid);
    Task UpdateApplicationViolationSlaAsync(string orgUuid, string applicationUuid, UpdateViolationSlaDto request);
}
