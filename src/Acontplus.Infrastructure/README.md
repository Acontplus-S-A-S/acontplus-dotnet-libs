# Acontplus.Infrastructure

[![NuGet](https://img.shields.io/nuget/v/Acontplus.Infrastructure.svg)](https://www.nuget.org/packages/Acontplus.Infrastructure)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

Enterprise-grade infrastructure library providing caching, resilience patterns, HTTP client factory, rate limiting,
health checks, and response compression for .NET applications. Built with modern .NET 10 features and industry best
practices.

> **💡 Application Services**: For authentication, authorization policies, security headers, device detection, and
> request context, use **[Acontplus.Services](https://www.nuget.org/packages/Acontplus.Services)**

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

- **Advanced Configuration**: Multi-key rate limiting (IP, Client ID, User ID)
- **Custom Policies**: Pre-configured "api" and "auth" policies
- **Built-in Middleware**: Uses .NET's built-in rate limiting infrastructure
- **Custom Responses**: JSON error responses with retry-after headers
- **Flexible Windows**: Configurable time windows and request limits

### 🏥 Health Checks

- **Cache Health Check**: Tests read/write/delete operations
- **Circuit Breaker Health Check**: Monitors circuit breaker states
- **Ready/Live Probes**: Kubernetes-compatible health endpoints
- **Detailed Metrics**: Rich health check data for monitoring

### 🗜️ Response Compression

- **Brotli & Gzip**: Modern compression algorithms with Brotli preferred
- **Configurable MIME Types**: Customize compressed content types
- **HTTPS Security**: Optional HTTPS-only compression
- **Performance Boost**: Reduce bandwidth and improve response times
- **Client-Aware**: Automatic compression based on client capabilities

### 📡 Application Event Bus (NEW in v1.2.1+)

- **Channel-Based Architecture**: High-performance using `System.Threading.Channels`
- **Async Background Processing**: Non-blocking event handling for cross-cutting concerns
- **Pub/Sub Pattern**: In-memory event publishing and subscribing
- **CQRS Ready**: Perfect for command/query separation with application events
- **Microservices Communication**: Scalable async event processing
- **Multiple Subscribers**: Many background handlers can listen to the same event
- **Thread-Safe**: Concurrent event publishing and consumption
- **Clean Architecture**: Abstractions in Core, implementation in Infrastructure
- **⚠️ Note**: For **transactional domain events** (same transaction, DB ID dependencies), use `IDomainEventDispatcher` from Core

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

// Add all infrastructure services with one line (includes Event Bus)
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Map health check endpoints
app.MapHealthChecks("/health");

app.Run();
```

### 2. With Event Bus

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add infrastructure services
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add in-memory event bus for CQRS/Event-Driven architecture
builder.Services.AddInMemoryEventBus(options =>
{
    options.EnableDiagnosticLogging = true;
});

// Register event handlers as background services
builder.Services.AddHostedService<OrderCreatedHandler>();

var app = builder.Build();
app.Run();
```

### 3. With Rate Limiting

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add infrastructure services
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add rate limiting (optional)
builder.Services.AddAdvancedRateLimiting(builder.Configuration);

var app = builder.Build();

// Use rate limiting middleware
app.UseRateLimiter();

// Map health and controllers
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
```

### 4. Configuration

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
      "ByIpAddress": true,
      "ByClientId": true,
      "ByUserId": false
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

## 🏥 Health Checks (2025+ Modern Approach)

### Unified Health Endpoints with Tags

Acontplus.Infrastructure now provides a single extension to map all health check endpoints with consistent JSON
formatting and tag-based filtering:

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddInfrastructureServices(builder.Configuration);
var app = builder.Build();

// One line to map all health endpoints (with apiName, tags, and full details)
app.MapHealthCheckEndpoints();

app.Run();
```

This creates:

- `/health` (all checks)
- `/health/ready` (checks tagged `ready`)
- `/health/live` (checks tagged `live`)
- `/health/cache` (checks tagged `cache`)
- `/health/resilience` (checks tagged `resilience`)

**Behavior:**

- If no cache or circuit breaker is registered, endpoints still work and return a self-check with the app name.
- If a tag endpoint (like `/health/cache`) has no checks, it returns an empty array but a valid response.
- All responses include `apiName`, `status`, `checks`, and `totalDuration`.

#### Example Response

```json
{
  "apiName": "Acontplus.TestApi",
  "status": "Healthy",
  "checks": [
    {
      "name": "self",
      "status": "Healthy",
      "description": "Acontplus.TestApi is running",
      "data": {
        "application": "Acontplus.TestApi",
        "tags": "live, ready",
        "lastCheckTime": "2025-11-27T12:00:00Z"
      }
    }
  ],
  "totalDuration": "00:00:00.0054321"
}
```

#### Customization

- You can override the base path: `app.MapHealthCheckEndpoints("/myhealth")`
- You can still add custom health checks and tags as before.

#### Migration

- **Old:** Multiple `app.MapHealthChecks` with custom response writers
- **New:** Just call `app.MapHealthCheckEndpoints()` for all endpoints and formatting

See `Extensions/HealthCheckEndpointExtensions.cs` for details.

---

## 🗜️ Response Compression

Optimize API performance with automatic response compression using Brotli and Gzip algorithms. Configurable MIME types
and HTTPS-only compression for modern web applications.

### Setup

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add response compression
builder.Services.AddResponseCompression(builder.Configuration);

var app = builder.Build();

// Use response compression middleware (MUST be before MapControllers)
app.UseResponseCompression();

app.MapControllers();
app.Run();
```

### Configuration

Add to your `appsettings.json`:

```json
{
  "ResponseCompression": {
    "EnableForHttps": true,
    "MimeTypes": [],
    "EnableBrotli": true,
    "BrotliLevel": "Optimal",
    "EnableGzip": true,
    "GzipLevel": "Optimal"
  }
}
```

### Compression Levels

- **Fastest**: Prioritizes speed over compression ratio
- **Optimal**: Balances speed and compression ratio (default)
- **NoCompression**: Disables compression (not recommended)

### Features

- **Dual Compression**: Brotli (preferred) and Gzip support
- **HTTPS Only**: Optional HTTPS-only compression for security
- **Configurable MIME Types**: Customize which content types to compress
- **Default Types**: Automatically includes JSON, XML, CSS, JS, and more
- **Performance Optimized**: Brotli provides better compression ratios

### Default MIME Types

When no custom MIME types are specified, the following are compressed:

- `text/plain`
- `text/css`
- `application/json`
- `application/xml`
- `application/javascript`
- `text/javascript`
- `application/json-patch+json`
- All `text/*` types

### Usage with Infrastructure Services

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add all infrastructure services including response compression
builder.Services.AddInfrastructureServices(builder.Configuration, addResponseCompression: true);

var app = builder.Build();

// Use response compression middleware
app.UseResponseCompression();

app.MapControllers();
app.Run();
```

### Client Support

Modern browsers automatically send `Accept-Encoding: gzip, deflate, br` headers. The middleware responds with compressed
content when supported.

```bash
# Example compressed response
curl -H "Accept-Encoding: gzip" https://api.example.com/data
# Response includes: Content-Encoding: gzip
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

## 🚦 Rate Limiting

### Setup

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Configure rate limiting
builder.Services.AddAdvancedRateLimiting(builder.Configuration);

var app = builder.Build();

// Add rate limiting middleware (MUST be before MapControllers)
app.UseRateLimiter();

app.MapControllers();
app.Run();
```

### Configuration

```json
{
  "Resilience": {
    "RateLimiting": {
      "Enabled": true,
      "WindowSeconds": 60,
      "MaxRequestsPerWindow": 100,
      "ByIpAddress": true,
      "ByClientId": true,
      "ByUserId": false
    }
  }
}
```

### Apply to Specific Endpoints

```csharp
using Microsoft.AspNetCore.RateLimiting;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    // Uses global rate limiting (100 requests per 60 seconds)
    [HttpGet]
    public IActionResult GetAll() => Ok();

    // Uses "api" policy (50 requests per 60 seconds)
    [HttpGet("{id}")]
    [EnableRateLimiting("api")]
    public IActionResult Get(int id) => Ok();

    // Uses "auth" policy (5 requests per 5 minutes)
    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    public IActionResult Login() => Ok();

    // Disable rate limiting for specific endpoint
    [HttpGet("health")]
    [DisableRateLimiting]
    public IActionResult Health() => Ok();
}
```

### Rate Limit Response

When rate limit is exceeded, clients receive:

```json
{
  "error": "Too many requests",
  "message": "Rate limit exceeded. Please try again later.",
  "retryAfter": 60
}
```

HTTP Status: `429 Too Many Requests`

## 🔧 Granular Setup

If you need fine-grained control, register services individually:

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Register only caching services
builder.Services.AddCachingServices(builder.Configuration);

// Register only resilience services (circuit breaker, retry)
builder.Services.AddResilienceServices(builder.Configuration);

// Register only HTTP client factory
builder.Services.AddResilientHttpClients(builder.Configuration);

// Register only health checks
builder.Services.AddInfrastructureHealthChecks();

// Optionally add rate limiting
builder.Services.AddAdvancedRateLimiting(builder.Configuration);

var app = builder.Build();

// Use rate limiting if configured
app.UseRateLimiter();

app.MapControllers();
app.Run();
```

## 📚 API Reference

### Extension Methods

#### Service Registration

```csharp
// Register all infrastructure services
services.AddInfrastructureServices(configuration);

// Or register individually
services.AddCachingServices(configuration);
services.AddResilienceServices(configuration);
services.AddResilientHttpClients(configuration);
services.AddInfrastructureHealthChecks();
services.AddAdvancedRateLimiting(configuration);
```

#### Middleware

```csharp
// Rate limiting middleware (uses .NET's built-in rate limiter)
app.UseRateLimiter();
```

### Core Interfaces

```csharp
// Caching
ICacheService
  - GetAsync<T>(string key, CancellationToken cancellationToken = default)
  - SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null, CancellationToken cancellationToken = default)
  - GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? absoluteExpiration = null, CancellationToken cancellationToken = default)
  - RemoveAsync(string key, CancellationToken cancellationToken = default)
  - GetStatistics() // In-memory only

// Circuit Breaker
ICircuitBreakerService
  - ExecuteAsync<T>(Func<Task<T>> action, string policy = "default")
  - GetCircuitBreakerState(string policy = "default")

// Retry Policy
RetryPolicyService
  - ExecuteAsync<T>(Func<Task<T>> action, int? maxRetries = null, TimeSpan? baseDelay = null)
  - Execute<T>(Func<T> action, int? maxRetries = null, TimeSpan? baseDelay = null)

// HTTP Client Factory
ResilientHttpClientFactory
  - CreateClient()
  - CreateApiClient()
  - CreateExternalClient()
  - CreateLongRunningClient()
  - CreateClientWithTimeout(string name, TimeSpan timeout)

// Event Bus (Acontplus.Core.Abstractions.Messaging)
IEventPublisher
  - PublishAsync<T>(T eventData, CancellationToken cancellationToken = default)

IEventSubscriber
  - SubscribeAsync<T>(CancellationToken cancellationToken = default)

IEventBus : IEventPublisher, IEventSubscriber
```

### 📚 Event Documentation

Acontplus provides **TWO event systems** for different purposes:

#### 1️⃣ Domain Event Dispatcher (from `Acontplus.Core`)
**Use for**: Transactional operations where second insert needs ID from first insert

```csharp
// Application Service
public async Task<Result<Order>> CreateOrderAsync(CreateOrderCommand cmd)
{
    var order = await _orderRepository.AddAsync(new Order { ... });

    // Dispatch DOMAIN EVENT (synchronous, same transaction)
    await _domainEventDispatcher.Dispatch(
        new EntityCreatedEvent(order.Id, nameof(Order), null));

    await _unitOfWork.SaveChangesAsync(); // Commits BOTH inserts
    return Result.Success(order);
}

// Domain Event Handler (runs in SAME transaction)
public class OrderLineItemsHandler : IDomainEventHandler<EntityCreatedEvent>
{
    public async Task HandleAsync(EntityCreatedEvent evt, CancellationToken ct)
    {
        if (evt.EntityType == nameof(Order))
        {
            // Second insert using Order.Id from first insert
            await _lineItemRepo.AddAsync(
                new OrderLineItem { OrderId = evt.EntityId, ... }, ct);
            // Don't SaveChanges - UoW will commit both together
        }
    }
}
```

#### 2️⃣ Application Event Bus (from `Acontplus.Infrastructure`)
**Use for**: Async cross-service communication (notifications, analytics, integration)

```csharp
// Define application event (NOT inheriting IDomainEvent)
public record OrderCreatedEvent(int OrderId, string CustomerName, decimal Total);

// Publish application event (async, fire-and-forget)
await _eventPublisher.PublishAsync(
    new OrderCreatedEvent(order.Id, "John", 99.99));

// Subscribe in BackgroundService (async handler)
public class OrderNotificationHandler : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _eventSubscriber.SubscribeAsync<OrderCreatedEvent>(async (evt, ct) =>
        {
            // Send email (non-transactional, async)
            await _emailService.SendAsync(
                evt.CustomerName,
                $"Order #{evt.OrderId} confirmed!");
        }, stoppingToken);
    }
}
```

#### 📖 Documentation
- **[Event Systems Comparison](../../docs/EVENT_SYSTEMS_COMPARISON.md)** - Which system to use when
- **[Event Bus Guide](../../docs/EVENT_BUS_GUIDE.md)** - Complete Application Event Bus guide
- **[TestApi Example](../../apps/src/Acontplus.TestApplication/)** - Full implementation with both systems

## 🏗️ Architecture

### Folder Structure

```
Acontplus.Infrastructure/
├── Caching/
│   ├── MemoryCacheService.cs          # In-memory cache implementation
│   └── DistributedCacheService.cs     # Redis distributed cache
├── Resilience/
│   ├── CircuitBreakerService.cs       # Circuit breaker service
│   └── RetryPolicyService.cs          # Retry policy service
├── Http/
│   └── ResilientHttpClientFactory.cs  # HTTP client factory
├── Messaging/
│   ├── InMemoryEventBus.cs            # Channel-based event bus
│   ├── ChannelExtensions.cs           # Type-safe channel transformations
│   └── EventBusOptions.cs             # Event bus configuration
├── HealthChecks/
│   ├── CacheHealthCheck.cs            # Cache health check
│   └── CircuitBreakerHealthCheck.cs   # Circuit breaker health check
├── Configuration/
│   ├── CacheConfiguration.cs          # Cache config
│   └── ResilienceConfiguration.cs     # Resilience config
└── Extensions/
    ├── InfrastructureServiceExtensions.cs  # DI registration
    ├── RateLimitingExtensions.cs           # Rate limiting configuration
    └── EventBusExtensions.cs               # Event bus registration
```

### Dependencies

- **Polly**: Resilience and transient-fault-handling
- **Microsoft.Extensions.Caching.StackExchangeRedis**: Redis provider
- **Microsoft.Extensions.Http.Resilience**: HTTP resilience
- **.NET Rate Limiting**: Built-in ASP.NET Core rate limiting middleware
