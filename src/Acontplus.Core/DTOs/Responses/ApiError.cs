namespace Acontplus.Core.Dtos.Responses;

public sealed record ApiError(
    string Code,
    string Message,
    string? Target = null,
    Dictionary<string, object>? Details = null,
    string Severity = "error",
    string Category = "system",
    string? HelpUrl = null,
    string? SuggestedAction = null,
    string? TraceId = null);