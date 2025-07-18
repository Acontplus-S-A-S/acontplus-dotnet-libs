using Acontplus.Services.Extensions.Context;
using Acontplus.Services.Services.Abstractions;

namespace Acontplus.Services.Services.Implementations;

/// <summary>
/// Implementation of request context service using HTTP context accessor.
/// </summary>
public class RequestContextService : IRequestContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<RequestContextService> _logger;

    public RequestContextService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<RequestContextService> logger)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string GetRequestId()
    {
        var context = GetHttpContext();
        return context.GetRequestId() ?? context.TraceIdentifier;
    }

    public string GetCorrelationId()
    {
        var context = GetHttpContext();
        return context.GetCorrelationId() ?? GetRequestId();
    }

    public string? GetTenantId()
    {
        var context = GetHttpContext();
        return context.GetTenantId();
    }

    public string? GetClientId()
    {
        var context = GetHttpContext();
        return context.GetClientId();
    }

    public string? GetIssuer()
    {
        var context = GetHttpContext();
        return context.GetIssuer();
    }

    public DeviceType GetDeviceType()
    {
        var context = GetHttpContext();
        return context.GetDeviceType() ?? DeviceType.Unknown;
    }

    public bool IsMobileRequest()
    {
        var context = GetHttpContext();
        return context.GetIsMobileRequest() ?? false;
    }

    public Dictionary<string, object?> GetRequestContext()
    {
        var context = GetHttpContext();

        return new Dictionary<string, object?>
        {
            ["requestId"] = GetRequestId(),
            ["correlationId"] = GetCorrelationId(),
            ["tenantId"] = GetTenantId(),
            ["clientId"] = GetClientId(),
            ["issuer"] = GetIssuer(),
            ["deviceType"] = GetDeviceType().ToString(),
            ["isMobileRequest"] = IsMobileRequest(),
            ["userAgent"] = context.Request.Headers.UserAgent.ToString(),
            ["ipAddress"] = context.Connection.RemoteIpAddress?.ToString(),
            ["timestamp"] = DateTime.UtcNow
        };
    }

    private HttpContext GetHttpContext()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
        {
            _logger.LogWarning("HTTP context is not available");
            throw new InvalidOperationException("HTTP context is not available");
        }
        return context;
    }
}