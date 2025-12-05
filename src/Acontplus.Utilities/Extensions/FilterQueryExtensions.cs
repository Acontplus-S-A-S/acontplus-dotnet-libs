using System.ComponentModel;
using Acontplus.Utilities.Dtos;

namespace Acontplus.Utilities.Extensions;

/// <summary>
/// Extension methods for FilterQuery to provide helper functionality.
/// </summary>
public static class FilterQueryExtensions
{
    /// <summary>
    /// Gets a filter value by key with type safety and conversion support.
    /// </summary>
    /// <typeparam name="T">The expected type of the filter value</typeparam>
    /// <param name="query">The filter query</param>
    /// <param name="key">The filter key to retrieve</param>
    /// <param name="defaultValue">Default value if key not found or conversion fails</param>
    /// <returns>The filter value converted to type T, or the default value</returns>
    public static T? GetFilterValue<T>(this FilterQuery query, string key, T? defaultValue = default)
    {
        if (query.Filters == null || !query.Filters.TryGetValue(key, out var value))
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
    /// <param name="query">The filter query</param>
    /// <param name="key">The filter key to retrieve</param>
    /// <param name="value">The output value if found and successfully converted</param>
    /// <returns>True if the value was found and successfully converted; otherwise, false</returns>
    public static bool TryGetFilterValue<T>(this FilterQuery query, string key, out T? value)
    {
        value = default;

        if (query.Filters == null || !query.Filters.TryGetValue(key, out var rawValue))
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
