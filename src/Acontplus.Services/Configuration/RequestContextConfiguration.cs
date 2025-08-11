namespace Acontplus.Services.Configuration;

/// <summary>
/// Configuration for request context and security headers.
/// </summary>
public class RequestContextConfiguration
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

    /// <summary>
    /// Content Security Policy configuration.
    /// </summary>
    public CspConfiguration? Csp { get; set; } = new();

    /// <summary>
    /// Resilience configuration for circuit breakers, rate limiting, and retry policies.
    /// </summary>
    public ResilienceConfiguration? Resilience { get; set; } = new();
}

/// <summary>
/// Configuration for Content Security Policy.
/// </summary>
public class CspConfiguration
{
    /// <summary>
    /// List of allowed image sources (domains) for img-src directive.
    /// </summary>
    public List<string> AllowedImageSources { get; set; } = new() { "https://i.ytimg.com" };

    /// <summary>
    /// List of allowed style sources (domains) for style-src directive.
    /// </summary>
    public List<string> AllowedStyleSources { get; set; } = new() { "https://fonts.googleapis.com" };

    /// <summary>
    /// List of allowed font sources (domains) for font-src directive.
    /// </summary>
    public List<string> AllowedFontSources { get; set; } = new() { "https://fonts.gstatic.com" };

    /// <summary>
    /// List of allowed script sources (domains) for script-src directive.
    /// </summary>
    public List<string> AllowedScriptSources { get; set; } = new();

    /// <summary>
    /// List of allowed connect sources (domains) for connect-src directive.
    /// </summary>
    public List<string> AllowedConnectSources { get; set; } = new();
}
