using System.Text.Json;
using System.Text.Json.Serialization;

namespace Acontplus.Core.DTOs.Responses;

/// <summary>
/// Legacy response with dynamic payload. Prefer the generic version for type safety.
/// </summary>
public record LegacySpResponse
{
    public string Code { get; set; }
    public string? Message { get; set; }
    public dynamic? Payload { get; set; }

    [JsonIgnore]
    public bool IsSuccess => Code == "0";

    public LegacySpResponse() { }

    public LegacySpResponse(string code, string? message = null, dynamic? payload = null)
    {
        Code = code;
        Message = message;
        Payload = payload;
    }

    /// <summary>
    /// Creates a success response with code "0" and optional payload/message.
    /// </summary>
    public static LegacySpResponse Success(dynamic? payload = null, string message = "Operation successful")
        => new("0", message, payload);

    public static LegacySpResponse Error(string code, string message)
        => new(code, message);
}

/// <summary>
/// Legacy response with strongly-typed payload. Prefer this for type safety.
/// </summary>
public record LegacySpResponse<T> : LegacySpResponse
{
    public new T? Payload { get; set; }

    public LegacySpResponse() { }

    public LegacySpResponse(string code, string? message = null, T? payload = default)
        : base(code, message, payload)
    {
        Payload = payload;
    }

    /// <summary>
    /// Creates a success response with code "0" and strongly-typed payload.
    /// </summary>
    public static LegacySpResponse<T> Success(T? payload = default, string message = "Operation successful")
        => new("0", message, payload);

    public static new LegacySpResponse<T> Error(string code, string message)
        => new(code, message);
}

/// <summary>
/// Modern response with dynamic content. Prefer SimpleResponse<T> for type safety.
/// </summary>
public record SpResponse(
    string Code,
    string? Message = null,
    dynamic? Content = null
)
{
    [JsonIgnore]
    public bool IsSuccess => Code == "0";

    /// <summary>
    /// Attempts to deserialize Content to the specified type. Handles both string and object cases.
    /// </summary>
    public T? GetContent<T>()
    {
        if (Content is null)
            return default;
        if (Content is T t)
            return t;
        if (Content is string s)
            return JsonSerializer.Deserialize<T>(s);
        return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(Content));
    }
}