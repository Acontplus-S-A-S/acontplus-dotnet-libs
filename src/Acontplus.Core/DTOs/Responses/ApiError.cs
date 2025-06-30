using System.Text.Json.Serialization;

namespace Acontplus.Core.DTOs.Responses;

/// Enhanced error format with additional context for modern frontends
/// </summary>
public sealed record ApiError(
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("target")] string? Target = null,
    [property: JsonPropertyName("details")] Dictionary<string, string>? Details = null,
    [property: JsonPropertyName("severity")] string Severity = "error") // error, warning, info
{
    /// <summary>
    /// Additional context for debugging (only in development)
    /// </summary>
    [JsonPropertyName("debug")]
    public object? Debug { get; init; }

    /// <summary>
    /// Error category for frontend handling
    /// </summary>
    [JsonPropertyName("category")]
    public string? Category { get; init; } // validation, authorization, business, system

    /// <summary>
    /// Suggested action for the user
    /// </summary>
    [JsonPropertyName("suggestedAction")]
    public string? SuggestedAction { get; init; }
}