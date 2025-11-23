namespace Acontplus.Core.Abstractions.Services;

/// <summary>
/// Service for managing request context information across the application.
/// </summary>
public interface IRequestContextService
{
    /// <summary>
    /// Gets the current request ID.
    /// </summary>
    string GetRequestId();

    /// <summary>
    /// Gets the current correlation ID.
    /// </summary>
    string GetCorrelationId();

    /// <summary>
    /// Gets the current tenant ID.
    /// </summary>
    string? GetTenantId();

    /// <summary>
    /// Gets the current client ID.
    /// </summary>
    string? GetClientId();

    /// <summary>
    /// Gets the current issuer.
    /// </summary>
    string? GetIssuer();

    /// <summary>
    /// Gets the detected device type for the current request.
    /// </summary>
    DeviceType GetDeviceType();

    /// <summary>
    /// Determines if the current request is from a mobile device.
    /// </summary>
    bool IsMobileRequest();

    /// <summary>
    /// Gets all request context information as a dictionary.
    /// </summary>
    Dictionary<string, object?> GetRequestContext();
}