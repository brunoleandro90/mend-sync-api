using MendSync.Application.DTOs.Auth;
using MendSync.Application.DTOs.Common;
using MendSync.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MendSync.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Faz login completo na Mend API (etapa 1 + etapa 2) e retorna o JWT.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AccessTokenResponseDto>), 200)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        await _authService.LoginAsync(request.Email, request.UserKey);
        var token = await _authService.RefreshAccessTokenAsync();
        return Ok(ApiResponse<AccessTokenResponseDto>.Ok(token));
    }

    /// <summary>Encerra a sessão na Mend API.</summary>
    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return Ok(ApiResponse<object>.Ok(new { message = "Logged out successfully" }));
    }
}
