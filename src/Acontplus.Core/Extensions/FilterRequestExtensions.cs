using System.ComponentModel;

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

    /// <summary>
    /// Gets a filter value by key with type safety and conversion support.
    /// </summary>
    /// <typeparam name="T">The expected type of the filter value</typeparam>
    /// <param name="filter">The filter request</param>
    /// <param name="key">The filter key to retrieve</param>
    /// <param name="defaultValue">Default value if key not found or conversion fails</param>
    /// <returns>The filter value converted to type T, or the default value</returns>
    public static T? GetFilterValue<T>(this FilterRequest filter, string key, T? defaultValue = default)
    {
        if (filter.Filters == null || !filter.Filters.TryGetValue(key, out var value))
            return defaultValue;

        try
        {
            if (value is T typed)
                return typed;

            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter.CanConvertFrom(value.GetType()))
                return (T?)converter.ConvertFrom(value);

            return defaultValue;
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Tries to get a filter value by key with type safety and conversion support.
    /// </summary>
    /// <typeparam name="T">The expected type of the filter value</typeparam>
    /// <param name="filter">The filter request</param>
    /// <param name="key">The filter key to retrieve</param>
    /// <param name="value">The output value if found and successfully converted</param>
    /// <returns>True if the value was found and successfully converted; otherwise, false</returns>
    public static bool TryGetFilterValue<T>(this FilterRequest filter, string key, out T? value)
    {
        value = default;

        if (filter.Filters == null || !filter.Filters.TryGetValue(key, out var rawValue))
            return false;

        try
        {
            if (rawValue is T typed)
            {
                value = typed;
                return true;
            }

            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter.CanConvertFrom(rawValue.GetType()))
            {
                value = (T?)converter.ConvertFrom(rawValue);
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}
