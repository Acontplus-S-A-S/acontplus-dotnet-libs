namespace Acontplus.Infrastructure.Http;

/// <summary>
/// Factory for creating HTTP clients with resilience patterns (circuit breaker, retry, timeout).
/// </summary>
public class ResilientHttpClientFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ResilientHttpClientFactory> _logger;
    private readonly ResilienceConfiguration _config;

    public ResilientHttpClientFactory(
        IHttpClientFactory httpClientFactory,
        ILogger<ResilientHttpClientFactory> logger,
        IOptions<ResilienceConfiguration> config)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _config = config.Value;
    }

    /// <summary>
    /// Creates an HTTP client with standard resilience policies.
    /// </summary>
    public HttpClient CreateClient(string name = "default")
    {
        return _httpClientFactory.CreateClient(name);
    }

    /// <summary>
    /// Creates an HTTP client with custom timeout.
    /// </summary>
    public HttpClient CreateClientWithTimeout(string name, TimeSpan timeout)
    {
        var client = _httpClientFactory.CreateClient(name);
        client.Timeout = timeout;
        return client;
    }

    /// <summary>
    /// Creates an HTTP client for API calls with appropriate resilience settings.
    /// </summary>
    public HttpClient CreateApiClient()
    {
        return CreateClient("api");
    }

    /// <summary>
    /// Creates an HTTP client for external service calls with strict resilience settings.
    /// </summary>
    public HttpClient CreateExternalClient()
    {
        return CreateClient("external");
    }

    /// <summary>
    /// Creates an HTTP client for long-running operations.
    /// </summary>
    public HttpClient CreateLongRunningClient()
    {
        return CreateClientWithTimeout(
            "long-running",
            TimeSpan.FromSeconds(_config.Timeout.LongRunningTimeoutSeconds));
    }
}
