using MendSync.Application.DTOs.Common;
using MendSync.Application.DTOs.Groups;
using MendSync.Application.DTOs.Users;
using MendSync.Application.Interfaces;
using MendSync.Infrastructure.Token;
using Microsoft.AspNetCore.Mvc;

namespace MendSync.API.Controllers;

[ApiController]
[Route("api/groups")]
public class GroupsController : ControllerBase
{
    private readonly IGroupsService _service;
    private readonly TokenStore _tokenStore;

    public GroupsController(IGroupsService service, TokenStore tokenStore)
    {
        _service = service;
        _tokenStore = tokenStore;
    }

    private string OrgUuid => _tokenStore.GetOrgUuid()
        ?? throw new InvalidOperationException("Not authenticated. Please login first.");

    [HttpGet]
    public async Task<IActionResult> GetGroups()
    {
        var result = await _service.GetGroupsAsync(OrgUuid);
        return Ok(ApiResponse<IEnumerable<GroupDto>>.Ok(result));
    }

    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto request)
    {
        var result = await _service.CreateGroupAsync(OrgUuid, request);
        return Ok(ApiResponse<GroupDto>.Ok(result));
    }

    [HttpGet("{groupUuid}")]
    public async Task<IActionResult> GetGroup(string groupUuid)
    {
        var result = await _service.GetGroupAsync(OrgUuid, groupUuid);
        return Ok(ApiResponse<GroupDto>.Ok(result));
    }

    [HttpPut("{groupUuid}")]
    public async Task<IActionResult> UpdateGroup(string groupUuid, [FromBody] UpdateGroupDto request)
    {
        await _service.UpdateGroupAsync(OrgUuid, groupUuid, request);
        return NoContent();
    }

    [HttpDelete("{groupUuid}")]
    public async Task<IActionResult> DeleteGroup(string groupUuid)
    {
        await _service.DeleteGroupAsync(OrgUuid, groupUuid);
        return NoContent();
    }

    [HttpGet("{groupUuid}/roles")]
    public async Task<IActionResult> GetGroupRoles(string groupUuid)
    {
        var result = await _service.GetGroupRolesAsync(OrgUuid, groupUuid);
        return Ok(ApiResponse<IEnumerable<RoleDto>>.Ok(result));
    }

    [HttpPost("{groupUuid}/roles")]
    public async Task<IActionResult> AddGroupRoles(string groupUuid, [FromBody] GroupRolesDto request)
    {
        await _service.AddGroupRolesAsync(OrgUuid, groupUuid, request);
        return NoContent();
    }

    [HttpPut("{groupUuid}/roles/remove")]
    public async Task<IActionResult> RemoveGroupRoles(string groupUuid, [FromBody] GroupRolesDto request)
    {
        await _service.RemoveGroupRolesAsync(OrgUuid, groupUuid, request);
        return NoContent();
    }

    [HttpGet("{groupUuid}/users")]
    public async Task<IActionResult> GetGroupUsers(string groupUuid)
    {
        var result = await _service.GetGroupUsersAsync(OrgUuid, groupUuid);
        return Ok(ApiResponse<IEnumerable<UserDto>>.Ok(result));
    }

    [HttpPost("{groupUuid}/users/{userUuid}")]
    public async Task<IActionResult> AddUserToGroup(string groupUuid, string userUuid)
    {
        await _service.AddUserToGroupAsync(OrgUuid, groupUuid, userUuid);
        return NoContent();
    }

    [HttpDelete("{groupUuid}/users/{userUuid}")]
    public async Task<IActionResult> RemoveUserFromGroup(string groupUuid, string userUuid)
    {
        await _service.RemoveUserFromGroupAsync(OrgUuid, groupUuid, userUuid);
        return NoContent();
    }
}
