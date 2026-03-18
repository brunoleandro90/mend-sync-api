using MendSync.Application.DTOs.Applications;
using MendSync.Application.DTOs.Common;
using MendSync.Application.DTOs.Groups;
using MendSync.Application.DTOs.Labels;
using MendSync.Application.DTOs.Projects;
using MendSync.Application.DTOs.Sync;
using MendSync.Application.DTOs.Users;
using MendSync.Application.Interfaces;
using MendSync.Infrastructure.Data;
using MendSync.Infrastructure.Data.Entities;
using MendSync.Infrastructure.HttpClients;
using MendSync.Infrastructure.Token;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MendSync.Infrastructure.Services;

public class SyncService(
    MendApiClient client,
    MendSyncDbContext db,
    TokenStore tokenStore,
    IConfiguration config,
    ILogger<SyncService> logger) : ISyncService
{
    private string OrgUuid => tokenStore.GetOrgUuid()
        ?? config["Mend:OrgUuid"]
        ?? throw new InvalidOperationException("OrgUuid not available. Please login first.");

    public async Task<bool> IsSeededAsync(CancellationToken ct = default) =>
        await db.Applications.AnyAsync(ct) ||
        await db.Projects.AnyAsync(ct) ||
        await db.Labels.AnyAsync(ct);

    public async Task<SyncStatusDto> GetStatusAsync(CancellationToken ct = default)
    {
        var logs = await db.SyncLogs
            .OrderByDescending(x => x.SyncedAt)
            .ToListAsync(ct);

        var lastSync = logs.FirstOrDefault()?.SyncedAt;

        var entities = logs
            .GroupBy(x => x.EntityType)
            .Select(g =>
            {
                var latest = g.OrderByDescending(x => x.SyncedAt).First();
                return new SyncEntityStatusDto
                {
                    EntityType = latest.EntityType,
                    RecordCount = latest.RecordCount,
                    SyncedAt = latest.SyncedAt,
                    Success = latest.Success
                };
            })
            .ToList();

        return new SyncStatusDto
        {
            IsSeeded = await IsSeededAsync(ct),
            LastSyncedAt = lastSync,
            Entities = entities
        };
    }

    public async Task SyncAllAsync(CancellationToken ct = default)
    {
        var orgUuid = OrgUuid;
        logger.LogInformation("Starting base data sync for org {OrgUuid}", orgUuid);

        await SyncEntityAsync(orgUuid, ct,
            fetch: () => FetchAllPagesAsync<ApplicationDto>(
                $"/api/v3.0/orgs/{orgUuid}/applications", ct),
            map: items => items.Select(x => new MendApplication
            {
                Uuid = x.Uuid,
                Name = x.Name,
                Description = x.Description,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                SyncedAt = DateTime.UtcNow
            }),
            dbSet: db.Applications,
            entityType: "Applications");

        await SyncEntityAsync(orgUuid, ct,
            fetch: () => FetchAllPagesAsync<ProjectDto>(
                $"/api/v3.0/orgs/{orgUuid}/projects", ct),
            map: items => items.Select(x => new MendProject
            {
                Uuid = x.Uuid,
                Name = x.Name,
                ApplicationUuid = x.ApplicationUuid,
                ApplicationName = x.ApplicationName,
                LastScanDate = x.LastScanDate,
                CreatedAt = x.CreatedAt,
                SyncedAt = DateTime.UtcNow
            }),
            dbSet: db.Projects,
            entityType: "Projects");

        await SyncEntityAsync(orgUuid, ct,
            fetch: () => FetchListAsync<LabelDto>(
                $"/api/v3.0/orgs/{orgUuid}/labels", ct),
            map: items => items.Select(x => new MendLabel
            {
                Uuid = x.Uuid,
                Name = x.Name,
                Color = x.Color,
                SyncedAt = DateTime.UtcNow
            }),
            dbSet: db.Labels,
            entityType: "Labels");

        await SyncEntityAsync(orgUuid, ct,
            fetch: () => FetchAllPagesAsync<UserDto>(
                $"/api/v3.0/orgs/{orgUuid}/users", ct),
            map: items => items.Select(x => new MendUser
            {
                Uuid = x.Uuid,
                Email = x.Email,
                Name = x.Name,
                Role = x.Role,
                IsBlocked = x.IsBlocked,
                CreatedAt = x.CreatedAt,
                LastLoginAt = x.LastLoginAt,
                SyncedAt = DateTime.UtcNow
            }),
            dbSet: db.Users,
            entityType: "Users");

        await SyncEntityAsync(orgUuid, ct,
            fetch: () => FetchListAsync<GroupDto>(
                $"/api/v3.0/orgs/{orgUuid}/groups", ct),
            map: items => items.Select(x => new MendGroup
            {
                Uuid = x.Uuid,
                Name = x.Name,
                Description = x.Description,
                MemberCount = x.MemberCount,
                CreatedAt = x.CreatedAt,
                SyncedAt = DateTime.UtcNow
            }),
            dbSet: db.Groups,
            entityType: "Groups");

        logger.LogInformation("Base data sync completed for org {OrgUuid}", orgUuid);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private async Task SyncEntityAsync<TDto, TEntity>(
        string orgUuid,
        CancellationToken ct,
        Func<Task<List<TDto>>> fetch,
        Func<List<TDto>, IEnumerable<TEntity>> map,
        DbSet<TEntity> dbSet,
        string entityType) where TEntity : class
    {
        var log = new MendSyncLog { EntityType = entityType, SyncedAt = DateTime.UtcNow };
        try
        {
            logger.LogInformation("Syncing {EntityType}...", entityType);
            var items = await fetch();

            await dbSet.ExecuteDeleteAsync(ct);
            await dbSet.AddRangeAsync(map(items), ct);
            await db.SaveChangesAsync(ct);

            log.RecordCount = items.Count;
            log.Success = true;
            logger.LogInformation("Synced {Count} {EntityType}", items.Count, entityType);
        }
        catch (Exception ex)
        {
            log.Success = false;
            log.ErrorMessage = ex.Message;
            logger.LogError(ex, "Failed to sync {EntityType}", entityType);
        }
        finally
        {
            db.SyncLogs.Add(log);
            await db.SaveChangesAsync(ct);
        }
    }

    /// <summary>Fetches all pages from a paginated Mend endpoint.</summary>
    private async Task<List<TDto>> FetchAllPagesAsync<TDto>(string baseUrl, CancellationToken ct)
    {
        var all = new List<TDto>();
        string? cursor = null;

        do
        {
            var url = $"{baseUrl}?limit=100" + (cursor != null ? $"&cursor={cursor}" : "");
            var page = await client.GetAsync<PagedResultDto<TDto>>(url, ct);
            if (page == null) break;

            all.AddRange(page.Response);
            cursor = page.AdditionalData?.Paging?.Next;
        }
        while (!string.IsNullOrEmpty(cursor));

        return all;
    }

    /// <summary>Fetches a non-paginated list from a Mend endpoint.</summary>
    private async Task<List<TDto>> FetchListAsync<TDto>(string url, CancellationToken ct)
    {
        var result = await client.GetAsync<MendResponse<List<TDto>>>(url, ct);
        return result?.Response ?? [];
    }
}
