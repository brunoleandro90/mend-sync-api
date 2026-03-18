namespace MendSync.Application.DTOs.Users;

public class UserDto
{
    public string Uuid { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Role { get; set; }
    public bool? IsBlocked { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

public class InviteUserDto
{
    public string Email { get; set; } = string.Empty;
    public string? Role { get; set; }
    public IEnumerable<string>? GroupUuids { get; set; }
}
