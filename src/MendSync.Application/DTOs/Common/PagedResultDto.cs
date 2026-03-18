namespace MendSync.Application.DTOs.Common;

public class PagedResultDto<T>
{
    public IEnumerable<T> Response { get; set; } = [];

    public AdditionalData? AdditionalData { get; set; }
}

public class AdditionalData
{
    public int TotalItems { get; set; }
    public Paging? Paging { get; set; }
}

public class Paging
{
    public string? Next { get; set; }
}

public class PaginationParams
{
    public int PageSize { get; set; } = 25;
    public string? Cursor { get; set; }
}
