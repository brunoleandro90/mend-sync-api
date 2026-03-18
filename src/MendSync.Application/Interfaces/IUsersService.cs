using MendSync.Application.DTOs.Common;
using MendSync.Application.DTOs.Users;

namespace MendSync.Application.Interfaces;

public interface IUsersService
{
    Task<PagedResultDto<UserDto>> GetUsersAsync(string orgUuid, PaginationParams pagination);
    Task InviteUserAsync(string orgUuid, InviteUserDto request);
    Task RemoveUserAsync(string orgUuid, string userUuid);
    Task BlockUserAsync(string orgUuid, string userUuid);
    Task UnblockUserAsync(string orgUuid, string userUuid);
}
