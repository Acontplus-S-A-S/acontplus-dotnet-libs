namespace Acontplus.Core.DTOs.Requests;

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

    public string? SortBy { get; init; }
    public SortDirection SortDirection { get; init; } = SortDirection.Asc;
    public string? SearchTerm { get; init; }
    public IReadOnlyDictionary<string, object>? Filters { get; init; }

    public int Skip => (PageIndex - 1) * PageSize;
    public int Take => PageSize;

    public bool IsEmpty => string.IsNullOrWhiteSpace(SearchTerm) &&
                          (Filters is null || !Filters.Any());

    /// <summary>
    /// Builds a dictionary of filters if they exist, otherwise returns null.
    /// Useful for converting to stored procedure parameters or other filter formats.
    /// </summary>
    /// <returns>Dictionary of filters or null if no filters exist</returns>
    public Dictionary<string, object>? BuildFilters()
    {
        if (Filters == null || !Filters.Any())
            return null;

        return Filters.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    /// <summary>
    /// Builds a dictionary of filters with custom key prefix (e.g., "@" for SQL parameters).
    /// </summary>
    /// <param name="prefix">Prefix to add to filter keys</param>
    /// <returns>Dictionary of filters with prefixed keys or null if no filters exist</returns>
    public Dictionary<string, object>? BuildFiltersWithPrefix(string prefix)
    {
        if (Filters == null || !Filters.Any())
            return null;

        return Filters.ToDictionary(kvp => prefix + kvp.Key, kvp => kvp.Value);
    }

    /// <summary>
    /// Builds a dictionary of filters with "@" prefix for SQL parameters.
    /// </summary>
    /// <returns>Dictionary of filters with "@" prefix or null if no filters exist</returns>
    public Dictionary<string, object>? BuildSqlParameters()
    {
        return BuildFiltersWithPrefix("@");
    }
}
