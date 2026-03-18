using MendSync.Application.DTOs.Sync;

namespace MendSync.Application.Interfaces;

public interface ISyncService
{
    Task SyncAllAsync(CancellationToken ct = default);
    Task<bool> IsSeededAsync(CancellationToken ct = default);
    Task<SyncStatusDto> GetStatusAsync(CancellationToken ct = default);
}
