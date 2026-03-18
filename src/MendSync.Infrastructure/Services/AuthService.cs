using MendSync.Application.DTOs.Auth;
using MendSync.Application.DTOs.Common;
using MendSync.Application.Interfaces;
using MendSync.Infrastructure.HttpClients;
using MendSync.Infrastructure.Token;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace MendSync.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly MendApiClient _client;
    private readonly TokenStore _tokenStore;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthService> _logger;

    public AuthService(MendApiClient client, TokenStore tokenStore, IConfiguration config, ILogger<AuthService> logger)
    {
        _client = client;
        _tokenStore = tokenStore;
        _config = config;
        _logger = logger;
        _client.SetAuthService(this);
    }

    public async Task<LoginResponseDto> LoginAsync(string email, string userKey)
    {
        _logger.LogInformation("Performing Mend login for {Email}", email);

        var loginPayload = new { email, userKey };
        var loginResponse = await _client.RawHttpClient.PostAsJsonAsync("/api/v3.0/login", loginPayload);
        loginResponse.EnsureSuccessStatusCode();
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<MendResponse<LoginResponseDto>>()
            ?? throw new InvalidOperationException("Login response was null");

        var data = loginResult.Response ?? throw new InvalidOperationException("Login response data was null");
        _tokenStore.SetRefreshToken(data.RefreshToken);

        // Automatically generate access token after login
        await RefreshAccessTokenAsync();

        return data;
    }

    public async Task<AccessTokenResponseDto> RefreshAccessTokenAsync()
    {
        var refreshToken = _tokenStore.GetRefreshToken()
            ?? throw new InvalidOperationException("No refresh token available. Please login first.");

        _logger.LogInformation("Refreshing Mend access token");

        var orgUuid = _config["Mend:OrgUuid"]
            ?? throw new InvalidOperationException("OrgUuid not configured in Mend:OrgUuid");

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v3.0/login/accessToken?orgUuid={orgUuid}");
        request.Headers.Add("wss-refresh-token", refreshToken);

        var response = await _client.RawHttpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<MendResponse<AccessTokenResponseDto>>()
            ?? throw new InvalidOperationException("Access token response was null");

        var data = result.Response ?? throw new InvalidOperationException("Access token response data was null");
        _tokenStore.SetJwtToken(data.JwtToken, data.TokenTTL);
        _tokenStore.SetOrgUuid(data.OrgUuid);

        return data;
    }

    public async Task LogoutAsync()
    {
        _logger.LogInformation("Logging out from Mend");
        await _client.PostRawAsync<object>("/api/v3.0/logout", new { });
        _tokenStore.Clear();
    }
}
