namespace Acontplus.Services.Services.Abstractions;

/// <summary>
/// Service for detecting device types and capabilities from HTTP requests.
/// </summary>
public interface IDeviceDetectionService
{
    /// <summary>
    /// Detects the device type from the HTTP context.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>The detected device type.</returns>
    DeviceType DetectDeviceType(HttpContext context);

    /// <summary>
    /// Determines if the request is from a mobile device.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>True if mobile, false otherwise.</returns>
    bool IsMobileDevice(HttpContext context);

    /// <summary>
    /// Gets device capabilities from the user agent.
    /// </summary>
    /// <param name="userAgent">The user agent string.</param>
    /// <returns>Device capabilities information.</returns>
    DeviceCapabilities GetDeviceCapabilities(string userAgent);

    /// <summary>
    /// Validates device-specific headers.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>True if headers are valid, false otherwise.</returns>
    bool ValidateDeviceHeaders(HttpContext context);
}

/// <summary>
/// Represents device capabilities and characteristics.
/// </summary>
public record DeviceCapabilities(
    DeviceType Type,
    bool IsMobile,
    bool IsTablet,
    bool SupportsTouch,
    string? OperatingSystem,
    string? Browser,
    string? Version
);