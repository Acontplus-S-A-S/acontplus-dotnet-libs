namespace Acontplus.Core.Dtos.Responses;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;
    public Dictionary<string, object> Metadata { get; set; } = new();

    // Parameterless constructor for serialization
    public PagedResult() { }

    // Constructor matching the expected signature
    public PagedResult(IEnumerable<T> items, int pageIndex, int pageSize, int totalCount)
        : this(items, pageIndex, pageSize, totalCount, null) { }

    // Full constructor with metadata
    public PagedResult(
        IEnumerable<T> items,
        int pageIndex,
        int pageSize,
        int totalCount,
        Dictionary<string, object>? metadata)
    {
        Items = items?.ToList() ?? new List<T>();
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = totalCount;
        Metadata = metadata ?? new Dictionary<string, object>();
    }
}