namespace Acontplus.Core.Extensions;

/// <summary>
/// Extension methods for FilterRequest to provide fluent API and helper functionality.
/// </summary>
public static class FilterRequestExtensions
{
    /// <summary>
    /// Adds a search term to the filter request.
    /// </summary>
    /// <param name="filter">The filter object</param>
    /// <param name="searchTerm">The search term to add</param>
    /// <returns>A new FilterRequest with the search term</returns>
    public static FilterRequest WithSearch(this FilterRequest filter, string searchTerm)
        => filter with { SearchTerm = searchTerm };

    /// <summary>
    /// Adds sorting criteria to the filter request.
    /// </summary>
    /// <param name="filter">The filter object</param>
    /// <param name="sortBy">The field to sort by</param>
    /// <param name="direction">The sort direction</param>
    /// <returns>A new FilterRequest with sorting criteria</returns>
    public static FilterRequest WithSort(this FilterRequest filter, string sortBy, SortDirection direction = SortDirection.Asc)
        => filter with { SortBy = sortBy, SortDirection = direction };

    /// <summary>
    /// Adds or merges filters with the existing filter request.
    /// </summary>
    /// <param name="filter">The filter object</param>
    /// <param name="filters">The filters to add or merge</param>
    /// <returns>A new FilterRequest with merged filters</returns>
    public static FilterRequest WithFilters(this FilterRequest filter, IReadOnlyDictionary<string, object> filters)
    {
        var existingFilters = filter.Filters?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, object>();
        foreach (var kvp in filters)
        {
            existingFilters[kvp.Key] = kvp.Value;
        }
        return filter with { Filters = existingFilters };
    }

    /// <summary>
    /// Adds a single filter to the filter request.
    /// </summary>
    /// <param name="filter">The filter object</param>
    /// <param name="key">The filter key</param>
    /// <param name="value">The filter value</param>
    /// <returns>A new FilterRequest with the additional filter</returns>
    public static FilterRequest WithFilter(this FilterRequest filter, string key, object value)
    {
        var existingFilters = filter.Filters?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, object>();
        existingFilters[key] = value;
        return filter with { Filters = existingFilters };
    }
}