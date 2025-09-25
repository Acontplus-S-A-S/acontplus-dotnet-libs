using Acontplus.Services.Extensions.Infrastructure;

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
        services.AddServiceHealthChecks(configuration);

        // Add application services
        services.AddApplicationServices(configuration);
        services.AddAuthorizationPolicies();
        services.AddApplicationHealthChecks(configuration);

        return services;
    }



}
