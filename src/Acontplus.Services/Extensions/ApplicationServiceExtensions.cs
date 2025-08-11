using Acontplus.Services.Configuration;
using Acontplus.Services.Extensions.Middleware;
using Acontplus.Services.Extensions.Security;
using Acontplus.Services.Filters;
using Acontplus.Services.Middleware;
using Acontplus.Services.Policies;
using Acontplus.Services.Services.Abstractions;
using Acontplus.Services.Services.Implementations;
using Microsoft.AspNetCore.Hosting;

namespace Acontplus.Services.Extensions;

/// <summary>
/// Extension methods for registering application services, filters, and policies.
/// </summary>
public static class ApplicationServiceExtensions
{
    /// <summary>
    /// Registers all application services including core services, filters, and policies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register core services
        services.AddHttpContextAccessor();

        // Register service implementations
        services.AddScoped<IRequestContextService, RequestContextService>();
        services.AddScoped<ISecurityHeaderService, SecurityHeaderService>();
        services.AddScoped<IDeviceDetectionService, DeviceDetectionService>();
        services.AddScoped<ICircuitBreakerService, CircuitBreakerService>();

        // Register action filters
        services.AddScoped<ValidationActionFilter>();
        services.AddScoped<RequestLoggingActionFilter>();
        services.AddScoped<SecurityHeaderActionFilter>();

        // Configure request context
        services.Configure<RequestContextConfiguration>(
            configuration.GetSection("RequestContext"));

        // Configure resilience services
        services.Configure<ResilienceConfiguration>(
            configuration.GetSection("Resilience"));

        return services;
    }

    /// <summary>
    /// Registers authorization policies for multi-tenant and device-aware scenarios.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="allowedClientIds">Optional list of allowed client IDs.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAuthorizationPolicies(
        this IServiceCollection services,
        List<string>? allowedClientIds = null)
    {
        services.AddClientIdAuthorization(allowedClientIds);
        services.AddTenantIsolationAuthorization();
        services.AddDeviceTypeAuthorization();

        return services;
    }

    /// <summary>
    /// Configures application middleware pipeline with proper ordering for security and context management.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="environment">The web host environment.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseApplicationMiddleware(
        this IApplicationBuilder app,
        IWebHostEnvironment environment)
    {
        // Security headers (early in pipeline)
        app.UseSecurityHeaders(environment);

        // CSP nonce generation
        app.UseMiddleware<CspNonceMiddleware>();

        // Advanced rate limiting
        app.UseAdvancedRateLimiting();

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
    /// Configures MVC with application filters and JSON serialization options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="enableGlobalFilters">Whether to enable global action filters.</param>
    /// <returns>The MVC builder for further configuration.</returns>
    public static IMvcBuilder AddApplicationMvc(
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
    /// Adds comprehensive health checks for application services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddApplicationHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add custom health checks for application services
        services.AddHealthChecks()
            .AddCheck<RequestContextHealthCheck>("request-context")
            .AddCheck<SecurityHeaderHealthCheck>("security-headers")
            .AddCheck<DeviceDetectionHealthCheck>("device-detection");

        return services;
    }

    /// <summary>
    /// Configures API explorer for documentation tools.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddApiExplorer(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        return services;
    }
}

// Health check implementations for application services
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
            // Test core functionality of request context service
            var contextData = _requestContextService.GetRequestContext();

            // Verify essential context data is available
            var hasRequestId = !string.IsNullOrEmpty(contextData.GetValueOrDefault("requestId")?.ToString());
            var hasCorrelationId = !string.IsNullOrEmpty(contextData.GetValueOrDefault("correlationId")?.ToString());
            var hasTimestamp = contextData.ContainsKey("timestamp");

            var healthData = contextData.Where(kvp => kvp.Value != null)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value!);

            healthData["hasRequestId"] = hasRequestId;
            healthData["hasCorrelationId"] = hasCorrelationId;
            healthData["hasTimestamp"] = hasTimestamp;
            healthData["lastCheckTime"] = DateTime.UtcNow;

            if (hasRequestId && hasCorrelationId && hasTimestamp)
            {
                return Task.FromResult(HealthCheckResult.Healthy("Request context service is fully operational", healthData));
            }

            return Task.FromResult(HealthCheckResult.Degraded("Request context service is partially operational", data: healthData));
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("HTTP context is not available"))
        {
            // This is expected when health check runs outside of HTTP context
            var data = new Dictionary<string, object>
            {
                ["status"] = "No HTTP context available (expected during startup)",
                ["lastCheckTime"] = DateTime.UtcNow
            };
            return Task.FromResult(HealthCheckResult.Healthy("Request context service is available", data));
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
            // Test with multiple user agents to verify detection accuracy
            var testCases = new[]
            {
                ("Desktop Chrome", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36", DeviceType.Desktop),
                ("Mobile Safari", "Mozilla/5.0 (iPhone; CPU iPhone OS 17_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.0 Mobile/15E148 Safari/604.1", DeviceType.Mobile),
                ("iPad", "Mozilla/5.0 (iPad; CPU OS 17_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.0 Mobile/15E148 Safari/604.1", DeviceType.Tablet)
            };

            var results = new Dictionary<string, object>();
            var allTestsPassed = true;

            foreach (var (name, userAgent, expectedType) in testCases)
            {
                var capabilities = _deviceDetectionService.GetDeviceCapabilities(userAgent);
                var testPassed = capabilities.Type == expectedType;
                allTestsPassed &= testPassed;

                results[$"test_{name.Replace(" ", "_").ToLower()}"] = new
                {
                    Expected = expectedType.ToString(),
                    Actual = capabilities.Type.ToString(),
                    Passed = testPassed,
                    Browser = capabilities.Browser,
                    OS = capabilities.OperatingSystem
                };
            }

            results["lastCheckTime"] = DateTime.UtcNow;
            results["allTestsPassed"] = allTestsPassed;

            if (allTestsPassed)
            {
                return Task.FromResult(HealthCheckResult.Healthy("Device detection service is fully operational", results));
            }

            return Task.FromResult(HealthCheckResult.Degraded("Device detection service has some detection issues", data: results));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Device detection service failed", ex));
        }
    }
}
