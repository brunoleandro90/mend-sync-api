using MendSync.Application.DTOs.Common;
using MendSync.Application.DTOs.Users;
using MendSync.Application.Interfaces;
using MendSync.Infrastructure.HttpClients;
using Microsoft.Extensions.Logging;

namespace MendSync.Infrastructure.Services;

public class UsersService : IUsersService
{
    private readonly MendApiClient _client;
    private readonly ILogger<UsersService> _logger;

    public UsersService(MendApiClient client, ILogger<UsersService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<PagedResultDto<UserDto>> GetUsersAsync(string orgUuid, PaginationParams pagination)
    {
        var query = $"?pageSize={pagination.PageSize}" + (pagination.Cursor != null ? $"&cursor={pagination.Cursor}" : "");
        return await _client.GetAsync<PagedResultDto<UserDto>>($"/api/v3.0/orgs/{orgUuid}/users{query}")
            ?? new PagedResultDto<UserDto>();
    }

    public async Task InviteUserAsync(string orgUuid, InviteUserDto request)
    {
        await _client.PostRawAsync($"/api/v3.0/orgs/{orgUuid}/users/invite", request);
    }

    public async Task RemoveUserAsync(string orgUuid, string userUuid)
    {
        await _client.DeleteAsync($"/api/v3.0/orgs/{orgUuid}/users/{userUuid}");
    }

    public async Task BlockUserAsync(string orgUuid, string userUuid)
    {
        await _client.PutRawAsync($"/api/v3.0/orgs/{orgUuid}/users/{userUuid}/block", new { });
    }

    public async Task UnblockUserAsync(string orgUuid, string userUuid)
    {
        await _client.PutRawAsync($"/api/v3.0/orgs/{orgUuid}/users/{userUuid}/unblock", new { });
    }
}
