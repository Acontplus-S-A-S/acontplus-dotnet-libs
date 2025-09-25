namespace Acontplus.Services.Services.Implementations;

/// <summary>
/// Configuration for policy creation.
/// </summary>
internal record PolicyConfig
{
    public int CircuitBreakerExceptions { get; init; }
    public int CircuitBreakerDuration { get; init; }
    public int RetryCount { get; init; }
    public int RetryBaseDelay { get; init; }
    public int RetryMaxDelay { get; init; }
    public bool RetryExponentialBackoff { get; init; }
    public double RetryBackoffMultiplier { get; init; } = 2.0;
    public int TimeoutSeconds { get; init; }
}

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
        _policies["default"] = CreatePolicy("default", new PolicyConfig
        {
            CircuitBreakerExceptions = _config.CircuitBreaker.ExceptionsAllowedBeforeBreaking,
            CircuitBreakerDuration = _config.CircuitBreaker.DurationOfBreakSeconds,
            RetryCount = _config.RetryPolicy.MaxRetries,
            RetryBaseDelay = _config.RetryPolicy.BaseDelaySeconds,
            RetryMaxDelay = _config.RetryPolicy.MaxDelaySeconds,
            RetryExponentialBackoff = _config.RetryPolicy.ExponentialBackoff,
            TimeoutSeconds = _config.Timeout.DefaultTimeoutSeconds
        });

        // API policy - more lenient
        _policies["api"] = CreatePolicy("api", new PolicyConfig
        {
            CircuitBreakerExceptions = _config.CircuitBreaker.ExceptionsAllowedBeforeBreaking + 2,
            CircuitBreakerDuration = _config.CircuitBreaker.DurationOfBreakSeconds - 30,
            RetryCount = _config.RetryPolicy.MaxRetries + 1,
            RetryBaseDelay = _config.RetryPolicy.BaseDelaySeconds,
            RetryMaxDelay = _config.RetryPolicy.MaxDelaySeconds,
            RetryExponentialBackoff = _config.RetryPolicy.ExponentialBackoff,
            RetryBackoffMultiplier = 1.5,
            TimeoutSeconds = _config.Timeout.DefaultTimeoutSeconds + 30
        });

        // Database policy - strict
        _policies["database"] = CreatePolicy("database", new PolicyConfig
        {
            CircuitBreakerExceptions = Math.Max(1, _config.CircuitBreaker.ExceptionsAllowedBeforeBreaking - 1),
            CircuitBreakerDuration = _config.CircuitBreaker.DurationOfBreakSeconds + 60,
            RetryCount = Math.Max(1, _config.RetryPolicy.MaxRetries - 1),
            RetryBaseDelay = _config.RetryPolicy.BaseDelaySeconds,
            RetryMaxDelay = _config.RetryPolicy.MaxDelaySeconds,
            RetryExponentialBackoff = _config.RetryPolicy.ExponentialBackoff,
            RetryBackoffMultiplier = 3.0,
            TimeoutSeconds = _config.Timeout.DefaultTimeoutSeconds - 15
        });

        // External service policy - very strict
        _policies["external"] = CreatePolicy("external", new PolicyConfig
        {
            CircuitBreakerExceptions = 1,
            CircuitBreakerDuration = 300, // 5 minutes
            RetryCount = 2,
            RetryBaseDelay = 5,
            RetryMaxDelay = 10,
            RetryExponentialBackoff = false,
            TimeoutSeconds = 30
        });

        // Authentication policy - strict
        _policies["auth"] = CreatePolicy("auth", new PolicyConfig
        {
            CircuitBreakerExceptions = Math.Max(1, _config.CircuitBreaker.ExceptionsAllowedBeforeBreaking - 1),
            CircuitBreakerDuration = _config.CircuitBreaker.DurationOfBreakSeconds + 30,
            RetryCount = Math.Max(1, _config.RetryPolicy.MaxRetries - 1),
            RetryBaseDelay = _config.RetryPolicy.BaseDelaySeconds,
            RetryMaxDelay = _config.RetryPolicy.MaxDelaySeconds,
            RetryExponentialBackoff = _config.RetryPolicy.ExponentialBackoff,
            RetryBackoffMultiplier = 2.5,
            TimeoutSeconds = _config.Timeout.DefaultTimeoutSeconds - 10
        });
    }

    private IAsyncPolicy CreatePolicy(string policyName, PolicyConfig config)
    {
        var circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: config.CircuitBreakerExceptions,
                durationOfBreak: TimeSpan.FromSeconds(config.CircuitBreakerDuration),
                onBreak: (exception, duration) =>
                {
                    _circuitStates[policyName] = CircuitBreakerState.Open;
                    _logger.LogWarning(exception, "Circuit breaker opened for {PolicyName} policy. Duration: {Duration}", policyName, duration);
                },
                onReset: () =>
                {
                    _circuitStates[policyName] = CircuitBreakerState.Closed;
                    _logger.LogInformation("Circuit breaker reset for {PolicyName} policy", policyName);
                },
                onHalfOpen: () =>
                {
                    _circuitStates[policyName] = CircuitBreakerState.HalfOpen;
                    _logger.LogInformation("Circuit breaker half-open for {PolicyName} policy", policyName);
                });

        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: config.RetryCount,
                sleepDurationProvider: retryAttempt =>
                {
                    if (config.RetryExponentialBackoff)
                    {
                        var delay = TimeSpan.FromSeconds(config.RetryBaseDelay * Math.Pow(config.RetryBackoffMultiplier, retryAttempt - 1));
                        return delay > TimeSpan.FromSeconds(config.RetryMaxDelay)
                            ? TimeSpan.FromSeconds(config.RetryMaxDelay)
                            : delay;
                    }
                    return TimeSpan.FromSeconds(config.RetryBaseDelay * retryAttempt);
                },
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(exception, "Retry {RetryCount} after {Delay}ms for {PolicyName} policy", retryCount, timeSpan.TotalMilliseconds, policyName);
                });

        var timeoutPolicy = Policy
            .TimeoutAsync(TimeSpan.FromSeconds(config.TimeoutSeconds));

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
