namespace Acontplus.Core.Extensions;


public static class PaginationExtensions
{
    public static PaginationDto WithSearch(this PaginationDto pagination, string searchTerm)
        => pagination with { SearchTerm = searchTerm };

    public static PaginationDto WithSort(this PaginationDto pagination, string sortBy, SortDirection direction = SortDirection.Ascending)
        => pagination with { SortBy = sortBy, SortDirection = direction };

    public static PaginationDto WithFilters(this PaginationDto pagination, IReadOnlyDictionary<string, object> filters)
        => pagination with { Filters = filters };
}
