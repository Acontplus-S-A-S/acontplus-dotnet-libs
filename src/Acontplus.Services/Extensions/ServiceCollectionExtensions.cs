using Acontplus.Services.Extensions.Infrastructure;
using Microsoft.AspNetCore.Hosting;

namespace Acontplus.Services.Extensions;

/// <summary>
/// Consolidated extension methods for registering all Acontplus.Services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all Acontplus.Services including infrastructure, security, and application services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAcontplusServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add infrastructure services
        services.AddResponseCompressionServices();
        services.AddBasicRateLimiting();

        // Add core services
        services.AddCachingServices(configuration);
        services.AddResilienceServices(configuration);
        services.AddResilientHttpClients(configuration);
        services.AddMonitoringServices(configuration);

        // Add application services
        services.AddApplicationServices(configuration);
        services.AddAuthorizationPolicies();
        services.AddServiceHealthChecks(configuration);
        services.AddApplicationHealthChecks(configuration);

        return services;
    }

    /// <summary>
    /// Configures the complete Acontplus.Services middleware pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="environment">The web host environment.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseAcontplusServices(
        this IApplicationBuilder app,
        IWebHostEnvironment environment)
    {
        // Use application middleware (includes security, rate limiting, context)
        app.UseApplicationMiddleware(environment);

        return app;
    }

    /// <summary>
    /// Configures MVC with all Acontplus.Services filters and options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="enableGlobalFilters">Whether to enable global action filters.</param>
    /// <returns>The MVC builder for further configuration.</returns>
    public static IMvcBuilder AddAcontplusMvc(
        this IServiceCollection services,
        bool enableGlobalFilters = true)
    {
        return services.AddApplicationMvc(enableGlobalFilters);
    }

    /// <summary>
    /// Adds API explorer and documentation support.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAcontplusApiExplorer(
        this IServiceCollection services)
    {
        return services.AddApiExplorer();
    }
}
