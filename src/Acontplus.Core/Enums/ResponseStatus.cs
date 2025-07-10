using System.Text.Json.Serialization;

namespace Acontplus.Core.Enums;

public enum ResponseStatus
{
    [JsonPropertyName("success")]
    Success,

    [JsonPropertyName("error")]
    Error,

    [JsonPropertyName("warning")]
    Warning
}