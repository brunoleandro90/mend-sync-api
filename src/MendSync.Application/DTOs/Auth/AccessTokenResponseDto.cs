namespace MendSync.Application.DTOs.Auth;

public class AccessTokenResponseDto
{
    public string JwtToken { get; set; } = string.Empty;
    public string OrgUuid { get; set; } = string.Empty;
    public long TokenTTL { get; set; }
}
