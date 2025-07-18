using Acontplus.Services.Configuration;

namespace Acontplus.Services.Services.Abstractions;

/// <summary>
/// Service for managing security headers and policies.
/// </summary>
public interface ISecurityHeaderService
{
    /// <summary>
    /// Applies security headers to the HTTP response.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="configuration">Security configuration options.</param>
    void ApplySecurityHeaders(HttpContext context, RequestContextConfiguration configuration);

    /// <summary>
    /// Generates a Content Security Policy nonce.
    /// </summary>
    /// <returns>A unique nonce value.</returns>
    string GenerateCspNonce();

    /// <summary>
    /// Validates security headers in the request.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>True if headers are valid, false otherwise.</returns>
    bool ValidateSecurityHeaders(HttpContext context);

    /// <summary>
    /// Gets the recommended security headers for the current environment.
    /// </summary>
    /// <param name="isDevelopment">Whether the application is in development mode.</param>
    /// <returns>Dictionary of recommended headers.</returns>
    Dictionary<string, string> GetRecommendedHeaders(bool isDevelopment);
}