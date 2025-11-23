using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System.Runtime.Versioning;

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
        
        // Register printer service only on Windows 6.1+
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
        {
            AddPrinterServiceIfSupported(services);
        }

        // Register report definition cache (constructed from configured options).
        // RdlcPrinterService depends on ReportDefinitionCache in its constructor,
        // so we always register one (even if caching is disabled, it won't be used).
        services.AddSingleton<Services.ReportDefinitionCache>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<Configuration.ReportOptions>>().Value;
            var max = Math.Max(1, opts.MaxCachedReportDefinitions);
            var ttl = TimeSpan.FromMinutes(Math.Max(1, opts.CacheTtlMinutes));
            return new Services.ReportDefinitionCache(max, ttl);
        });

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
        
        // Register printer service only on Windows 6.1+
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
        {
            AddPrinterServiceIfSupported(services);
        }

        // Register report definition cache (constructed from configured options).
        services.AddSingleton<Services.ReportDefinitionCache>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<Configuration.ReportOptions>>().Value;
            var max = Math.Max(1, opts.MaxCachedReportDefinitions);
            var ttl = TimeSpan.FromMinutes(Math.Max(1, opts.CacheTtlMinutes));
            return new Services.ReportDefinitionCache(max, ttl);
        });

        return services;
    }

    [SupportedOSPlatform("windows6.1")]
    private static void AddPrinterServiceIfSupported(IServiceCollection services)
    {
        services.TryAddScoped<IRdlcPrinterService, Services.RdlcPrinterService>();
    }
}
