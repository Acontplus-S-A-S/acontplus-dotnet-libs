namespace Acontplus.Infrastructure.Configuration;

/// <summary>
///     Configuration for resilience patterns including rate limiting and circuit breakers.
/// </summary>
public class ResilienceConfiguration
{
    /// <summary>
    ///     Rate limiting configuration.
    /// </summary>
    public RateLimitingOptions RateLimiting { get; set; } = new();

    /// <summary>
    ///     Circuit breaker configuration.
    /// </summary>
    public CircuitBreakerOptions CircuitBreaker { get; set; } = new();

    /// <summary>
    ///     Retry policy configuration.
    /// </summary>
    public RetryPolicyOptions RetryPolicy { get; set; } = new();

    /// <summary>
    ///     Timeout configuration.
    /// </summary>
    public TimeoutOptions Timeout { get; set; } = new();
}

/// <summary>
///     Rate limiting configuration options.
/// </summary>
public class RateLimitingOptions
{
    /// <summary>
    ///     Enable rate limiting. Default: true.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    ///     Rate limit window in seconds. Default: 60.
    /// </summary>
    public int WindowSeconds { get; set; } = 60;

    /// <summary>
    ///     Maximum requests per window. Default: 100.
    /// </summary>
    public int MaxRequestsPerWindow { get; set; } = 100;

    /// <summary>
    ///     Enable sliding window. Default: true.
    /// </summary>
    public bool SlidingWindow { get; set; } = true;

    /// <summary>
    ///     Rate limit by IP address. Default: true.
    /// </summary>
    public bool ByIpAddress { get; set; } = true;

    /// <summary>
    ///     Rate limit by user ID. Default: false.
    /// </summary>
    public bool ByUserId { get; set; } = false;

    /// <summary>
    ///     Rate limit by client ID. Default: true.
    /// </summary>
    public bool ByClientId { get; set; } = true;
}

/// <summary>
///     Circuit breaker configuration options.
/// </summary>
public class CircuitBreakerOptions
{
    /// <summary>
    ///     Enable circuit breaker. Default: true.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    ///     Number of exceptions before opening circuit. Default: 5.
    /// </summary>
    public int ExceptionsAllowedBeforeBreaking { get; set; } = 5;

    /// <summary>
    ///     Duration of break in seconds. Default: 30.
    /// </summary>
    public int DurationOfBreakSeconds { get; set; } = 30;

    /// <summary>
    ///     Sampling duration in seconds. Default: 60.
    /// </summary>
    public int SamplingDurationSeconds { get; set; } = 60;

    /// <summary>
    ///     Minimum throughput before circuit can open. Default: 10.
    /// </summary>
    public int MinimumThroughput { get; set; } = 10;
}

/// <summary>
///     Retry policy configuration options.
/// </summary>
public class RetryPolicyOptions
{
    /// <summary>
    ///     Enable retry policy. Default: true.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    ///     Maximum number of retries. Default: 3.
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    ///     Base delay between retries in seconds. Default: 1.
    /// </summary>
    public int BaseDelaySeconds { get; set; } = 1;

    /// <summary>
    ///     Enable exponential backoff. Default: true.
    /// </summary>
    public bool ExponentialBackoff { get; set; } = true;

    /// <summary>
    ///     Maximum delay between retries in seconds. Default: 30.
    /// </summary>
    public int MaxDelaySeconds { get; set; } = 30;
}

/// <summary>
///     Timeout configuration options.
/// </summary>
public class TimeoutOptions
{
    /// <summary>
    ///     Enable timeout policies. Default: true.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    ///     Default timeout in seconds. Default: 30.
    /// </summary>
    public int DefaultTimeoutSeconds { get; set; } = 30;

    /// <summary>
    ///     Database operation timeout in seconds. Default: 60.
    /// </summary>
    public int DatabaseTimeoutSeconds { get; set; } = 60;

    /// <summary>
    ///     HTTP client timeout in seconds. Default: 30.
    /// </summary>
    public int HttpClientTimeoutSeconds { get; set; } = 30;

    /// <summary>
    ///     Long-running operation timeout in seconds. Default: 300.
    /// </summary>
    public int LongRunningTimeoutSeconds { get; set; } = 300;
}
