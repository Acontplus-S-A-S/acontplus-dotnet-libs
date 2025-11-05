using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Acontplus.Reports.Extensions;

/// <summary>
/// Extension methods for registering report services in the dependency injection container
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds report generation services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration instance</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddReportServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure options from configuration
        services.Configure<Configuration.ReportOptions>(configuration.GetSection("Reports"));

        // Register services
        services.TryAddScoped<IRdlcReportService, Services.RdlcReportService>();
        services.TryAddScoped<IRdlcPrinterService, Services.RdlcPrinterService>();

        return services;
    }

    /// <summary>
    /// Adds report generation services with custom configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureOptions">Action to configure report options</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddReportServices(
        this IServiceCollection services,
        Action<Configuration.ReportOptions> configureOptions)
    {
        // Configure options programmatically
        services.Configure(configureOptions);

        // Register services
        services.TryAddScoped<IRdlcReportService, Services.RdlcReportService>();
        services.TryAddScoped<IRdlcPrinterService, Services.RdlcPrinterService>();

        return services;
    }
}
