using System.Text.Json;
using System.Text.Json.Serialization;

namespace Acontplus.Core.Extensions;

public static class JsonExtensions
{
    public static JsonSerializerOptions DefaultOptions => new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public static JsonSerializerOptions PrettyOptions => new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

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

    public static string SerializeModern<T>(this T obj, bool pretty = false)
    {
        if (obj == null)
            return "null";

        var options = pretty ? PrettyOptions : DefaultOptions;
        return JsonSerializer.Serialize(obj, options);
    }

    public static T CloneViaJson<T>(this T obj)
    {
        if (obj == null)
            return default!;

        var json = JsonSerializer.Serialize(obj, DefaultOptions);
        return JsonSerializer.Deserialize<T>(json, DefaultOptions)!;
    }
}
