using Acontplus.Core.Enums;

namespace Acontplus.Core.DTOs;

public record PaginationDto
{
    private int _pageIndex = 1;
    private int _pageSize = 10;

    public int PageIndex
    {
        get => _pageIndex;
        init => _pageIndex = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value switch
        {
            < 1 => 10,
            > 1000 => 1000, // Increased limit for large systems
            _ => value
        };
    }

    public string SortBy { get; init; }
    public SortDirection SortDirection { get; init; } = SortDirection.Ascending;
    public string SearchTerm { get; init; }
    public IReadOnlyDictionary<string, object> Filters { get; init; }

    public int Skip => (PageIndex - 1) * PageSize;
    public int Take => PageSize;

    public bool IsEmpty => string.IsNullOrWhiteSpace(SearchTerm) &&
                          (Filters is null || !Filters.Any());
}
