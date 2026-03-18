using System.Net.Http.Headers;
using System.Net.Http.Json;
using MendSync.Application.Interfaces;
using MendSync.Infrastructure.Token;

namespace MendSync.Infrastructure.HttpClients;

public class MendApiClient
{
    private readonly HttpClient _httpClient;
    private readonly TokenStore _tokenStore;
    private IAuthService? _authService;

    public MendApiClient(HttpClient httpClient, TokenStore tokenStore)
    {
        _httpClient = httpClient;
        _tokenStore = tokenStore;
    }

    // Injected after construction to avoid circular dependency
    public void SetAuthService(IAuthService authService) => _authService = authService;

    private async Task EnsureTokenAsync()
    {
        if (!_tokenStore.IsJwtValid() && _authService != null)
            await _authService.RefreshAccessTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _tokenStore.GetJwtToken());
    }

    public async Task<T?> GetAsync<T>(string endpoint, CancellationToken ct = default)
    {
        await EnsureTokenAsync();
        var response = await _httpClient.GetAsync(endpoint, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest body, CancellationToken ct = default)
    {
        await EnsureTokenAsync();
        var response = await _httpClient.PostAsJsonAsync(endpoint, body, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: ct);
    }

    public async Task PostRawAsync<TRequest>(string endpoint, TRequest body, CancellationToken ct = default)
    {
        await EnsureTokenAsync();
        var response = await _httpClient.PostAsJsonAsync(endpoint, body, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest body, CancellationToken ct = default)
    {
        await EnsureTokenAsync();
        var response = await _httpClient.PutAsJsonAsync(endpoint, body, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: ct);
    }

    public async Task PutRawAsync<TRequest>(string endpoint, TRequest body, CancellationToken ct = default)
    {
        await EnsureTokenAsync();
        var response = await _httpClient.PutAsJsonAsync(endpoint, body, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(string endpoint, CancellationToken ct = default)
    {
        await EnsureTokenAsync();
        var response = await _httpClient.DeleteAsync(endpoint, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task<TResponse?> PatchAsync<TRequest, TResponse>(string endpoint, TRequest body, CancellationToken ct = default)
    {
        await EnsureTokenAsync();
        var request = new HttpRequestMessage(HttpMethod.Patch, endpoint)
        {
            Content = JsonContent.Create(body)
        };
        var response = await _httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: ct);
    }

    public async Task<byte[]> GetBytesAsync(string endpoint, CancellationToken ct = default)
    {
        await EnsureTokenAsync();
        var response = await _httpClient.GetAsync(endpoint, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync(ct);
    }

    // Used by AuthService directly (no token needed)
    public HttpClient RawHttpClient => _httpClient;
}
