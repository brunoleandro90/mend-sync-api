using MendSync.Application.DTOs.Common;
using MendSync.Application.DTOs.Users;
using MendSync.Application.Interfaces;
using MendSync.Infrastructure.Token;
using Microsoft.AspNetCore.Mvc;

namespace MendSync.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUsersService _service;
    private readonly TokenStore _tokenStore;

    public UsersController(IUsersService service, TokenStore tokenStore)
    {
        _service = service;
        _tokenStore = tokenStore;
    }

    private string OrgUuid => _tokenStore.GetOrgUuid()
        ?? throw new InvalidOperationException("Not authenticated. Please login first.");

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] PaginationParams pagination)
    {
        var result = await _service.GetUsersAsync(OrgUuid, pagination);
        return Ok(ApiResponse<PagedResultDto<UserDto>>.Ok(result));
    }

    [HttpPost("invite")]
    public async Task<IActionResult> InviteUser([FromBody] InviteUserDto request)
    {
        await _service.InviteUserAsync(OrgUuid, request);
        return Ok(ApiResponse<object>.Ok(new { message = "User invited successfully" }));
    }

    [HttpDelete("{userUuid}")]
    public async Task<IActionResult> RemoveUser(string userUuid)
    {
        await _service.RemoveUserAsync(OrgUuid, userUuid);
        return NoContent();
    }

    [HttpPut("{userUuid}/block")]
    public async Task<IActionResult> BlockUser(string userUuid)
    {
        await _service.BlockUserAsync(OrgUuid, userUuid);
        return NoContent();
    }

    [HttpPut("{userUuid}/unblock")]
    public async Task<IActionResult> UnblockUser(string userUuid)
    {
        await _service.UnblockUserAsync(OrgUuid, userUuid);
        return NoContent();
    }
}
