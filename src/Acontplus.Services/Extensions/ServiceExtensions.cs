using Acontplus.Core.Abstractions.Services;

namespace Acontplus.Services.Extensions;

/// <summary>
/// Extension methods for registering application-level services.
/// NOTE: For infrastructure services (caching, resilience, HTTP clients), use Acontplus.Infrastructure package.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Add monitoring and telemetry services.
    /// </summary>
    public static IServiceCollection AddMonitoringServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add application insights if configured
        var connectionString = configuration["ApplicationInsights:ConnectionString"];
        if (!string.IsNullOrEmpty(connectionString))
        {
            services.AddApplicationInsightsTelemetry();
        }

        return services;
    }

    /// <summary>
    /// Add device detection service.
    /// </summary>
    public static IServiceCollection AddDeviceDetection(this IServiceCollection services)
    {
        services.AddScoped<IDeviceDetectionService, DeviceDetectionService>();
        return services;
    }

    /// <summary>
    /// Add request context service.
    /// </summary>
    public static IServiceCollection AddRequestContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RequestContextConfiguration>(configuration.GetSection("RequestContext"));
        services.AddScoped<IRequestContextService, RequestContextService>();
        return services;
    }

    /// <summary>
    /// Add security header service.
    /// </summary>
    public static IServiceCollection AddSecurityHeaders(this IServiceCollection services)
    {
        services.AddScoped<ISecurityHeaderService, SecurityHeaderService>();
        return services;
    }
}


