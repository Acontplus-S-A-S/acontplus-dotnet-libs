using System.Text.Json;
using System.Text.Json.Serialization;

namespace Acontplus.Core.DTOs.Responses;

public record LegacySpResponse(
    string Code,
    string? Message,
    dynamic? Payload = null
)
{
    [JsonIgnore]
    public bool IsSuccess => Code == "1";

    public T? GetContent<T>() => Payload != null
        ? JsonSerializer.Deserialize<T>(Payload)
        : default;
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