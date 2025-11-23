namespace Acontplus.Infrastructure.Resilience;

/// <summary>
/// Retry policy service providing retry patterns for operations.
/// </summary>
public class RetryPolicyService
{
    private readonly ILogger<RetryPolicyService> _logger;
    private readonly ResilienceConfiguration _config;

    public RetryPolicyService(
        ILogger<RetryPolicyService> _logger,
        IOptions<ResilienceConfiguration> config)
    {
        this._logger = _logger;
        _config = config.Value;
    }

    /// <summary>
    /// Executes an async action with retry policy.
    /// </summary>
    public async Task<TResult> ExecuteAsync<TResult>(
        Func<Task<TResult>> action,
        int? maxRetries = null,
        TimeSpan? baseDelay = null)
    {
        var retries = maxRetries ?? _config.RetryPolicy.MaxRetries;
        var delay = baseDelay ?? TimeSpan.FromSeconds(_config.RetryPolicy.BaseDelaySeconds);

        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: retries,
                sleepDurationProvider: retryAttempt =>
                {
                    if (_config.RetryPolicy.ExponentialBackoff)
                    {
                        var calculatedDelay = TimeSpan.FromSeconds(
                            _config.RetryPolicy.BaseDelaySeconds * Math.Pow(2, retryAttempt - 1));

                        return calculatedDelay > TimeSpan.FromSeconds(_config.RetryPolicy.MaxDelaySeconds)
                            ? TimeSpan.FromSeconds(_config.RetryPolicy.MaxDelaySeconds)
                            : calculatedDelay;
                    }
                    return delay;
                },
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(
                        exception,
                        "Retry {RetryCount} after {Delay}ms",
                        retryCount,
                        timeSpan.TotalMilliseconds);
                });

        return await retryPolicy.ExecuteAsync(action);
    }

    /// <summary>
    /// Executes an async action with retry policy.
    /// </summary>
    public async Task ExecuteAsync(
        Func<Task> action,
        int? maxRetries = null,
        TimeSpan? baseDelay = null)
    {
        var retries = maxRetries ?? _config.RetryPolicy.MaxRetries;
        var delay = baseDelay ?? TimeSpan.FromSeconds(_config.RetryPolicy.BaseDelaySeconds);

        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: retries,
                sleepDurationProvider: retryAttempt =>
                {
                    if (_config.RetryPolicy.ExponentialBackoff)
                    {
                        var calculatedDelay = TimeSpan.FromSeconds(
                            _config.RetryPolicy.BaseDelaySeconds * Math.Pow(2, retryAttempt - 1));

                        return calculatedDelay > TimeSpan.FromSeconds(_config.RetryPolicy.MaxDelaySeconds)
                            ? TimeSpan.FromSeconds(_config.RetryPolicy.MaxDelaySeconds)
                            : calculatedDelay;
                    }
                    return delay;
                },
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(
                        exception,
                        "Retry {RetryCount} after {Delay}ms",
                        retryCount,
                        timeSpan.TotalMilliseconds);
                });

        await retryPolicy.ExecuteAsync(action);
    }

    /// <summary>
    /// Executes a sync action with retry policy.
    /// </summary>
    public TResult Execute<TResult>(
        Func<TResult> action,
        int? maxRetries = null)
    {
        var retries = maxRetries ?? _config.RetryPolicy.MaxRetries;

        var retryPolicy = Policy
            .Handle<Exception>()
            .Retry(
                retryCount: retries,
                onRetry: (exception, retryCount) =>
                {
                    _logger.LogWarning(
                        exception,
                        "Retry {RetryCount}",
                        retryCount);
                });

        return retryPolicy.Execute(action);
    }

    /// <summary>
    /// Executes a sync action with retry policy.
    /// </summary>
    public void Execute(
        Action action,
        int? maxRetries = null)
    {
        var retries = maxRetries ?? _config.RetryPolicy.MaxRetries;

        var retryPolicy = Policy
            .Handle<Exception>()
            .Retry(
                retryCount: retries,
                onRetry: (exception, retryCount) =>
                {
                    _logger.LogWarning(
                        exception,
                        "Retry {RetryCount}",
                        retryCount);
                });

        retryPolicy.Execute(action);
    }
}
