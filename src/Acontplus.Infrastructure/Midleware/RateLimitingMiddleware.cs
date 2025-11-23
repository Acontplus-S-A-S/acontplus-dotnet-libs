namespace Acontplus.Infrastructure.Midleware;

/// <summary>
/// Advanced rate limiting middleware using .NET 9's built-in rate limiting.
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly ResilienceConfiguration _config;

    public RateLimitingMiddleware(
        RequestDelegate next,
        ILogger<RateLimitingMiddleware> logger,
        IOptions<ResilienceConfiguration> config)
    {
        _next = next;
        _logger = logger;
        _config = config.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!_config.RateLimiting.Enabled)
        {
            await _next(context);
            return;
        }

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in rate limiting middleware");
            throw;
        }
    }
}

/// <summary>
/// Extension methods for configuring rate limiting.
/// </summary>
public static class RateLimitingExtensions
{
    /// <summary>
    /// Configure rate limiting with advanced patterns.
    /// </summary>
    public static IServiceCollection AddAdvancedRateLimiting(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var resilienceConfig = configuration.GetSection("Resilience").Get<ResilienceConfiguration>()
            ?? new ResilienceConfiguration();

        if (!resilienceConfig.RateLimiting.Enabled)
            return services;

        // Configure rate limiting policies
        services.AddRateLimiter(options =>
        {
            // Global rate limiting
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var key = GetRateLimitKey(context, resilienceConfig.RateLimiting);
                return RateLimitPartition.GetFixedWindowLimiter(key, _ =>
                    new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = resilienceConfig.RateLimiting.MaxRequestsPerWindow,
                        Window = TimeSpan.FromSeconds(resilienceConfig.RateLimiting.WindowSeconds),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    });
            });

            // Configure rate limiting responses
            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.ContentType = "application/json";

                var response = new
                {
                    error = "Too many requests",
                    message = "Rate limit exceeded. Please try again later.",
                    retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter)
                        ? retryAfter.TotalSeconds
                        : resilienceConfig.RateLimiting.WindowSeconds
                };

                await context.HttpContext.Response.WriteAsJsonAsync(response, token);
            };

            // Add specific policies for different endpoints
            AddEndpointSpecificPolicies(options, resilienceConfig);
        });

        return services;
    }

    /// <summary>
    /// Use advanced rate limiting middleware.
    /// </summary>
    public static IApplicationBuilder UseAdvancedRateLimiting(this IApplicationBuilder app)
    {
        return app.UseRateLimiter();
    }

    private static string GetRateLimitKey(HttpContext context, RateLimitingOptions options)
    {
        var keys = new List<string>();

        if (options.ByIpAddress)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            keys.Add($"ip:{ip}");
        }

        if (options.ByClientId)
        {
            var clientId = context.Request.Headers["Client-Id"].FirstOrDefault() ?? "anonymous";
            keys.Add($"client:{clientId}");
        }

        if (options.ByUserId && context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst("sub")?.Value ?? context.User.Identity.Name ?? "unknown";
            keys.Add($"user:{userId}");
        }

        return string.Join("|", keys);
    }

    private static void AddEndpointSpecificPolicies(RateLimiterOptions options, ResilienceConfiguration config)
    {
        // API endpoints - stricter limits
        options.AddPolicy<string>("api", context =>
        {
            var key = GetRateLimitKey(context, config.RateLimiting);
            return RateLimitPartition.GetFixedWindowLimiter(key, _ =>
                new FixedWindowRateLimiterOptions
                {
                    PermitLimit = config.RateLimiting.MaxRequestsPerWindow / 2, // Stricter for APIs
                    Window = TimeSpan.FromSeconds(config.RateLimiting.WindowSeconds),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });
        });

        // Authentication endpoints - very strict limits
        options.AddPolicy<string>("auth", context =>
        {
            var key = GetRateLimitKey(context, config.RateLimiting);
            return RateLimitPartition.GetFixedWindowLimiter(key, _ =>
                new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5, // Very strict for auth
                    Window = TimeSpan.FromMinutes(5),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });
        });
    }
}
