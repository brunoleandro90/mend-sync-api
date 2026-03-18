using MendSync.Application.DTOs.Common;
using MendSync.Application.DTOs.Scans;

namespace MendSync.Application.Interfaces;

public interface IScansService
{
    Task<PagedResultDto<ScanDto>> GetProjectScansAsync(string orgUuid, string projectUuid, PaginationParams pagination);
    Task<ScanDetailDto> GetScanAsync(string orgUuid, string projectUuid, string scanUuid);
    Task<ScanSummaryDto> GetScanSummaryAsync(string orgUuid, string projectUuid, string scanUuid);
    Task<IEnumerable<ScanTagDto>> GetScanTagsAsync(string orgUuid, string projectUuid, string scanUuid);
}
