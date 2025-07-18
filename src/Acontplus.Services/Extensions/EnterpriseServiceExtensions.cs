using Acontplus.Services.Configuration;
using Acontplus.Services.Extensions.Infrastructure;
using Acontplus.Services.Extensions.Middleware;
using Acontplus.Services.Extensions.Security;
using Acontplus.Services.Filters;
using Acontplus.Services.Middleware;
using Acontplus.Services.Policies;
using Acontplus.Services.Services.Abstractions;
using Acontplus.Services.Services.Implementations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace Acontplus.Services.Extensions;

/// <summary>
/// Extension methods for registering enterprise service patterns and configurations.
/// </summary>
public static class EnterpriseServiceExtensions
{
    /// <summary>
    /// Registers all enterprise service patterns including services, filters, and policies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddEnterpriseServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register core services
        services.AddHttpContextAccessor();

        // Register enterprise service implementations
        services.AddScoped<IRequestContextService, RequestContextService>();
        services.AddScoped<ISecurityHeaderService, SecurityHeaderService>();
        services.AddScoped<IDeviceDetectionService, DeviceDetectionService>();

        // Register action filters
        services.AddScoped<ValidationActionFilter>();
        services.AddScoped<RequestLoggingActionFilter>();
        services.AddScoped<SecurityHeaderActionFilter>();

        // Configure request context
        services.Configure<RequestContextConfiguration>(
            configuration.GetSection("RequestContext"));

        return services;
    }

    /// <summary>
    /// Registers enterprise authorization policies for multi-tenant and device-aware scenarios.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="allowedClientIds">Optional list of allowed client IDs.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddEnterpriseAuthorizationPolicies(
        this IServiceCollection services,
        List<string>? allowedClientIds = null)
    {
        services.AddClientIdAuthorization(allowedClientIds);
        services.AddTenantIsolationAuthorization();
        services.AddDeviceTypeAuthorization();

        return services;
    }

    /// <summary>
    /// Configures enterprise middleware pipeline with proper ordering for security and context management.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="environment">The web host environment.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseEnterpriseMiddleware(
        this IApplicationBuilder app,
        IWebHostEnvironment environment)
    {
        // Security headers (early in pipeline)
        app.UseSecurityHeaders(environment);

        // CSP nonce generation
        app.UseMiddleware<CspNonceMiddleware>();

        // Request context and tracking
        app.UseMiddleware<RequestContextMiddleware>();

        // Global exception handling (late in pipeline, before MVC)
        app.UseAcontplusExceptionHandling(options =>
        {
            options.IncludeRequestDetails = true;
            options.LogRequestBody = environment.IsDevelopment();
            options.IncludeDebugDetailsInResponse = environment.IsDevelopment();
        });

        return app;
    }

    /// <summary>
    /// Configures MVC with enterprise filters and JSON serialization options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="enableGlobalFilters">Whether to enable global action filters.</param>
    /// <returns>The MVC builder for further configuration.</returns>
    public static IMvcBuilder AddEnterpriseMvc(
        this IServiceCollection services,
        bool enableGlobalFilters = true)
    {
        var mvcBuilder = services.AddControllers(options =>
        {
            if (enableGlobalFilters)
            {
                // Add global filters in order of execution
                options.Filters.Add<SecurityHeaderActionFilter>();
                options.Filters.Add<RequestLoggingActionFilter>();
                options.Filters.Add<ValidationActionFilter>();
            }
        });

        // Configure JSON options
        JsonConfigurationService.ConfigureAspNetCore(services);

        return mvcBuilder;
    }

    /// <summary>
    /// Adds comprehensive health checks for enterprise applications.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddEnterpriseHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAdvancedHealthChecks(configuration);

        // Add custom health checks for enterprise services
        services.AddHealthChecks()
            .AddCheck<RequestContextHealthCheck>("request-context")
            .AddCheck<SecurityHeaderHealthCheck>("security-headers")
            .AddCheck<DeviceDetectionHealthCheck>("device-detection");

        return services;
    }

    /// <summary>
    /// Configures API explorer for documentation tools (use with your ApiDocumentation project).
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddEnterpriseApiExplorer(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        return services;
    }
}

// Health check implementations for enterprise services
public class RequestContextHealthCheck : IHealthCheck
{
    private readonly IRequestContextService _requestContextService;

    public RequestContextHealthCheck(IRequestContextService requestContextService)
    {
        _requestContextService = requestContextService;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Basic service availability check
            var contextData = _requestContextService.GetRequestContext();
            return Task.FromResult(HealthCheckResult.Healthy("Request context service is operational",
                contextData.Where(kvp => kvp.Value != null).ToDictionary(kvp => kvp.Key, kvp => kvp.Value!)));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Request context service failed", ex));
        }
    }
}

public class SecurityHeaderHealthCheck : IHealthCheck
{
    private readonly ISecurityHeaderService _securityHeaderService;

    public SecurityHeaderHealthCheck(ISecurityHeaderService securityHeaderService)
    {
        _securityHeaderService = securityHeaderService;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var headers = _securityHeaderService.GetRecommendedHeaders(false);
            return Task.FromResult(HealthCheckResult.Healthy("Security header service is operational",
                new Dictionary<string, object> { ["headerCount"] = headers.Count }));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Security header service failed", ex));
        }
    }
}

public class DeviceDetectionHealthCheck : IHealthCheck
{
    private readonly IDeviceDetectionService _deviceDetectionService;

    public DeviceDetectionHealthCheck(IDeviceDetectionService deviceDetectionService)
    {
        _deviceDetectionService = deviceDetectionService;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var capabilities = _deviceDetectionService.GetDeviceCapabilities("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
            return Task.FromResult(HealthCheckResult.Healthy("Device detection service is operational",
                new Dictionary<string, object> { ["testCapabilities"] = capabilities }));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Device detection service failed", ex));
        }
    }
}