using Acontplus.Infrastructure.Caching;
using Acontplus.Infrastructure.HealthChecks;
using Acontplus.Infrastructure.Http;
using Acontplus.Infrastructure.Midleware;
using Acontplus.Infrastructure.Resilience;

namespace Acontplus.Infrastructure.Extensions;

/// <summary>
/// Service collection extensions for registering infrastructure services.
/// </summary>
public static class InfrastructureServiceExtensions
{
    /// <summary>
    /// Adds all infrastructure services (caching, resilience, HTTP client factory, health checks).
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add caching services
        services.AddCachingServices(configuration);

        // Add resilience services
        services.AddResilienceServices(configuration);

        // Add HTTP client factory with resilience
        services.AddResilientHttpClients(configuration);

        // Add health checks
        services.AddInfrastructureHealthChecks();

        return services;
    }

    /// <summary>
    /// Adds caching services (in-memory or distributed with Redis).
    /// </summary>
    public static IServiceCollection AddCachingServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var cacheConfig = configuration.GetSection("Caching").Get<CacheConfiguration>()
            ?? new CacheConfiguration();

        services.Configure<CacheConfiguration>(configuration.GetSection("Caching"));

        if (cacheConfig.UseDistributedCache && !string.IsNullOrEmpty(cacheConfig.RedisConnectionString))
        {
            // Register distributed cache (Redis)
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = cacheConfig.RedisConnectionString;
                options.InstanceName = cacheConfig.RedisInstanceName ?? "acontplus:";
            });

            services.AddSingleton<ICacheService, DistributedCacheService>();
        }
        else
        {
            // Register in-memory cache
            services.AddMemoryCache(options =>
            {
                options.SizeLimit = cacheConfig.MemoryCacheSizeLimit;
                options.ExpirationScanFrequency = TimeSpan.FromMinutes(cacheConfig.ExpirationScanFrequencyMinutes);
            });

            services.AddSingleton<ICacheService, MemoryCacheService>();
        }

        return services;
    }

    /// <summary>
    /// Adds resilience services (circuit breaker, retry policies).
    /// </summary>
    public static IServiceCollection AddResilienceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ResilienceConfiguration>(configuration.GetSection("Resilience"));

        services.AddSingleton<ICircuitBreakerService, CircuitBreakerService>();
        services.AddSingleton<RetryPolicyService>();

        return services;
    }

    /// <summary>
    /// Adds HTTP client factory with resilience patterns.
    /// </summary>
    public static IServiceCollection AddResilientHttpClients(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var resilienceConfig = configuration.GetSection("Resilience").Get<ResilienceConfiguration>()
            ?? new ResilienceConfiguration();

        // Register HTTP client factory
        services.AddHttpClient();

        // Register default HTTP client with resilience
        services.AddHttpClient("default")
            .AddStandardResilienceHandler(options =>
            {
                options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(resilienceConfig.CircuitBreaker.SamplingDurationSeconds);
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(resilienceConfig.Timeout.DefaultTimeoutSeconds);
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(resilienceConfig.Timeout.DefaultTimeoutSeconds * 2);
            });

        // Register API HTTP client with more lenient settings
        services.AddHttpClient("api")
            .AddStandardResilienceHandler(options =>
            {
                options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(resilienceConfig.CircuitBreaker.SamplingDurationSeconds);
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(resilienceConfig.Timeout.HttpClientTimeoutSeconds);
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(resilienceConfig.Timeout.HttpClientTimeoutSeconds * 2);
            });

        // Register external HTTP client with strict settings
        services.AddHttpClient("external")
            .AddStandardResilienceHandler(options =>
            {
                options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(60);
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(30);
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(60);
            });

        // Register long-running HTTP client
        services.AddHttpClient("long-running")
            .AddStandardResilienceHandler(options =>
            {
                options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(resilienceConfig.CircuitBreaker.SamplingDurationSeconds);
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(resilienceConfig.Timeout.LongRunningTimeoutSeconds);
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(resilienceConfig.Timeout.LongRunningTimeoutSeconds * 2);
            });

        services.AddSingleton<ResilientHttpClientFactory>();

        return services;
    }

    /// <summary>
    /// Adds health checks for infrastructure services.
    /// </summary>
    public static IServiceCollection AddInfrastructureHealthChecks(
        this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<CacheHealthCheck>(
                "cache",
                tags: new[] { "ready", "cache" })
            .AddCheck<CircuitBreakerHealthCheck>(
                "circuit-breaker",
                tags: new[] { "ready", "resilience" });

        return services;
    }

    /// <summary>
    /// Configures middleware pipeline for infrastructure services.
    /// </summary>
    public static IApplicationBuilder UseInfrastructureMiddleware(
        this IApplicationBuilder app,
        IHostEnvironment environment)
    {
        // Add rate limiting middleware (basic implementation)
        app.UseMiddleware<RateLimitingMiddleware>();

        return app;
    }
}
