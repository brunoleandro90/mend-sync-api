using MendSync.Application.DTOs.Users;

namespace MendSync.Application.DTOs.Groups;

public class GroupDto
{
    public string Uuid { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? MemberCount { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class CreateGroupDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateGroupDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class RoleDto
{
    public string Uuid { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Scope { get; set; }
    public string? ScopeUuid { get; set; }
}

public class GroupRolesDto
{
    public IEnumerable<string> RoleUuids { get; set; } = [];
}
