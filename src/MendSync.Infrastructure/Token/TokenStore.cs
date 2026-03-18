namespace MendSync.Infrastructure.Token;

public class TokenStore
{
    private string? _refreshToken;
    private string? _jwtToken;
    private DateTime _jwtExpiry = DateTime.MinValue;
    private string? _orgUuid;

    public void SetRefreshToken(string token) => _refreshToken = token;
    public string? GetRefreshToken() => _refreshToken;

    public void SetJwtToken(string token, long ttlMs)
    {
        _jwtToken = token;
        _jwtExpiry = DateTime.UtcNow.AddMilliseconds(ttlMs - 30000); // 30s buffer
    }

    public bool IsJwtValid() => _jwtToken != null && DateTime.UtcNow < _jwtExpiry;
    public string? GetJwtToken() => _jwtToken;

    public void SetOrgUuid(string orgUuid) => _orgUuid = orgUuid;
    public string? GetOrgUuid() => _orgUuid;

    public void Clear()
    {
        _refreshToken = null;
        _jwtToken = null;
        _jwtExpiry = DateTime.MinValue;
        _orgUuid = null;
    }
}
