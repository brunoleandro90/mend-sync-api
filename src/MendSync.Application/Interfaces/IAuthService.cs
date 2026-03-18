using MendSync.Application.DTOs.Auth;

namespace MendSync.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(string email, string userKey);
    Task<AccessTokenResponseDto> RefreshAccessTokenAsync();
    Task LogoutAsync();
}
