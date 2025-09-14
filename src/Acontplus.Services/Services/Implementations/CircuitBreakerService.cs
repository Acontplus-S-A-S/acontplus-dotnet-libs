namespace Acontplus.Services.Services.Implementations;

/// <summary>
/// Circuit breaker service implementation using Polly policies.
/// </summary>
public class CircuitBreakerService : ICircuitBreakerService
{
    private readonly ILogger<CircuitBreakerService> _logger;
    private readonly ResilienceConfiguration _config;
    private readonly Dictionary<string, IAsyncPolicy> _policies;
    private readonly Dictionary<string, CircuitBreakerState> _circuitStates;

    public CircuitBreakerService(
        ILogger<CircuitBreakerService> logger,
        IOptions<ResilienceConfiguration> config)
    {
        _logger = logger;
        _config = config.Value;
        _policies = new Dictionary<string, IAsyncPolicy>();
        _circuitStates = new Dictionary<string, CircuitBreakerState>();

        InitializePolicies();
    }

    public async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action, string? policyName = null)
    {
        var policy = GetPolicy(policyName);
        return await policy.ExecuteAsync(action);
    }

    public async Task ExecuteAsync(Func<Task> action, string? policyName = null)
    {
        var policy = GetPolicy(policyName);
        await policy.ExecuteAsync(action);
    }

    public TResult Execute<TResult>(Func<TResult> action, string? policyName = null)
    {
        // For sync operations, we'll use a simple retry without circuit breaker
        var retryPolicy = Policy
            .Handle<Exception>()
            .Retry(_config.RetryPolicy.MaxRetries);

        return retryPolicy.Execute(action);
    }

    public void Execute(Action action, string? policyName = null)
    {
        // For sync operations, we'll use a simple retry without circuit breaker
        var retryPolicy = Policy
            .Handle<Exception>()
            .Retry(_config.RetryPolicy.MaxRetries);

        retryPolicy.Execute(action);
    }

    public CircuitBreakerState GetCircuitBreakerState(string policyName = "default")
    {
        return _circuitStates.GetValueOrDefault(policyName, CircuitBreakerState.Closed);
    }

    public void OpenCircuit(string policyName = "default")
    {
        _circuitStates[policyName] = CircuitBreakerState.Open;
        _logger.LogWarning("Circuit breaker manually opened for policy: {PolicyName}", policyName);
    }

    public void CloseCircuit(string policyName = "default")
    {
        _circuitStates[policyName] = CircuitBreakerState.Closed;
        _logger.LogInformation("Circuit breaker manually closed for policy: {PolicyName}", policyName);
    }

    private void InitializePolicies()
    {
        if (!_config.CircuitBreaker.Enabled)
            return;

        // Default policy
        _policies["default"] = CreateDefaultPolicy();

        // API policy - more lenient
        _policies["api"] = CreateApiPolicy();

        // Database policy - strict
        _policies["database"] = CreateDatabasePolicy();

        // External service policy - very strict
        _policies["external"] = CreateExternalServicePolicy();

        // Authentication policy - strict
        _policies["auth"] = CreateAuthPolicy();
    }

    private IAsyncPolicy CreateDefaultPolicy()
    {
        var circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: _config.CircuitBreaker.ExceptionsAllowedBeforeBreaking,
                durationOfBreak: TimeSpan.FromSeconds(_config.CircuitBreaker.DurationOfBreakSeconds),
                onBreak: (exception, duration) =>
                {
                    _circuitStates["default"] = CircuitBreakerState.Open;
                    _logger.LogWarning(exception, "Circuit breaker opened for default policy. Duration: {Duration}", duration);
                },
                onReset: () =>
                {
                    _circuitStates["default"] = CircuitBreakerState.Closed;
                    _logger.LogInformation("Circuit breaker reset for default policy");
                },
                onHalfOpen: () =>
                {
                    _circuitStates["default"] = CircuitBreakerState.HalfOpen;
                    _logger.LogInformation("Circuit breaker half-open for default policy");
                });

        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: _config.RetryPolicy.MaxRetries,
                sleepDurationProvider: retryAttempt =>
                {
                    if (_config.RetryPolicy.ExponentialBackoff)
                    {
                        var delay = TimeSpan.FromSeconds(_config.RetryPolicy.BaseDelaySeconds * Math.Pow(2, retryAttempt - 1));
                        return delay > TimeSpan.FromSeconds(_config.RetryPolicy.MaxDelaySeconds)
                            ? TimeSpan.FromSeconds(_config.RetryPolicy.MaxDelaySeconds)
                            : delay;
                    }
                    return TimeSpan.FromSeconds(_config.RetryPolicy.BaseDelaySeconds);
                },
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(exception, "Retry {RetryCount} after {Delay}ms for default policy", retryCount, timeSpan.TotalMilliseconds);
                });

        var timeoutPolicy = Policy
            .TimeoutAsync(TimeSpan.FromSeconds(_config.Timeout.DefaultTimeoutSeconds));

        return Policy.WrapAsync(circuitBreakerPolicy, retryPolicy, timeoutPolicy);
    }

    private IAsyncPolicy CreateApiPolicy()
    {
        var circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: _config.CircuitBreaker.ExceptionsAllowedBeforeBreaking + 2, // More lenient
                durationOfBreak: TimeSpan.FromSeconds(_config.CircuitBreaker.DurationOfBreakSeconds - 30), // Shorter break
                onBreak: (exception, duration) =>
                {
                    _circuitStates["api"] = CircuitBreakerState.Open;
                    _logger.LogWarning(exception, "Circuit breaker opened for API policy. Duration: {Duration}", duration);
                },
                onReset: () =>
                {
                    _circuitStates["api"] = CircuitBreakerState.Closed;
                    _logger.LogInformation("Circuit breaker reset for API policy");
                },
                onHalfOpen: () =>
                {
                    _circuitStates["api"] = CircuitBreakerState.HalfOpen;
                    _logger.LogInformation("Circuit breaker half-open for API policy");
                });

        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: _config.RetryPolicy.MaxRetries + 1, // More retries
                sleepDurationProvider: retryAttempt =>
                {
                    if (_config.RetryPolicy.ExponentialBackoff)
                    {
                        var delay = TimeSpan.FromSeconds(_config.RetryPolicy.BaseDelaySeconds * Math.Pow(1.5, retryAttempt - 1)); // Less aggressive backoff
                        return delay > TimeSpan.FromSeconds(_config.RetryPolicy.MaxDelaySeconds)
                            ? TimeSpan.FromSeconds(_config.RetryPolicy.MaxDelaySeconds)
                            : delay;
                    }
                    return TimeSpan.FromSeconds(_config.RetryPolicy.BaseDelaySeconds);
                },
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(exception, "Retry {RetryCount} after {Delay}ms for API policy", retryCount, timeSpan.TotalMilliseconds);
                });

        var timeoutPolicy = Policy
            .TimeoutAsync(TimeSpan.FromSeconds(_config.Timeout.DefaultTimeoutSeconds + 30)); // More lenient timeout

        return Policy.WrapAsync(circuitBreakerPolicy, retryPolicy, timeoutPolicy);
    }

    private IAsyncPolicy CreateDatabasePolicy()
    {
        var circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: Math.Max(1, _config.CircuitBreaker.ExceptionsAllowedBeforeBreaking - 1), // Stricter
                durationOfBreak: TimeSpan.FromSeconds(_config.CircuitBreaker.DurationOfBreakSeconds + 60), // Longer break
                onBreak: (exception, duration) =>
                {
                    _circuitStates["database"] = CircuitBreakerState.Open;
                    _logger.LogWarning(exception, "Circuit breaker opened for database policy. Duration: {Duration}", duration);
                },
                onReset: () =>
                {
                    _circuitStates["database"] = CircuitBreakerState.Closed;
                    _logger.LogInformation("Circuit breaker reset for database policy");
                },
                onHalfOpen: () =>
                {
                    _circuitStates["database"] = CircuitBreakerState.HalfOpen;
                    _logger.LogInformation("Circuit breaker half-open for database policy");
                });

        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: Math.Max(1, _config.RetryPolicy.MaxRetries - 1), // Fewer retries
                sleepDurationProvider: retryAttempt =>
                {
                    if (_config.RetryPolicy.ExponentialBackoff)
                    {
                        var delay = TimeSpan.FromSeconds(_config.RetryPolicy.BaseDelaySeconds * Math.Pow(3, retryAttempt - 1)); // More aggressive backoff
                        return delay > TimeSpan.FromSeconds(_config.RetryPolicy.MaxDelaySeconds)
                            ? TimeSpan.FromSeconds(_config.RetryPolicy.MaxDelaySeconds)
                            : delay;
                    }
                    return TimeSpan.FromSeconds(_config.RetryPolicy.BaseDelaySeconds);
                },
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(exception, "Retry {RetryCount} after {Delay}ms for database policy", retryCount, timeSpan.TotalMilliseconds);
                });

        var timeoutPolicy = Policy
            .TimeoutAsync(TimeSpan.FromSeconds(_config.Timeout.DefaultTimeoutSeconds - 15)); // Stricter timeout

        return Policy.WrapAsync(circuitBreakerPolicy, retryPolicy, timeoutPolicy);
    }

    private IAsyncPolicy CreateExternalServicePolicy()
    {
        var circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 1, // Very strict
                durationOfBreak: TimeSpan.FromMinutes(5), // Long break
                onBreak: (exception, duration) =>
                {
                    _circuitStates["external"] = CircuitBreakerState.Open;
                    _logger.LogWarning(exception, "Circuit breaker opened for external service policy. Duration: {Duration}", duration);
                },
                onReset: () =>
                {
                    _circuitStates["external"] = CircuitBreakerState.Closed;
                    _logger.LogInformation("Circuit breaker reset for external service policy");
                },
                onHalfOpen: () =>
                {
                    _circuitStates["external"] = CircuitBreakerState.HalfOpen;
                    _logger.LogInformation("Circuit breaker half-open for external service policy");
                });

        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 2, // Very few retries
                sleepDurationProvider: retryAttempt =>
                {
                    return TimeSpan.FromSeconds(5 * retryAttempt); // Simple linear backoff
                },
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(exception, "Retry {RetryCount} after {Delay}ms for external service policy", retryCount, timeSpan.TotalMilliseconds);
                });

        var timeoutPolicy = Policy
            .TimeoutAsync(TimeSpan.FromSeconds(30)); // Very strict timeout

        return Policy.WrapAsync(circuitBreakerPolicy, retryPolicy, timeoutPolicy);
    }

    private IAsyncPolicy CreateAuthPolicy()
    {
        var circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: Math.Max(1, _config.CircuitBreaker.ExceptionsAllowedBeforeBreaking - 1), // Stricter
                durationOfBreak: TimeSpan.FromSeconds(_config.CircuitBreaker.DurationOfBreakSeconds + 30), // Longer break
                onBreak: (exception, duration) =>
                {
                    _circuitStates["auth"] = CircuitBreakerState.Open;
                    _logger.LogWarning(exception, "Circuit breaker opened for auth policy. Duration: {Duration}", duration);
                },
                onReset: () =>
                {
                    _circuitStates["auth"] = CircuitBreakerState.Closed;
                    _logger.LogInformation("Circuit breaker reset for auth policy");
                },
                onHalfOpen: () =>
                {
                    _circuitStates["auth"] = CircuitBreakerState.HalfOpen;
                    _logger.LogInformation("Circuit breaker half-open for auth policy");
                });

        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: Math.Max(1, _config.RetryPolicy.MaxRetries - 1), // Fewer retries
                sleepDurationProvider: retryAttempt =>
                {
                    if (_config.RetryPolicy.ExponentialBackoff)
                    {
                        var delay = TimeSpan.FromSeconds(_config.RetryPolicy.BaseDelaySeconds * Math.Pow(2.5, retryAttempt - 1)); // Aggressive backoff
                        return delay > TimeSpan.FromSeconds(_config.RetryPolicy.MaxDelaySeconds)
                            ? TimeSpan.FromSeconds(_config.RetryPolicy.MaxDelaySeconds)
                            : delay;
                    }
                    return TimeSpan.FromSeconds(_config.RetryPolicy.BaseDelaySeconds);
                },
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(exception, "Retry {RetryCount} after {Delay}ms for auth policy", retryCount, timeSpan.TotalMilliseconds);
                });

        var timeoutPolicy = Policy
            .TimeoutAsync(TimeSpan.FromSeconds(_config.Timeout.DefaultTimeoutSeconds - 10)); // Stricter timeout

        return Policy.WrapAsync(circuitBreakerPolicy, retryPolicy, timeoutPolicy);
    }

    private IAsyncPolicy GetPolicy(string? policyName)
    {
        var name = policyName ?? "default";
        if (!_policies.ContainsKey(name))
        {
            _logger.LogWarning("Policy {PolicyName} not found, using default", name);
            name = "default";
        }
        return _policies[name];
    }
}
