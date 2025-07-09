namespace Acontplus.Services.Configuration;

public class RequestContextOptions
{
    public bool EnableSecurityHeaders { get; set; } = true;
    public bool FrameOptionsDeny { get; set; } = true;
    public string ReferrerPolicy { get; set; } = "strict-origin-when-cross-origin";
    public bool RequireClientId { get; set; } = false;
    public string? AnonymousClientId { get; set; } = "anonymous"; // Default anonymous client ID
    public List<string>? AllowedClientIds { get; set; } // New: For whitelisting client IDs
}