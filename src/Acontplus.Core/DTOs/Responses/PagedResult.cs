namespace Acontplus.Core.DTOs.Responses;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public PagedResult()
    {
    }

    public PagedResult(IEnumerable<T> items, int pageIndex, int pageSize, int totalCount)
    {
        Items = items?.ToList() ?? [];
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = pageSize > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 0;
    }
}


