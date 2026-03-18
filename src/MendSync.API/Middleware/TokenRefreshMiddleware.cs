using MendSync.Application.Interfaces;
using MendSync.Infrastructure.Token;

namespace MendSync.API.Middleware;

public class TokenRefreshMiddleware
{
    private readonly RequestDelegate _next;

    public TokenRefreshMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, TokenStore tokenStore, IAuthService authService)
    {
        // Skip auth endpoints
        if (context.Request.Path.StartsWithSegments("/api/auth"))
        {
            await _next(context);
            return;
        }

        // Proactively refresh token if it's about to expire
        if (tokenStore.GetRefreshToken() != null && !tokenStore.IsJwtValid())
        {
            await authService.RefreshAccessTokenAsync();
        }

        await _next(context);
    }
}
