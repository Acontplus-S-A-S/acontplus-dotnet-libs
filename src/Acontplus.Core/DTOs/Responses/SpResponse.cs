using System.Text.Json;
using System.Text.Json.Serialization;

namespace Acontplus.Core.DTOs.Responses;

// Base class with dynamic payload
public record LegacySpResponse
{
    public string Code { get; set; }
    public string Message { get; set; }
    public dynamic Payload { get; set; }

    [JsonIgnore]
    public bool IsSuccess => Code == "0";

    public LegacySpResponse() { }

    public LegacySpResponse(string code, string message = null, dynamic payload = null)
    {
        Code = code;
        Message = message;
        Payload = payload;
    }

    /// <summary>
    /// Creates a success response with a default code and message.
    /// </summary>
    /// <param name="code"></param> // Default is "1" to success! Excuse me
    /// <param name="payload"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static LegacySpResponse Success(string code = "0", dynamic payload = null, string message = "Operation successful")
    {
        return new LegacySpResponse(code, message, payload);
    }

    public static LegacySpResponse Error(string code, string message)
    {
        return new LegacySpResponse(code, message);
    }
}

// Generic version for strongly-typed payload
public record LegacySpResponse<T> : LegacySpResponse
{
    public new T Payload { get; set; }

    public LegacySpResponse() { }

    public LegacySpResponse(string code, string message = null, T payload = default)
        : base(code, message, null)
    {
        Payload = payload;
    }

    /// <summary>
    /// Creates a success response with a default code and message.
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="code"></param> // Default is "1" to success! Excuse me
    /// <param name="message"></param>
    /// <returns></returns>
    public static LegacySpResponse<T> Success(T payload, string code = "1", string message = "Operation successful")
    {
        return new LegacySpResponse<T>(code, message, payload);
    }

    public static new LegacySpResponse<T> Error(string code, string message)
    {
        return new LegacySpResponse<T>(code, message);
    }
}


public record SpResponse(
    string Code,
    string? Message,
    dynamic? Content = null
)
{
    [JsonIgnore]
    public bool IsSuccess => Code == "0";

    public T? GetContent<T>() => Content != null
        ? JsonSerializer.Deserialize<T>(Content)
        : default;
}