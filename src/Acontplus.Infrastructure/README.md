# Acontplus.Infrastructure

[![NuGet](https://img.shields.io/nuget/v/Acontplus.Infrastructure.svg)](https://www.nuget.org/packages/Acontplus.Infrastructure)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

Enterprise-grade infrastructure library providing caching, resilience patterns, HTTP client factory, rate limiting, and health checks for .NET applications. Built with modern .NET features and industry best practices.

> **💡 Application Services**: For authentication, authorization policies, security headers, device detection, and request context, use **[Acontplus.Services](https://www.nuget.org/packages/Acontplus.Services)**

## 🚀 Features

### 🗄️ Caching

- **Unified Interface**: Single `ICacheService` for both in-memory and distributed caching
- **In-Memory Cache**: High-performance memory cache with statistics tracking
- **Distributed Cache**: Redis support for multi-instance deployments
- **Automatic Fallback**: Graceful degradation when cache operations fail
- **Thread-Safe**: Concurrent access patterns with proper locking
- **Statistics**: Cache hit/miss rates and performance metrics (in-memory only)

### 🛡️ Resilience Patterns

- **Circuit Breaker**: Automatic failure detection and recovery using Polly
- **Retry Policies**: Configurable retry with exponential backoff
- **Timeout Policies**: Operation-specific timeout configurations
- **Pre-configured Policies**: Default, API, Database, External, and Auth policies
- **Health Monitoring**: Circuit breaker state tracking and reporting

### 🌐 HTTP Client Resilience

- **Resilient HTTP Clients**: Built-in circuit breaker, retry, and timeout
- **Multiple Configurations**: Default, API, External, and Long-Running clients
- **Standard Resilience Handler**: Uses Microsoft.Extensions.Http.Resilience
- **Factory Pattern**: Easy client creation with appropriate resilience settings

### 🚦 Rate Limiting

- **Advanced Rate Limiting**: IP-based, client-based, and sliding window support
- **Configurable Windows**: Flexible time windows and request limits
- **Multi-Strategy**: Support for fixed and sliding window algorithms
- **Middleware Integration**: Easy integration with ASP.NET Core pipeline

### 🏥 Health Checks

- **Cache Health Check**: Tests read/write/delete operations
- **Circuit Breaker Health Check**: Monitors circuit breaker states
- **Ready/Live Probes**: Kubernetes-compatible health endpoints
- **Detailed Metrics**: Rich health check data for monitoring

## 📦 Installation

### NuGet Package Manager

```bash
Install-Package Acontplus.Infrastructure
```

### .NET CLI

```bash
dotnet add package Acontplus.Infrastructure
```

### PackageReference

```xml
<PackageReference Include="Acontplus.Infrastructure" Version="1.0.0" />
```

## 🎯 Quick Start

### 1. Basic Setup

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add all infrastructure services with one line
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Map health check endpoints
app.MapHealthChecks("/health");

app.Run();
```

### 2. Configuration

Add to your `appsettings.json`:

```json
{
  "Caching": {
    "UseDistributedCache": false,
    "MemoryCacheSizeLimit": 104857600,
    "ExpirationScanFrequencyMinutes": 5
  },
  "Resilience": {
    "CircuitBreaker": {
      "Enabled": true,
      "ExceptionsAllowedBeforeBreaking": 5,
      "DurationOfBreakSeconds": 30,
      "SamplingDurationSeconds": 60,
      "MinimumThroughput": 10
    },
    "RetryPolicy": {
      "Enabled": true,
      "MaxRetries": 3,
      "BaseDelaySeconds": 1,
      "ExponentialBackoff": true,
      "MaxDelaySeconds": 30
    },
    "RateLimiting": {
      "Enabled": true,
      "WindowSeconds": 60,
      "MaxRequestsPerWindow": 100,
      "SlidingWindow": true,
      "ByIpAddress": true,
      "ByClientId": true
    },
    "Timeout": {
      "Enabled": true,
      "DefaultTimeoutSeconds": 30,
      "DatabaseTimeoutSeconds": 60,
      "HttpClientTimeoutSeconds": 30,
      "LongRunningTimeoutSeconds": 300
    }
  }
}
```

### 3. Basic Usage Example

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ICacheService _cache;
    private readonly ICircuitBreakerService _circuitBreaker;
    private readonly ResilientHttpClientFactory _httpFactory;

    public ProductsController(
        ICacheService cache,
        ICircuitBreakerService circuitBreaker,
        ResilientHttpClientFactory httpFactory)
    {
        _cache = cache;
        _circuitBreaker = circuitBreaker;
        _httpFactory = httpFactory;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        // Use caching
        var product = await _cache.GetOrCreateAsync(
            $"product:{id}",
            async () => await FetchProductFromDatabase(id),
            TimeSpan.FromMinutes(15)
        );

        return Ok(product);
    }

    [HttpGet("external/{id}")]
    public async Task<IActionResult> GetExternalData(int id)
    {
        // Use circuit breaker for external calls
        var data = await _circuitBreaker.ExecuteAsync(async () =>
        {
            var client = _httpFactory.CreateExternalClient();
            var response = await client.GetAsync($"https://api.example.com/data/{id}");
            return await response.Content.ReadAsStringAsync();
        }, "external");

        return Ok(data);
    }

    private async Task<Product> FetchProductFromDatabase(int id)
    {
        // Simulate database call
        await Task.Delay(100);
        return new Product { Id = id, Name = $"Product {id}" };
    }
}
```

## 📚 Detailed Usage

### Caching Service

#### In-Memory Caching

```csharp
public class UserService
{
    private readonly ICacheService _cache;

    public UserService(ICacheService cache)
    {
        _cache = cache;
    }

    public async Task<User> GetUserAsync(int userId)
    {
        var cacheKey = $"user:{userId}";

        // Simple get/set
        var cachedUser = await _cache.GetAsync<User>(cacheKey);
        if (cachedUser != null)
            return cachedUser;

        var user = await _repository.GetByIdAsync(userId);
        await _cache.SetAsync(cacheKey, user, TimeSpan.FromMinutes(30));

        return user;
    }

    public async Task<User> GetOrCreateUserAsync(int userId)
    {
        // Factory pattern - only calls factory if not in cache
        return await _cache.GetOrCreateAsync(
            $"user:{userId}",
            async () => await _repository.GetByIdAsync(userId),
            TimeSpan.FromMinutes(30)
        );
    }

    public async Task<CacheStatistics> GetCacheStatsAsync()
    {
        // Get cache statistics (in-memory only)
        return _cache.GetStatistics();
    }
}
```

#### Redis Distributed Caching

```json
{
  "Caching": {
    "UseDistributedCache": true,
    "RedisConnectionString": "localhost:6379,abortConnect=false",
    "RedisInstanceName": "myapp:"
  }
}
```

```csharp
// Same code works with Redis - just change configuration!
var user = await _cache.GetOrCreateAsync(
    $"user:{userId}",
    async () => await _repository.GetByIdAsync(userId),
    TimeSpan.FromMinutes(30)
);
```

### Circuit Breaker Service

#### Pre-configured Policies

```csharp
public class ExternalApiService
{
    private readonly ICircuitBreakerService _circuitBreaker;

    // Default policy: 5 failures, 30s break
    public async Task<Data> CallDefaultAsync()
    {
        return await _circuitBreaker.ExecuteAsync(
            async () => await MakeApiCall(),
            "default"
        );
    }

    // API policy: More lenient (7 failures, 60s break)
    public async Task<Data> CallApiAsync()
    {
        return await _circuitBreaker.ExecuteAsync(
            async () => await MakeApiCall(),
            "api"
        );
    }

    // Database policy: Strict (4 failures, 90s break)
    public async Task<Data> CallDatabaseAsync()
    {
        return await _circuitBreaker.ExecuteAsync(
            async () => await QueryDatabase(),
            "database"
        );
    }

    // External policy: Very strict (1 failure, 300s break)
    public async Task<Data> CallExternalAsync()
    {
        return await _circuitBreaker.ExecuteAsync(
            async () => await CallThirdPartyApi(),
            "external"
        );
    }

    // Auth policy: Strict (4 failures, 60s break)
    public async Task<AuthResult> AuthenticateAsync()
    {
        return await _circuitBreaker.ExecuteAsync(
            async () => await ValidateToken(),
            "auth"
        );
    }

    // Check circuit breaker state
    public CircuitBreakerState GetStatus(string policy = "default")
    {
        return _circuitBreaker.GetCircuitBreakerState(policy);
        // Returns: Closed, Open, or HalfOpen
    }
}
```

### Retry Policy Service

```csharp
public class OrderService
{
    private readonly RetryPolicyService _retryPolicy;

    // Async with default retry settings
    public async Task<Order> CreateOrderAsync(Order order)
    {
        return await _retryPolicy.ExecuteAsync(
            async () => await _repository.CreateAsync(order)
        );
    }

    // Async with custom retry settings
    public async Task<Order> CreateOrderWithCustomRetryAsync(Order order)
    {
        return await _retryPolicy.ExecuteAsync(
            async () => await _repository.CreateAsync(order),
            maxRetries: 5,
            baseDelay: TimeSpan.FromSeconds(2)
        );
    }

    // Synchronous retry
    public Order CreateOrderSync(Order order)
    {
        return _retryPolicy.Execute(
            () => _repository.Create(order),
            maxRetries: 3
        );
    }
}
```

### Resilient HTTP Client Factory

```csharp
public class ApiIntegrationService
{
    private readonly ResilientHttpClientFactory _httpFactory;

    public ApiIntegrationService(ResilientHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    // Use default client (30s timeout, standard resilience)
    public async Task<string> CallApiAsync()
    {
        var client = _httpFactory.CreateClient();
        var response = await client.GetAsync("https://api.example.com/data");
        return await response.Content.ReadAsStringAsync();
    }

    // Use API client (30s timeout, lenient resilience)
    public async Task<string> CallApiWithLenientPolicyAsync()
    {
        var client = _httpFactory.CreateApiClient();
        var response = await client.GetAsync("https://api.example.com/data");
        return await response.Content.ReadAsStringAsync();
    }

    // Use external client (30s timeout, strict resilience)
    public async Task<string> CallExternalApiAsync()
    {
        var client = _httpFactory.CreateExternalClient();
        var response = await client.GetAsync("https://external.api.com/data");
        return await response.Content.ReadAsStringAsync();
    }

    // Use long-running client (300s timeout)
    public async Task<string> ProcessLongRunningAsync()
    {
        var client = _httpFactory.CreateLongRunningClient();
        var response = await client.GetAsync("https://api.example.com/process");
        return await response.Content.ReadAsStringAsync();
    }

    // Use custom timeout
    public async Task<string> CallWithCustomTimeoutAsync()
    {
        var client = _httpFactory.CreateClientWithTimeout(
            "custom",
            TimeSpan.FromMinutes(5)
        );
        var response = await client.GetAsync("https://api.example.com/data");
        return await response.Content.ReadAsStringAsync();
    }
}
```

### Health Checks

```csharp
// Program.cs
var app = builder.Build();

// Map health check endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});
```

Health check response example:

```json
{
  "status": "Healthy",
  "results": {
    "cache": {
      "status": "Healthy",
      "description": "Cache service is fully operational",
      "data": {
        "totalEntries": 42,
        "hitRatePercentage": 87.5,
        "lastTestTime": "2024-11-23T10:30:00Z"
      }
    },
    "circuit-breaker": {
      "status": "Healthy",
      "description": "All circuit breakers are operational",
      "data": {
        "default": "Closed",
        "api": "Closed",
        "database": "Closed",
        "external": "Closed",
        "auth": "Closed"
      }
    }
  }
}
```

## ⚙️ Configuration Reference

### Complete Configuration Example

```json
{
  "Caching": {
    "UseDistributedCache": false,
    "RedisConnectionString": "localhost:6379,abortConnect=false",
    "RedisInstanceName": "myapp:",
    "MemoryCacheSizeLimit": 104857600,
    "ExpirationScanFrequencyMinutes": 5
  },
  "Resilience": {
    "RateLimiting": {
      "Enabled": true,
      "WindowSeconds": 60,
      "MaxRequestsPerWindow": 100,
      "SlidingWindow": true,
      "ByIpAddress": true,
      "ByUserId": false,
      "ByClientId": true
    },
    "CircuitBreaker": {
      "Enabled": true,
      "ExceptionsAllowedBeforeBreaking": 5,
      "DurationOfBreakSeconds": 30,
      "SamplingDurationSeconds": 60,
      "MinimumThroughput": 10
    },
    "RetryPolicy": {
      "Enabled": true,
      "MaxRetries": 3,
      "BaseDelaySeconds": 1,
      "ExponentialBackoff": true,
      "MaxDelaySeconds": 30
    },
    "Timeout": {
      "Enabled": true,
      "DefaultTimeoutSeconds": 30,
      "DatabaseTimeoutSeconds": 60,
      "HttpClientTimeoutSeconds": 30,
      "LongRunningTimeoutSeconds": 300
    }
  }
}
```

## 🏗️ Architecture

### Folder Structure

```
Acontplus.Infrastructure/
├── Caching/
│   ├── MemoryCacheService.cs          # In-memory cache implementation
│   └── DistributedCacheService.cs     # Redis distributed cache implementation
├── Resilience/
│   ├── CircuitBreakerService.cs       # Circuit breaker with pre-configured policies
│   └── RetryPolicyService.cs          # Retry policy service
├── Http/
│   └── ResilientHttpClientFactory.cs  # HTTP client factory with resilience
├── Middleware/
│   └── RateLimitingMiddleware.cs      # Advanced rate limiting middleware
├── HealthChecks/
│   ├── CacheHealthCheck.cs            # Cache service health check
│   └── CircuitBreakerHealthCheck.cs   # Circuit breaker health check
├── Configuration/
│   ├── CacheConfiguration.cs          # Cache configuration options
│   └── ResilienceConfiguration.cs     # Resilience configuration options
└── Extensions/
    └── InfrastructureServiceExtensions.cs  # DI registration extensions
```

### Dependencies

- **Polly**: Resilience and transient-fault-handling library
- **Microsoft.Extensions.Caching.StackExchangeRedis**: Redis cache provider
- **Microsoft.Extensions.Http.Resilience**: HTTP resilience patterns

## 🔧 Granular Setup

If you need fine-grained control, register services individually:

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Register only caching services
builder.Services.AddCachingServices(builder.Configuration);

// Register only resilience services
builder.Services.AddResilienceServices(builder.Configuration);

// Register only HTTP client factory
builder.Services.AddResilientHttpClients(builder.Configuration);

// Register only health checks
builder.Services.AddInfrastructureHealthChecks();

var app = builder.Build();

// Use rate limiting middleware if needed
app.UseMiddleware<RateLimitingMiddleware>();

app.Run();
```

## 📊 Monitoring & Observability

### Cache Metrics

```csharp
public class MonitoringService
{
    private readonly ICacheService _cache;

    public CacheMetrics GetCacheMetrics()
    {
        var stats = _cache.GetStatistics();
        return new CacheMetrics
        {
            TotalEntries = stats.TotalEntries,
            HitRate = stats.HitRatePercentage,
            MissRate = stats.MissRatePercentage
        };
    }
}
```

### Circuit Breaker Monitoring

```csharp
public class ResilienceMonitoringService
{
    private readonly ICircuitBreakerService _circuitBreaker;

    public Dictionary<string, string> GetCircuitBreakerStates()
    {
        var policies = new[] { "default", "api", "database", "external", "auth" };
        return policies.ToDictionary(
            p => p,
            p => _circuitBreaker.GetCircuitBreakerState(p).ToString()
        );
    }
}
```

## 🧪 Testing

### Unit Testing with Mocks

```csharp
public class ProductServiceTests
{
    private readonly Mock<ICacheService> _mockCache;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _mockCache = new Mock<ICacheService>();
        _service = new ProductService(_mockCache.Object);
    }

    [Fact]
    public async Task GetProduct_CacheHit_ReturnsCachedValue()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test" };
        _mockCache
            .Setup(x => x.GetAsync<Product>("product:1", default))
            .ReturnsAsync(product);

        // Act
        var result = await _service.GetProductAsync(1);

        // Assert
        Assert.Equal(product, result);
        _mockCache.Verify(x => x.GetAsync<Product>("product:1", default), Times.Once);
    }
}
```

## 📄 License

MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📞 Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/acontplus/acontplus-dotnet-libs).
