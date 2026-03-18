using MendSync.Application.DTOs.Groups;
using MendSync.Application.DTOs.Users;
using MendSync.Application.Interfaces;
using MendSync.Infrastructure.HttpClients;
using Microsoft.Extensions.Logging;

namespace MendSync.Infrastructure.Services;

public class GroupsService : IGroupsService
{
    private readonly MendApiClient _client;
    private readonly ILogger<GroupsService> _logger;

    public GroupsService(MendApiClient client, ILogger<GroupsService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<IEnumerable<GroupDto>> GetGroupsAsync(string orgUuid)
    {
        return await _client.GetAsync<IEnumerable<GroupDto>>($"/api/v3.0/orgs/{orgUuid}/groups") ?? [];
    }

    public async Task<GroupDto> CreateGroupAsync(string orgUuid, CreateGroupDto request)
    {
        return await _client.PostAsync<CreateGroupDto, GroupDto>($"/api/v3.0/orgs/{orgUuid}/groups", request)
            ?? new GroupDto();
    }

    public async Task<GroupDto> GetGroupAsync(string orgUuid, string groupUuid)
    {
        return await _client.GetAsync<GroupDto>($"/api/v3.0/orgs/{orgUuid}/groups/{groupUuid}")
            ?? new GroupDto();
    }

    public async Task UpdateGroupAsync(string orgUuid, string groupUuid, UpdateGroupDto request)
    {
        await _client.PutRawAsync($"/api/v3.0/orgs/{orgUuid}/groups/{groupUuid}", request);
    }

    public async Task DeleteGroupAsync(string orgUuid, string groupUuid)
    {
        await _client.DeleteAsync($"/api/v3.0/orgs/{orgUuid}/groups/{groupUuid}");
    }

    public async Task<IEnumerable<RoleDto>> GetGroupRolesAsync(string orgUuid, string groupUuid)
    {
        return await _client.GetAsync<IEnumerable<RoleDto>>($"/api/v3.0/orgs/{orgUuid}/groups/{groupUuid}/roles") ?? [];
    }

    public async Task AddGroupRolesAsync(string orgUuid, string groupUuid, GroupRolesDto request)
    {
        await _client.PostRawAsync($"/api/v3.0/orgs/{orgUuid}/groups/{groupUuid}/roles", request);
    }

    public async Task RemoveGroupRolesAsync(string orgUuid, string groupUuid, GroupRolesDto request)
    {
        await _client.PutRawAsync($"/api/v3.0/orgs/{orgUuid}/groups/{groupUuid}/roles/remove", request);
    }

    public async Task<IEnumerable<UserDto>> GetGroupUsersAsync(string orgUuid, string groupUuid)
    {
        return await _client.GetAsync<IEnumerable<UserDto>>($"/api/v3.0/orgs/{orgUuid}/groups/{groupUuid}/users") ?? [];
    }

    public async Task AddUserToGroupAsync(string orgUuid, string groupUuid, string userUuid)
    {
        await _client.PostRawAsync($"/api/v3.0/orgs/{orgUuid}/groups/{groupUuid}/users/{userUuid}", new { });
    }

    public async Task RemoveUserFromGroupAsync(string orgUuid, string groupUuid, string userUuid)
    {
        await _client.DeleteAsync($"/api/v3.0/orgs/{orgUuid}/groups/{groupUuid}/users/{userUuid}");
    }
}
