// AcontPlus.Core/Extensions/JsonExtensions.cs
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Acontplus.Core.Extensions;

/// <summary>
/// Modern JSON extensions for .NET enterprise applications
/// Provides high-performance, secure JSON operations with System.Text.Json
/// </summary>
public static class JsonExtensions
{
    /// <summary>
    /// Default JSON serializer options optimized for enterprise applications
    /// </summary>
    public static JsonSerializerOptions DefaultOptions => new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        WriteIndented = false, // For production performance
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    /// <summary>
    /// JSON options for development/debugging with pretty formatting
    /// </summary>
    public static JsonSerializerOptions PrettyOptions => new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        WriteIndented = true, // Pretty formatting for development
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    /// <summary>
    /// Strict JSON options for APIs that require exact matching
    /// </summary>
    public static JsonSerializerOptions StrictOptions => new()
    {
        PropertyNameCaseInsensitive = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        AllowTrailingCommas = false,
        ReadCommentHandling = JsonCommentHandling.Disallow,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        NumberHandling = JsonNumberHandling.Strict
    };

    /// <summary>
    /// Deserialize JSON with enterprise-optimized settings
    /// </summary>
    public static T DeserializeModern<T>(this string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON string cannot be null or empty", nameof(json));

        try
        {
            return JsonSerializer.Deserialize<T>(json, DefaultOptions)!;
        }
        catch (JsonException ex)
        {
            throw new JsonException($"Failed to deserialize JSON to {typeof(T).Name}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Safely deserialize JSON with fallback value
    /// </summary>
    public static T DeserializeModernSafe<T>(this string json, T fallback = default!)
    {
        if (string.IsNullOrWhiteSpace(json))
            return fallback;

        try
        {
            return JsonSerializer.Deserialize<T>(json, DefaultOptions) ?? fallback;
        }
        catch
        {
            return fallback;
        }
    }

    /// <summary>
    /// Serialize object to JSON with enterprise-optimized settings
    /// </summary>
    public static string SerializeModern<T>(this T obj, bool pretty = false)
    {
        if (obj == null)
            return "null";

        var options = pretty ? PrettyOptions : DefaultOptions;
        return JsonSerializer.Serialize(obj, options);
    }

    /// <summary>
    /// Clone object using JSON serialization (deep clone)
    /// </summary>
    public static T CloneViaJson<T>(this T obj)
    {
        if (obj == null)
            return default!;

        var json = JsonSerializer.Serialize(obj, DefaultOptions);
        return JsonSerializer.Deserialize<T>(json, DefaultOptions)!;
    }
}