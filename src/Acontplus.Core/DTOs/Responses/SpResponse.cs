namespace Acontplus.Core.DTOs.Responses;

public record SpResponse
{
    public required string Code { get; set; }
    public dynamic? Result { get; set; }
    public dynamic? Payload { get; set; } // Further consider remove Payload in all cases and change to Result only
    public string? Message { get; set; }
    public bool IsSuccess => Code == "0" || Code == "1"; // Further consider remove 1 in all cases and change to 0 only
}
