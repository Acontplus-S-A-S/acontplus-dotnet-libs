using System.Text.Json;
using System.Text.Json.Serialization;

namespace Acontplus.Core.Extensions;


/// <summary>
/// Modern JSON extensions for .NET 9 enterprise applications
/// Provides high-performance, secure JSON operations with System.Text.Json
/// </summary>
public static class JsonExtensions
{
    /// <summary>
    /// Default JSON serializer options optimized for enterprise applications
    /// </summary>
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        WriteIndented = false, // For production performance
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    /// <summary>
    /// JSON options for development/debugging with pretty formatting
    /// </summary>
    private static readonly JsonSerializerOptions PrettyOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        WriteIndented = true, // Pretty formatting for development
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    /// <summary>
    /// Strict JSON options for APIs that require exact matching
    /// </summary>
    private static readonly JsonSerializerOptions StrictOptions = new()
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
    /// <typeparam name="T">Type to deserialize to</typeparam>
    /// <param name="json">JSON string</param>
    /// <returns>Deserialized object</returns>
    /// <exception cref="JsonException">When JSON is invalid or cannot be deserialized</exception>
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
    /// <typeparam name="T">Type to deserialize to</typeparam>
    /// <param name="json">JSON string</param>
    /// <param name="fallback">Fallback value if deserialization fails</param>
    /// <returns>Deserialized object or fallback</returns>
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
    /// <typeparam name="T">Type to serialize</typeparam>
    /// <param name="obj">Object to serialize</param>
    /// <param name="pretty">Whether to use pretty formatting (for development)</param>
    /// <returns>JSON string</returns>
    public static string SerializeModern<T>(this T obj, bool pretty = false)
    {
        if (obj == null)
            return "null";

        var options = pretty ? PrettyOptions : DefaultOptions;
        return JsonSerializer.Serialize(obj, options);
    }

    /// <summary>
    /// Deserialize with strict validation (for critical APIs)
    /// </summary>
    /// <typeparam name="T">Type to deserialize to</typeparam>
    /// <param name="json">JSON string</param>
    /// <returns>Deserialized object</returns>
    public static T DeserializeStrict<T>(this string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON string cannot be null or empty", nameof(json));

        return JsonSerializer.Deserialize<T>(json, StrictOptions)!;
    }

    /// <summary>
    /// Validate if string is valid JSON
    /// </summary>
    /// <param name="json">JSON string to validate</param>
    /// <returns>True if valid JSON</returns>
    public static bool IsValidJson(this string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return false;

        try
        {
            using var document = JsonDocument.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Clone object using JSON serialization (deep clone)
    /// </summary>
    /// <typeparam name="T">Type to clone</typeparam>
    /// <param name="obj">Object to clone</param>
    /// <returns>Cloned object</returns>
    public static T CloneViaJson<T>(this T obj)
    {
        if (obj == null)
            return default!;

        var json = JsonSerializer.Serialize(obj, DefaultOptions);
        return JsonSerializer.Deserialize<T>(json, DefaultOptions)!;
    }

    /// <summary>
    /// Convert object to JsonDocument for advanced manipulation
    /// </summary>
    /// <typeparam name="T">Type to convert</typeparam>
    /// <param name="obj">Object to convert</param>
    /// <returns>JsonDocument</returns>
    public static JsonDocument ToJsonDocument<T>(this T obj)
    {
        var json = JsonSerializer.Serialize(obj, DefaultOptions);
        return JsonDocument.Parse(json);
    }

    /// <summary>
    /// Get JSON property value safely
    /// </summary>
    /// <param name="json">JSON string</param>
    /// <param name="propertyName">Property name</param>
    /// <returns>Property value or null</returns>
    public static string? GetJsonProperty(this string json, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(json) || string.IsNullOrWhiteSpace(propertyName))
            return null;

        try
        {
            using var document = JsonDocument.Parse(json);
            if (document.RootElement.TryGetProperty(propertyName, out var property))
            {
                return property.GetString();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Merge two JSON objects (second object overwrites first)
    /// </summary>
    /// <param name="json1">First JSON string</param>
    /// <param name="json2">Second JSON string</param>
    /// <returns>Merged JSON string</returns>
    public static string MergeJson(this string json1, string json2)
    {
        if (string.IsNullOrWhiteSpace(json1)) return json2;
        if (string.IsNullOrWhiteSpace(json2)) return json1;

        try
        {
            using var doc1 = JsonDocument.Parse(json1);
            using var doc2 = JsonDocument.Parse(json2);

            var merged = new Dictionary<string, object>();

            // Add properties from first JSON
            foreach (var property in doc1.RootElement.EnumerateObject())
            {
                merged[property.Name] = property.Value.GetRawText();
            }

            // Overwrite with properties from second JSON
            foreach (var property in doc2.RootElement.EnumerateObject())
            {
                merged[property.Name] = property.Value.GetRawText();
            }

            return JsonSerializer.Serialize(merged, DefaultOptions);
        }
        catch
        {
            return json2; // Return second JSON if merge fails
        }
    }
}
