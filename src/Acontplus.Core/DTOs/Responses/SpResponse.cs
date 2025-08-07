namespace Acontplus.Core.DTOs.Responses;

public record SpResponse
{
    public required string Code { get; set; }
    public dynamic? Data { get; set; }
    public dynamic? Payload { get; set; } //Consider removing this
    public string? Message { get; set; }
    public bool IsSuccess => Code == "0" || Code == "1"; // Further consider remove 1 in all cases and change to 0 only
}
