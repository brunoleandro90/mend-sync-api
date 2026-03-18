namespace MendSync.Application.DTOs.Common;

public class MendResponse<T>
{
    public T? Response { get; set; }
    public string? SupportToken { get; set; }
}
