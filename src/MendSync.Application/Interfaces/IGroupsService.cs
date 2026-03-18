using MendSync.Application.DTOs.Groups;
using MendSync.Application.DTOs.Users;

namespace MendSync.Application.Interfaces;

public interface IGroupsService
{
    Task<IEnumerable<GroupDto>> GetGroupsAsync(string orgUuid);
    Task<GroupDto> CreateGroupAsync(string orgUuid, CreateGroupDto request);
    Task<GroupDto> GetGroupAsync(string orgUuid, string groupUuid);
    Task UpdateGroupAsync(string orgUuid, string groupUuid, UpdateGroupDto request);
    Task DeleteGroupAsync(string orgUuid, string groupUuid);
    Task<IEnumerable<RoleDto>> GetGroupRolesAsync(string orgUuid, string groupUuid);
    Task AddGroupRolesAsync(string orgUuid, string groupUuid, GroupRolesDto request);
    Task RemoveGroupRolesAsync(string orgUuid, string groupUuid, GroupRolesDto request);
    Task<IEnumerable<UserDto>> GetGroupUsersAsync(string orgUuid, string groupUuid);
    Task AddUserToGroupAsync(string orgUuid, string groupUuid, string userUuid);
    Task RemoveUserFromGroupAsync(string orgUuid, string groupUuid, string userUuid);
}
