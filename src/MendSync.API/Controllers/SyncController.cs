using MendSync.Application.DTOs.Sync;
using MendSync.Application.Interfaces;
using MendSync.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace MendSync.API.Controllers;

[ApiController]
[Route("api/sync")]
public class SyncController(ISyncService syncService, SyncState syncState) : ControllerBase
{
    /// <summary>Triggers a full base data sync from Mend.io into Oracle.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult TriggerSync()
    {
        if (syncState.IsSyncing)
            return Conflict(new { message = "Sync already in progress." });

        _ = Task.Run(async () =>
        {
            if (!await syncState.TryBeginSyncAsync())
                return;
            try
            {
                await syncService.SyncAllAsync();
                syncState.EndSync(success: true);
            }
            catch
            {
                syncState.EndSync(success: false);
                throw;
            }
        });

        return Accepted(new { message = "Sync started in background." });
    }

    /// <summary>Returns the current sync status and last sync details per entity type.</summary>
    [HttpGet("status")]
    [ProducesResponseType(typeof(SyncStatusDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SyncStatusDto>> GetStatus(CancellationToken ct)
    {
        var status = await syncService.GetStatusAsync(ct);
        status.IsSyncing = syncState.IsSyncing;
        return Ok(status);
    }
}
