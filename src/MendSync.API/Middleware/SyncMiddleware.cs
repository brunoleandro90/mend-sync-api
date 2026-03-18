using MendSync.Application.Interfaces;
using MendSync.Infrastructure.Data;
using MendSync.Infrastructure.Token;

namespace MendSync.API.Middleware;

/// <summary>
/// On the first authenticated non-auth request, triggers base data sync in the background
/// if the Oracle tables are not yet populated.
/// </summary>
public class SyncMiddleware(RequestDelegate next, ILogger<SyncMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context,
        SyncState syncState,
        TokenStore tokenStore,
        IServiceScopeFactory scopeFactory)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        var shouldSkip = path.StartsWith("/api/auth", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/api/sync", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/health", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase);

        if (!shouldSkip && !syncState.IsSeeded && tokenStore.IsJwtValid())
        {
            _ = Task.Run(async () =>
            {
                if (!await syncState.TryBeginSyncAsync())
                    return; // another sync already running

                try
                {
                    using var scope = scopeFactory.CreateScope();
                    var syncService = scope.ServiceProvider.GetRequiredService<ISyncService>();

                    if (!await syncService.IsSeededAsync())
                    {
                        logger.LogInformation("Auto-sync triggered on first request");
                        await syncService.SyncAllAsync();
                        syncState.EndSync(success: true);
                        logger.LogInformation("Auto-sync completed");
                    }
                    else
                    {
                        syncState.EndSync(success: true); // already seeded by a previous run
                    }
                }
                catch (Exception ex)
                {
                    syncState.EndSync(success: false);
                    logger.LogError(ex, "Auto-sync failed");
                }
            });
        }

        await next(context);
    }
}
