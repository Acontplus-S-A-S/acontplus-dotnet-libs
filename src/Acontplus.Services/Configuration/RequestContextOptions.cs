namespace Acontplus.Services.Configuration;

/// <summary>
/// Options for configuring request context and security headers.
/// </summary>
public class RequestContextOptions
{
    /// <summary>
    /// Enables security headers for all responses. Default: true.
    /// </summary>
    public bool EnableSecurityHeaders { get; set; } = true;
    /// <summary>
    /// Denies framing of the site (X-Frame-Options: DENY). Default: true.
    /// </summary>
    public bool FrameOptionsDeny { get; set; } = true;
    /// <summary>
    /// The referrer policy to use. Default: "strict-origin-when-cross-origin".
    /// </summary>
    public string ReferrerPolicy { get; set; } = "strict-origin-when-cross-origin";
    /// <summary>
    /// Requires a client ID for requests. Default: false.
    /// </summary>
    public bool RequireClientId { get; set; } = false;
    /// <summary>
    /// The default anonymous client ID. Default: "anonymous".
    /// </summary>
    public string? AnonymousClientId { get; set; } = "anonymous"; // Default anonymous client ID
    /// <summary>
    /// List of allowed client IDs for whitelisting.
    /// </summary>
    public List<string>? AllowedClientIds { get; set; } // New: For whitelisting client IDs
}