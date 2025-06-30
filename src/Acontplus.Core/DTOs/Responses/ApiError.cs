using System.Text.Json.Serialization;

namespace Acontplus.Core.DTOs.Responses;

public sealed record ApiError(
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("target")] string? Target = null,
    [property: JsonPropertyName("details")] Dictionary<string, string>? Details = null,
    [property: JsonPropertyName("severity")] string Severity = "error",
    [property: JsonPropertyName("category")] string Category = "system",
    [property: JsonPropertyName("debug")] object? Debug = null
    
    )
{
    [JsonPropertyName("suggestedAction")]
    public string? SuggestedAction { get; init; }
}