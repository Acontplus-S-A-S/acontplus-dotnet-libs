# Acontplus.Services

[![NuGet](https://img.shields.io/nuget/v/Acontplus.Services.svg)](https://www.nuget.org/packages/Acontplus.Services)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

A comprehensive .NET 9+ service library providing enterprise-grade patterns, security, caching, resilience, and monitoring for ASP.NET Core applications. Built with modern .NET features and best practices.

## 🚀 .NET 9 Modern Features

### 🎯 Latest C# Language Features

- **Collection Expressions** - Modern `[]` syntax for efficient collection initialization
- **Primary Constructors** - Concise record and class definitions
- **Required Properties** - Compile-time null safety with `required` keyword
- **Pattern Matching** - Advanced `switch` expressions and `is` patterns
- **Record Structs** - High-performance value types for DTOs and results
- **Nullable Reference Types** - Full compile-time null safety
- **Source Generators** - JSON serialization with AOT compilation support
- **Global Usings** - Clean namespace management with global using directives

### 🏗️ Modern Architecture Patterns

- **Domain-Driven Design (DDD)** - Complete DDD implementation with modern C# features
- **Functional Result Pattern** - Railway-oriented programming with record structs
- **Repository Pattern** - Comprehensive data access with bulk operations
- **Specification Pattern** - Type-safe query composition with expressions
- **Event Sourcing Ready** - Domain events with modern event patterns

## 🎯 Core Features

### 🏗️ Service Architecture Patterns

- **Service Layer**: Clean separation of concerns with dependency injection
- **Action Filters**: Reusable cross-cutting concerns (validation, logging, security)
- **Authorization Policies**: Fine-grained access control for modern scenarios
- **Middleware Pipeline**: Properly ordered middleware for security and context management

### 🔒 Security & Compliance

- **Security Headers**: Comprehensive HTTP security header management
- **Content Security Policy**: CSP nonce generation and management
- **Client Validation**: Client-ID based access control
- **Tenant Isolation**: Multi-tenant security policies

### 📱 Device & Context Awareness

- **Device Detection**: Smart device type detection from headers and user agents
- **Request Context**: Correlation IDs, tenant isolation, and request tracking
- **Device-Aware**: Mobile and tablet-aware policies and services

### 🚀 Performance & Observability

- **Request Logging**: Structured logging with performance metrics
- **Health Checks**: Comprehensive health monitoring for all services
- **Response Compression**: Optimized content delivery
- **Rate Limiting**: Built-in rate limiting capabilities
- **Modern Resilience**: Circuit breakers, retry policies, and timeouts using Polly
- **Advanced Caching**: In-memory and distributed caching with Redis support
- **HTTP Client Resilience**: Resilient HTTP clients with automatic retry and circuit breaker
- **Metrics & Monitoring**: Built-in metrics collection and Application Insights integration

### 📊 Advanced Data Patterns

- **Async Streaming** - `IAsyncEnumerable<T>` for memory-efficient processing
- **Projections** - Expression-based data transfer for performance
- **Bulk Operations** - High-performance batch processing with EF Core 9
- **Smart Pagination** - Advanced pagination with search and filtering
- **Modern JSON** - System.Text.Json with source generation

### 🗄️ Caching Architecture

- **Unified Interface** - Single `ICacheService` interface for both in-memory and distributed caching
- **Automatic Fallback** - Graceful degradation when cache operations fail
- **Statistics Support** - Comprehensive cache statistics (in-memory only)
- **Distributed Limitations** - Clear documentation of Redis/distributed cache limitations
- **Health Monitoring** - Functional health checks that test actual cache operations

## 🚀 Quick Start

### 1. Install the Package

```bash
dotnet add package Acontplus.Services
```

### 2. Add to Your Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add all services with one line
builder.Services.AddAcontplusServices(builder.Configuration);

var app = builder.Build();

// Use all middleware with one line
app.UseAcontplusServices(builder.Environment);

app.MapControllers();
app.Run();
```

### 3. Add Basic Configuration

```json
{
  "RequestContext": {
    "EnableSecurityHeaders": true,
    "RequireClientId": false,
    "Csp": {
      "AllowedFrameSources": [
        "https://www.youtube-nocookie.com",
        "https://www.youtube.com"
      ],
      "AllowedMediaSources": [
        "https://www.youtube-nocookie.com"
      ],
      "AllowedBaseUriSources": [
        "https://yourdomain.com"
      ],
      "AllowedFormActionSources": [
        "https://yourdomain.com"
      ],
      "AllowedScriptSources": [
        "https://cdn.jsdelivr.net"
      ],
      "AllowedConnectSources": [
        "https://api.yourdomain.com"
      ]
    }
  },
  "Caching": {
    "UseDistributedCache": false
  }
}
```

### 4. Use in Your Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class HelloController : ControllerBase
{
    private readonly ICacheService _cache;

    public HelloController(ICacheService cache) => _cache = cache;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var message = await _cache.GetOrCreateAsync("hello", 
            () => Task.FromResult("Hello from Acontplus.Services!"), 
            TimeSpan.FromMinutes(5));
        
        return Ok(message);
    }
}
```

That's it! You now have caching, security headers, device detection, and resilience patterns working in your application.

## 📦 Installation

### NuGet Package Manager

```bash
Install-Package Acontplus.Services
```

### .NET CLI

```bash
dotnet add package Acontplus.Services
```

### PackageReference

```xml
<PackageReference Include="Acontplus.Services" Version="1.5.0" />
```

## 🎯 Usage Examples

### 🟢 Basic Usage - Simple Setup

Perfect for small applications or getting started quickly.

```csharp
// Program.cs (.NET 6+)
var builder = WebApplication.CreateBuilder(args);

// One-line setup with sensible defaults
builder.Services.AddAcontplusServices(builder.Configuration);

// Add MVC with built-in filters
builder.Services.AddAcontplusMvc();

var app = builder.Build();

// Complete middleware pipeline in one call
app.UseAcontplusServices(builder.Environment);

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

#### Basic Controller Example

```csharp
[ApiController]
[Route("api/[controller]")]
public class BasicController : ControllerBase
{
    private readonly ICacheService _cache;
    private readonly IRequestContextService _context;

    public BasicController(ICacheService cache, IRequestContextService context)
    {
        _cache = cache;
        _context = context;
    }

    [HttpGet("hello")]
    public async Task<IActionResult> Hello()
    {
        var message = await _cache.GetOrCreateAsync(
            "hello-message",
            () => Task.FromResult("Hello from Acontplus.Services!"),
            TimeSpan.FromMinutes(5)
        );

        return Ok(new { 
            Message = message, 
            CorrelationId = _context.GetCorrelationId() 
        });
    }
}
```

#### Basic Configuration

```json
{
  "RequestContext": {
    "EnableSecurityHeaders": true,
    "RequireClientId": false,
    "Csp": {
      "AllowedFrameSources": [
        "https://www.youtube-nocookie.com",
        "https://www.youtube.com"
      ],
      "AllowedMediaSources": [
        "https://www.youtube-nocookie.com"
      ],
      "AllowedBaseUriSources": [
        "https://yourdomain.com"
      ],
      "AllowedFormActionSources": [
        "https://yourdomain.com"
      ],
      "AllowedScriptSources": [
        "https://cdn.jsdelivr.net"
      ],
      "AllowedConnectSources": [
        "https://api.yourdomain.com"
      ]
    }
  },
  "Caching": {
    "UseDistributedCache": false
  }
}
```

### 🟡 Intermediate Usage - Granular Control

For applications that need fine-grained control over services and middleware.

```csharp
// Program.cs with granular control
var builder = WebApplication.CreateBuilder(args);

// Add services individually for more control
builder.Services.AddCachingServices(builder.Configuration);
builder.Services.AddResilienceServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddAuthorizationPolicies(new List<string> { "web-app", "mobile-app" });

// Add health checks
builder.Services.AddApplicationHealthChecks(builder.Configuration);
builder.Services.AddServiceHealthChecks(builder.Configuration);

// Add MVC with custom filters
builder.Services.AddApplicationMvc(enableGlobalFilters: true);

var app = builder.Build();

// Configure middleware pipeline manually
app.UseSecurityHeaders(builder.Environment);
app.UseMiddleware<CspNonceMiddleware>();
app.UseAdvancedRateLimiting();
app.UseMiddleware<RequestContextMiddleware>();
app.UseAcontplusExceptionHandling();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
```

#### Intermediate Controller with Device Detection

```csharp
[ApiController]
[Route("api/[controller]")]
public class IntermediateController : ControllerBase
{
    private readonly ICacheService _cache;
    private readonly IDeviceDetectionService _deviceDetection;
    private readonly ICircuitBreakerService _circuitBreaker;

    public IntermediateController(
        ICacheService cache, 
        IDeviceDetectionService deviceDetection,
        ICircuitBreakerService circuitBreaker)
    {
        _cache = cache;
        _deviceDetection = deviceDetection;
        _circuitBreaker = circuitBreaker;
    }

    [HttpGet("content")]
    public async Task<IActionResult> GetContent()
    {
        var deviceType = _deviceDetection.DetectDeviceType(HttpContext);
        var cacheKey = $"content:{deviceType}";

        var content = await _cache.GetOrCreateAsync(cacheKey, async () =>
        {
            // Simulate external API call with circuit breaker
            return await _circuitBreaker.ExecuteAsync(async () =>
            {
                await Task.Delay(100); // Simulate API call
                return deviceType switch
                {
                    DeviceType.Mobile => "Mobile-optimized content",
                    DeviceType.Tablet => "Tablet-optimized content",
                    _ => "Desktop content"
                };
            }, "content-api");
        }, TimeSpan.FromMinutes(10));

        return Ok(new { Content = content, DeviceType = deviceType.ToString() });
    }

    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        var circuitBreakerStatus = _circuitBreaker.GetCircuitBreakerState("content-api");
        var cacheStats = _cache.GetStatistics();

        return Ok(new
        {
            CircuitBreaker = circuitBreakerStatus,
            Cache = new
            {
                TotalEntries = cacheStats.TotalEntries,
                HitRate = $"{cacheStats.HitRatePercentage:F1}%"
            }
        });
    }
}
```

#### Intermediate Configuration

```json
{
  "RequestContext": {
    "EnableSecurityHeaders": true,
    "RequireClientId": true,
    "AllowedClientIds": ["web-app", "mobile-app"]
  },
  "Caching": {
    "UseDistributedCache": false,
    "MemoryCacheSizeLimit": 52428800
  },
  "Resilience": {
    "CircuitBreaker": {
      "Enabled": true,
      "ExceptionsAllowedBeforeBreaking": 3
    },
    "RetryPolicy": {
      "Enabled": true,
      "MaxRetries": 2
    }
  }
}
```

### 🔴 Enterprise Usage - Full Configuration

Complete setup for enterprise applications with all features enabled.

```csharp
// Program.cs for enterprise applications
var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddApplicationInsights();

// Add all Acontplus services
builder.Services.AddAcontplusServices(builder.Configuration);

// Add authentication and authorization
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// Add authorization policies
builder.Services.AddAuthorizationPolicies(new List<string>
{
    "web-app", "mobile-app", "admin-portal", "api-client"
});

// Add API documentation
builder.Services.AddAcontplusApiExplorer();
builder.Services.AddSwaggerGen();

// Add MVC with all features
builder.Services.AddAcontplusMvc(enableGlobalFilters: true);

var app = builder.Build();

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAcontplusServices(app.Environment);

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});

app.Run();
```

#### Enterprise Controller with Multi-Tenant Support

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "RequireClientId")]
public class EnterpriseController : ControllerBase
{
    private readonly ICacheService _cache;
    private readonly IRequestContextService _requestContext;
    private readonly ICircuitBreakerService _circuitBreaker;
    private readonly IDeviceDetectionService _deviceDetection;
    private readonly ISecurityHeaderService _securityHeaders;

    public EnterpriseController(
        ICacheService cache,
        IRequestContextService requestContext,
        ICircuitBreakerService circuitBreaker,
        IDeviceDetectionService deviceDetection,
        ISecurityHeaderService securityHeaders)
    {
        _cache = cache;
        _requestContext = requestContext;
        _circuitBreaker = circuitBreaker;
        _deviceDetection = deviceDetection;
        _securityHeaders = securityHeaders;
    }

    [HttpGet("dashboard")]
    [EnableRateLimiting("api")]
    public async Task<IActionResult> GetDashboard()
    {
        var tenantId = _requestContext.GetTenantId();
        var clientId = _requestContext.GetClientId();
        var deviceType = _deviceDetection.DetectDeviceType(HttpContext);
        
        var cacheKey = $"dashboard:{tenantId}:{clientId}:{deviceType}";

        var dashboard = await _cache.GetOrCreateAsync(cacheKey, async () =>
        {
            return await _circuitBreaker.ExecuteAsync(async () =>
            {
                // Simulate complex dashboard data retrieval
                await Task.Delay(200);
                return new
                {
                    TenantId = tenantId,
                    ClientId = clientId,
                    DeviceType = deviceType.ToString(),
                    Data = new
                    {
                        Metrics = new { Requests = 1500, Errors = 2, Uptime = 99.8 },
                        RecentActivity = new[] { "User login", "Data export", "Report generated" },
                        Alerts = new[] { "High memory usage", "Database slow queries" }
                    }
                };
            }, "dashboard-service");
        }, TimeSpan.FromMinutes(5));

        return Ok(dashboard);
    }

    [HttpPost("audit")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult LogAuditEvent([FromBody] AuditEvent auditEvent)
    {
        var context = _requestContext.GetRequestContext();
        
        var auditLog = new
        {
            Event = auditEvent,
            Context = new
            {
                CorrelationId = context.CorrelationId,
                TenantId = context.TenantId,
                ClientId = context.ClientId,
                UserId = User.Identity?.Name,
                Timestamp = DateTime.UtcNow,
                UserAgent = Request.Headers.UserAgent.ToString(),
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            }
        };

        // In a real application, you would log this to your audit system
        _logger.LogInformation("Audit event: {@AuditLog}", auditLog);

        return Ok(new { Message = "Audit event logged", CorrelationId = context.CorrelationId });
    }

    [HttpGet("security-status")]
    public IActionResult GetSecurityStatus()
    {
        var headers = _securityHeaders.GetRecommendedHeaders(false);
        var cspNonce = _securityHeaders.GenerateCspNonce();
        
        return Ok(new
        {
            SecurityHeaders = headers,
            CspNonce = cspNonce,
            CircuitBreakerStatus = new
            {
                Dashboard = _circuitBreaker.GetCircuitBreakerState("dashboard-service"),
                Auth = _circuitBreaker.GetCircuitBreakerState("auth-service"),
                Database = _circuitBreaker.GetCircuitBreakerState("database-service")
            }
        });
    }
}

public class AuditEvent
{
    public string Action { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;
    public Dictionary<string, object>? Metadata { get; set; }
}
```

#### Enterprise Configuration

```json
{
  "RequestContext": {
    "EnableSecurityHeaders": true,
    "FrameOptionsDeny": true,
    "ReferrerPolicy": "strict-origin-when-cross-origin",
    "RequireClientId": true,
    "AllowedClientIds": ["web-app", "mobile-app", "admin-portal", "api-client"],
    "Csp": {
      "AllowedImageSources": ["https://cdn.example.com", "https://images.example.com"],
      "AllowedStyleSources": ["https://fonts.googleapis.com", "https://cdn.example.com"],
      "AllowedScriptSources": ["https://cdn.example.com", "https://js.example.com"],
      "AllowedConnectSources": ["https://api.example.com", "https://auth.example.com"]
    }
  },
  "Resilience": {
    "RateLimiting": {
      "Enabled": true,
      "WindowSeconds": 60,
      "MaxRequestsPerWindow": 1000,
      "SlidingWindow": true,
      "ByIpAddress": true,
      "ByClientId": true
    },
    "CircuitBreaker": {
      "Enabled": true,
      "ExceptionsAllowedBeforeBreaking": 5,
      "DurationOfBreakSeconds": 60,
      "SamplingDurationSeconds": 120,
      "MinimumThroughput": 20
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
  },
  "Caching": {
    "UseDistributedCache": true,
    "RedisConnectionString": "your-redis-connection-string",
    "RedisInstanceName": "acontplus-enterprise",
    "MemoryCacheSizeLimit": 209715200
  }
}
```

```csharp
// Simple controller using basic services
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ICacheService _cache;
    private readonly IRequestContextService _context;

    public ProductsController(ICacheService cache, IRequestContextService context)
    {
        _cache = cache;
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        // Simple caching
        var product = await _cache.GetOrCreateAsync(
            $"product:{id}",
            async () => await GetProductFromDatabase(id),
            TimeSpan.FromMinutes(15)
        );

        // Basic request context
        var correlationId = _context.GetCorrelationId();

        return Ok(new { product, correlationId });
    }

    private async Task<Product> GetProductFromDatabase(int id)
    {
        // Simulate database call
        await Task.Delay(100);
        return new Product { Id = id, Name = $"Product {id}" };
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
```

### 🟡 Intermediate Usage - Multi-Tenant & Device-Aware

For applications with multiple tenants, device detection, and resilience patterns.

```csharp
// Program.cs - Granular service registration
public void ConfigureServices(IServiceCollection services)
{
    // Core infrastructure
    services.AddResponseCompressionServices();
    services.AddBasicRateLimiting();

    // Caching with Redis
    services.AddCachingServices(Configuration);

    // Resilience patterns
    services.AddResilienceServices(Configuration);
}
```

## ⚙️ Configuration Examples

### 🟢 Basic Configuration

Minimal configuration for getting started:

```json
{
  "RequestContext": {
    "EnableSecurityHeaders": true,
    "RequireClientId": false
  },
  "Caching": {
    "UseDistributedCache": false
  },
  "Resilience": {
    "CircuitBreaker": {
      "Enabled": true
    },
    "RetryPolicy": {
      "Enabled": true,
      "MaxRetries": 3
    }
  }
}
```

### 🟡 Production Configuration

Complete configuration for production environments:

```json
{
  "RequestContext": {
    "EnableSecurityHeaders": true,
    "FrameOptionsDeny": true,
    "ReferrerPolicy": "strict-origin-when-cross-origin",
    "RequireClientId": false,
    "AnonymousClientId": "anonymous",
    "AllowedClientIds": ["web-app", "mobile-app", "admin-portal"],
    "Csp": {
      "AllowedImageSources": ["https://i.ytimg.com", "https://example.com"],
      "AllowedStyleSources": ["https://fonts.googleapis.com"],
      "AllowedFontSources": ["https://fonts.gstatic.com"],
      "AllowedScriptSources": ["https://cdnjs.cloudflare.com"],
      "AllowedConnectSources": ["https://api.example.com"]
    }
  },
  "Resilience": {
    "RateLimiting": {
      "Enabled": true,
      "WindowSeconds": 60,
      "MaxRequestsPerWindow": 100,
      "SlidingWindow": true,
      "ByIpAddress": true,
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
  },
  "Caching": {
    "UseDistributedCache": false,
    "RedisConnectionString": "localhost:6379",
    "RedisInstanceName": "acontplus",
    "MemoryCacheSizeLimit": 104857600,
    "ExpirationScanFrequencyMinutes": 5
  }
}
```

## 🧪 Testing Examples

### 🟢 Unit Testing Services

```csharp
public class CacheServiceTests
{
    private readonly Mock<IMemoryCache> _mockCache;
    private readonly Mock<ILogger<MemoryCacheService>> _mockLogger;
    private readonly Mock<IOptions<MemoryCacheOptions>> _mockOptions;
    private readonly MemoryCacheService _cacheService;

    public CacheServiceTests()
    {
        _mockCache = new Mock<IMemoryCache>();
        _mockLogger = new Mock<ILogger<MemoryCacheService>>();
        _mockOptions = new Mock<IOptions<MemoryCacheOptions>>();
        _mockOptions.Setup(x => x.Value).Returns(new MemoryCacheOptions());

        _cacheService = new MemoryCacheService(_mockCache.Object, _mockLogger.Object, _mockOptions.Object);
    }

    [Fact]
    public async Task GetAsync_WhenKeyExists_ReturnsValue()
    {
        // Arrange
        var key = "test-key";
        var expectedValue = "test-value";
        object cacheValue = expectedValue;

        _mockCache.Setup(x => x.TryGetValue(key, out cacheValue)).Returns(true);

        // Act
        var result = await _cacheService.GetAsync<string>(key);

        // Assert
        Assert.Equal(expectedValue, result);
    }

    [Fact]
    public async Task SetAsync_WhenCalled_StoresValue()
    {
        // Arrange
        var key = "test-key";
        var value = "test-value";
        var expiration = TimeSpan.FromMinutes(30);

        // Act
        await _cacheService.SetAsync(key, value, expiration);

        // Assert
        _mockCache.Verify(x => x.Set(key, value, It.IsAny<MemoryCacheEntryOptions>()), Times.Once);
    }
}
```

### 🟡 Integration Testing

```csharp
public class AcontplusServicesIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AcontplusServicesIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task HealthCheck_ReturnsHealthy()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var healthResult = JsonSerializer.Deserialize<HealthCheckResponse>(content);

        Assert.Equal("Healthy", healthResult?.Status);
    }

    [Fact]
    public async Task SecurityHeaders_AreApplied()
    {
        // Act
        var response = await _client.GetAsync("/api/test");

        // Assert
        Assert.True(response.Headers.Contains("X-Content-Type-Options"));
        Assert.True(response.Headers.Contains("X-Frame-Options"));
        Assert.True(response.Headers.Contains("Referrer-Policy"));
    }

    [Fact]
    public async Task RateLimiting_EnforcesLimits()
    {
        // Arrange
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act - Send many requests quickly
        for (int i = 0; i < 150; i++)
        {
            tasks.Add(_client.GetAsync("/api/test"));
        }

        var responses = await Task.WhenAll(tasks);

        // Assert - Some requests should be rate limited
        var rateLimitedResponses = responses.Count(r => r.StatusCode == HttpStatusCode.TooManyRequests);
        Assert.True(rateLimitedResponses > 0);
    }
}

public class HealthCheckResponse
{
    public string? Status { get; set; }
    public Dictionary<string, object>? Results { get; set; }
}
```

### 🔴 Performance Testing

```csharp
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net90)]
public class CacheServiceBenchmarks
{
    private ICacheService _memoryCache = null!;
    private ICacheService _distributedCache = null!;

    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMemoryCache();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = "localhost:6379";
        });

        var provider = services.BuildServiceProvider();

        _memoryCache = new MemoryCacheService(
            provider.GetRequiredService<IMemoryCache>(),
            provider.GetRequiredService<ILogger<MemoryCacheService>>(),
            provider.GetRequiredService<IOptions<MemoryCacheOptions>>());

        _distributedCache = new DistributedCacheService(
            provider.GetRequiredService<IDistributedCache>(),
            provider.GetRequiredService<ILogger<DistributedCacheService>>());
    }

    [Benchmark]
    public async Task MemoryCache_SetAndGet()
    {
        await _memoryCache.SetAsync("benchmark-key", "benchmark-value");
        await _memoryCache.GetAsync<string>("benchmark-key");
    }

    [Benchmark]
    public async Task DistributedCache_SetAndGet()
    {
        await _distributedCache.SetAsync("benchmark-key", "benchmark-value");
        await _distributedCache.GetAsync<string>("benchmark-key");
    }

    [Benchmark]
    [Arguments(100)]
    [Arguments(1000)]
    public async Task MemoryCache_BulkOperations(int count)
    {
        var tasks = new List<Task>();

        for (int i = 0; i < count; i++)
        {
            tasks.Add(_memoryCache.SetAsync($"key-{i}", $"value-{i}"));
        }

        await Task.WhenAll(tasks);

        tasks.Clear();

        for (int i = 0; i < count; i++)
        {
            tasks.Add(_memoryCache.GetAsync<string>($"key-{i}"));
        }

        await Task.WhenAll(tasks);
    }
}
```

## 🚀 Modern Features Examples

### Modern Caching Service

```csharp
public class ProductService
{
    private readonly ICacheService _cache;

    public ProductService(ICacheService cache) => _cache = cache;

    public async Task<Product?> GetProductAsync(int id)
    {
        var cacheKey = $"product:{id}";

        // Modern async caching with factory pattern
        // Works with both in-memory and distributed (Redis) caching
        return await _cache.GetOrCreateAsync(
            cacheKey,
            async () => await _repository.GetByIdAsync(id),
            TimeSpan.FromMinutes(30)
        );
    }

    public async Task<CacheStatistics> GetCacheStatsAsync()
    {
        // Note: Statistics are only available for in-memory cache
        // Distributed cache returns empty statistics due to platform limitations
        return _cache.GetStatistics();
    }
}
```

### Modern Resilience with Polly

```csharp
// Automatically configured with AddResilienceServices
public class ExternalApiService
{
    private readonly HttpClient _httpClient;

    public ExternalApiService(HttpClient httpClient) => _httpClient = httpClient;

    // Automatically wrapped with circuit breaker and retry policies
    public async Task<ApiResponse> GetDataAsync()
    {
        var response = await _httpClient.GetAsync("/api/data");
        return await response.Content.ReadFromJsonAsync<ApiResponse>();
    }
}
```

### Modern Health Checks

```csharp
// Automatically configured with AddApplicationHealthChecks
public class CustomHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        // Your health check logic
        return Task.FromResult(HealthCheckResult.Healthy("Service is operational"));
    }
}
```

## 📱 Device Detection & Context

### Device-Aware Services

```csharp
public class ProductController : ControllerBase
{
    private readonly IDeviceDetectionService _deviceDetection;
    private readonly IRequestContextService _requestContext;

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts()
    {
        var userAgent = Request.Headers.UserAgent.ToString();
        var capabilities = _deviceDetection.GetDeviceCapabilities(userAgent);

        var products = capabilities.IsMobile
            ? await _productService.GetMobileProductsAsync()
            : await _productService.GetDesktopProductsAsync();

        return Ok(products);
    }
}
```

### Request Context Management

```csharp
public class OrderController : ControllerBase
{
    private readonly IRequestContextService _requestContext;

    [HttpPost("orders")]
    public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
    {
        var context = _requestContext.GetRequestContext();
        var correlationId = context.CorrelationId;
        var tenantId = context.TenantId;

        // Use context for logging, tracking, and tenant isolation
        _logger.LogInformation("Creating order {OrderId} for tenant {TenantId}",
            request.OrderId, tenantId);

        return Ok(new { OrderId = request.OrderId, CorrelationId = correlationId });
    }
}
```

## 🔒 Security & Authorization

### Authorization Policies

```csharp
[Authorize(Policy = "RequireClientId")]
[HttpGet("secure")]
public IActionResult SecureEndpoint()
{
    return Ok("Access granted");
}

[Authorize(Policy = "RequireTenant")]
[HttpGet("tenant-data")]
public IActionResult GetTenantData()
{
    return Ok("Tenant-specific data");
}

[Authorize(Policy = "MobileOnly")]
[HttpGet("mobile-only")]
public IActionResult MobileOnlyEndpoint()
{
    return Ok("Mobile access only");
}
```

### Security Headers

```csharp
public class SecurityController : ControllerBase
{
    private readonly ISecurityHeaderService _securityHeaders;

    [HttpGet("headers")]
    public IActionResult GetRecommendedHeaders()
    {
        var headers = _securityHeaders.GetRecommendedHeaders(isDevelopment: false);
        return Ok(headers);
    }
}
```

## 📊 Monitoring & Observability

### Health Checks

Access comprehensive health information at `/health`:

```json
{
  "status": "Healthy",
  "results": {
    "memory": {
      "status": "Healthy",
      "description": "Memory usage is within acceptable limits",
      "data": {
        "allocatedBytes": 52428800,
        "gen0Collections": 5,
        "gen1Collections": 2,
        "gen2Collections": 1
      }
    },
    "cache": {
      "status": "Healthy",
      "description": "Cache service is fully operational",
      "data": {
        "totalEntries": 150,
        "hitRatePercentage": 85.5,
        "lastTestTime": "2024-01-15T10:30:00Z"
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
    },
    "device-detection": {
      "status": "Healthy",
      "description": "Device detection service is fully operational",
      "data": {
        "allTestsPassed": true,
        "test_desktop_chrome": {
          "expected": "Desktop",
          "actual": "Desktop",
          "passed": true
        }
      }
    }
  }
}
```

### Metrics & Telemetry

```csharp
// Automatically configured with AddMonitoringServices
public class MetricsController : ControllerBase
{
    private readonly IMetricsService _metrics;

    public MetricsController(IMetricsService metrics) => _metrics = metrics;

    [HttpGet("metrics")]
    public IActionResult GetMetrics()
    {
        var requestCount = _metrics.GetCounter("http_requests_total");
        var responseTime = _metrics.GetHistogram("http_request_duration_seconds");

        return Ok(new { requestCount, responseTime });
    }
}
```

## 🔐 JWT Authentication Usage

### 🎯 Overview

The JWT authentication extension provides enterprise-grade JWT validation with enhanced security features. It's designed for both single-service applications and complex gateway architectures with multiple APIs.

### 🚀 Quick Start

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add JWT authentication with one line
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### ⚙️ Configuration

#### Basic Configuration

```json
{
  "JwtSettings": {
    "Issuer": "https://auth.acontplus.com",
    "Audience": "api.acontplus.com",
    "SecurityKey": "your-super-secret-key-at-least-32-characters-long",
    "ClockSkew": "5",
    "RequireHttps": "true"
  }
}
```

#### Advanced Configuration

```json
{
  "JwtSettings": {
    "Issuer": "https://auth.acontplus.com",
    "Audience": "api.acontplus.com",
    "SecurityKey": "your-super-secret-key-at-least-32-characters-long",
    "ClockSkew": "2",
    "RequireHttps": "true"
  }
}
```

### 🏗️ Architecture Patterns

#### 1. Single Service Architecture

For standalone applications that handle their own authentication:

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add JWT authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Add authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireValidToken", policy =>
        policy.RequireAuthenticatedUser());
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

**Controller Example:**
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "RequireValidToken")]
public class SecureController : ControllerBase
{
    [HttpGet("data")]
    public IActionResult GetSecureData()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        
        return Ok(new { 
            Message = "Secure data accessed", 
            UserId = userId, 
            Email = email 
        });
    }
}
```

#### 2. Multiple Services Architecture

For applications with multiple APIs that share the same authentication service:

```csharp
// Program.cs for each API
var builder = WebApplication.CreateBuilder(args);

// Each API validates against the same auth service
builder.Services.AddJwtAuthentication(builder.Configuration);

// Add service-specific authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UsersApi", policy =>
        policy.RequireClaim("scope", "users.read", "users.write"));
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

**Configuration for Each API:**
```json
// Users API
{
  "JwtSettings": {
    "Issuer": "https://auth.acontplus.com",
    "Audience": "users-api",
    "SecurityKey": "your-super-secret-key",
    "ClockSkew": "5"
  }
}

// Orders API
{
  "JwtSettings": {
    "Issuer": "https://auth.acontplus.com",
    "Audience": "orders-api",
    "SecurityKey": "your-super-secret-key",
    "ClockSkew": "5"
  }
}
```

**Token with Multiple Audiences:**
```json
{
  "iss": "https://auth.acontplus.com",
  "aud": ["users-api", "orders-api", "gateway"],
  "sub": "user123",
  "scope": ["users.read", "users.write", "orders.read"],
  "exp": 1234567890
}
```

#### 3. Gateway Architecture (Recommended)

For enterprise applications using API gateways like Ocelot:

```csharp
// Gateway Program.cs
var builder = WebApplication.CreateBuilder(args);

// Gateway validates against auth service
builder.Services.AddJwtAuthentication(builder.Configuration);

// Add gateway-specific authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("GatewayAccess", policy =>
        policy.RequireClaim("scope", "gateway"));
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Ocelot configuration
app.UseOcelot();

app.Run();
```

**Gateway Configuration:**
```json
{
  "JwtSettings": {
    "Issuer": "https://auth.acontplus.com",
    "Audience": "gateway.acontplus.com",
    "SecurityKey": "your-super-secret-key",
    "ClockSkew": "2",
    "RequireHttps": "true"
  }
}
```

**API Configuration (Each API):**
```json
{
  "JwtSettings": {
    "Issuer": "https://auth.acontplus.com",
    "Audience": "users-api",
    "SecurityKey": "your-super-secret-key",
    "ClockSkew": "5"
  }
}
```

### 🔒 Security Features

#### Enhanced Token Validation

The JWT authentication extension includes:

- **Full Validation**: Issuer, audience, lifetime, and signing key validation
- **Enhanced Security**: Signed tokens and replay protection
- **HTTPS Enforcement**: Requires HTTPS metadata in production
- **Token Security**: Prevents token storage in claims
- **Error Handling**: Secure error responses without information leakage

#### Security Best Practices

```csharp
// Always use HTTPS in production
options.RequireHttpsMetadata = true;

// Don't save tokens in claims (security risk)
options.SaveToken = false;

// Don't expose internal errors
options.IncludeErrorDetails = false;

// Validate token replay attacks
ValidateTokenReplay = true;

// Require signed tokens
RequireSignedTokens = true;
```

### 📱 Multi-Device Support

#### Device-Aware Authentication

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DeviceController : ControllerBase
{
    private readonly IDeviceDetectionService _deviceDetection;

    public DeviceController(IDeviceDetectionService deviceDetection)
    {
        _deviceDetection = deviceDetection;
    }

    [HttpGet("content")]
    public IActionResult GetContent()
    {
        var deviceType = _deviceDetection.DetectDeviceType(HttpContext);
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var content = deviceType switch
        {
            DeviceType.Mobile => GetMobileContent(userId),
            DeviceType.Tablet => GetTabletContent(userId),
            _ => GetDesktopContent(userId)
        };
        
        return Ok(content);
    }
}
```

### 🔄 Token Refresh & Management

#### Refresh Token Pattern

```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            // Validate refresh token
            var principal = ValidateRefreshToken(request.RefreshToken);
            
            // Generate new access token
            var newToken = GenerateAccessToken(principal);
            
            return Ok(new { 
                AccessToken = newToken,
                ExpiresIn = 3600 
            });
        }
        catch (SecurityTokenException)
        {
            return Unauthorized(new { Message = "Invalid refresh token" });
        }
    }
}
```

### 🧪 Testing JWT Authentication

#### Unit Testing

```csharp
public class JwtAuthenticationTests
{
    [Fact]
    public void AddJwtAuthentication_RegistersRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["JwtSettings:Issuer"] = "https://test.com",
                ["JwtSettings:Audience"] = "test-api",
                ["JwtSettings:SecurityKey"] = "test-key-32-characters-long"
            })
            .Build();

        // Act
        services.AddJwtAuthentication(config);
        var provider = services.BuildServiceProvider();

        // Assert
        var authService = provider.GetService<IAuthenticationService>();
        Assert.NotNull(authService);
    }
}
```

#### Integration Testing

```csharp
public class JwtAuthenticationIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public JwtAuthenticationIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task SecureEndpoint_WithoutToken_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/secure");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SecureEndpoint_WithValidToken_ReturnsOk()
    {
        // Arrange
        var token = GenerateValidJwtToken();
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/secure");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```

### 🚨 Error Handling

#### Authentication Errors

```csharp
[ApiController]
[Route("api/[controller]")]
public class ErrorHandlingController : ControllerBase
{
    [HttpGet("test")]
    [Authorize]
    public IActionResult Test()
    {
        // This will be caught by the global exception handler
        throw new UnauthorizedAccessException("Custom auth error");
    }
}
```

#### Custom Error Responses

```csharp
public class JwtAuthenticationOptions
{
    public static void ConfigureJwtBearer(JwtBearerOptions options)
    {
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                
                var result = JsonSerializer.Serialize(new
                {
                    Error = "Authentication failed",
                    Message = "Invalid or expired token",
                    Timestamp = DateTime.UtcNow
                });
                
                return context.Response.WriteAsync(result);
            },
            
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                
                var result = JsonSerializer.Serialize(new
                {
                    Error = "Authentication required",
                    Message = "Valid JWT token is required",
                    Timestamp = DateTime.UtcNow
                });
                
                return context.Response.WriteAsync(result);
            }
        };
    }
}
```

### 🔧 Advanced Configuration

#### Custom Token Validation

```csharp
public static class CustomJwtExtensions
{
    public static IServiceCollection AddCustomJwtAuthentication(
        this IServiceCollection services, 
        IConfiguration config)
    {
        services.AddJwtAuthentication(config);
        
        // Add custom validation
        services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.TokenValidationParameters.ValidateIssuerSigningKey = true;
            options.TokenValidationParameters.ValidateLifetime = true;
            
            // Custom validation logic
            options.TokenValidationParameters.ValidateIssuer = (issuer, token, parameters) =>
            {
                // Custom issuer validation logic
                return issuer == "https://custom.auth.com";
            };
        });
        
        return services;
    }
}
```

#### Multiple Authentication Schemes

```csharp
public static class MultiSchemeExtensions
{
    public static IServiceCollection AddMultiSchemeAuthentication(
        this IServiceCollection services, 
        IConfiguration config)
    {
        // Add JWT authentication
        services.AddJwtAuthentication(config);
        
        // Add API key authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "MultiScheme";
            options.DefaultChallengeScheme = "MultiScheme";
        })
        .AddPolicyScheme("MultiScheme", "MultiScheme", options =>
        {
            options.ForwardDefaultSelector = context =>
            {
                var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
                
                if (authHeader?.StartsWith("Bearer ") == true)
                    return JwtBearerDefaults.AuthenticationScheme;
                
                if (authHeader?.StartsWith("ApiKey ") == true)
                    return "ApiKey";
                
                return JwtBearerDefaults.AuthenticationScheme;
            };
        });
        
        return services;
    }
}
```

### 📊 Monitoring & Health Checks

#### JWT Authentication Health Check

```csharp
public class JwtAuthenticationHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate JWT configuration
            var config = context.Registration.GetService<IConfiguration>();
            var issuer = config?["JwtSettings:Issuer"];
            var audience = config?["JwtSettings:Audience"];
            var securityKey = config?["JwtSettings:SecurityKey"];
            
            if (string.IsNullOrEmpty(issuer) || 
                string.IsNullOrEmpty(audience) || 
                string.IsNullOrEmpty(securityKey))
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    "JWT configuration is incomplete"));
            }
            
            return Task.FromResult(HealthCheckResult.Healthy(
                "JWT authentication is properly configured"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(
                "JWT authentication health check failed", ex));
        }
    }
}
```

### 🎯 Best Practices Summary

#### ✅ Do's

- **Use the same issuer** across all services in your architecture
- **Use different audiences** for each API/service
- **Always validate tokens** in both gateway and APIs
- **Use HTTPS** in production environments
- **Implement proper error handling** without information leakage
- **Monitor authentication failures** for security insights

#### ❌ Don'ts

- **Don't modify JWT tokens** after they're signed
- **Don't use weak security keys** (minimum 32 characters)
- **Don't expose internal errors** in authentication responses
- **Don't store tokens** in claims or cookies
- **Don't disable security validations** in production

#### 🔐 Security Checklist

- [ ] JWT issuer validation enabled
- [ ] JWT audience validation enabled
- [ ] Token lifetime validation enabled
- [ ] Signing key validation enabled
- [ ] HTTPS metadata required in production
- [ ] Token replay protection enabled
- [ ] Signed tokens required
- [ ] Error details disabled in production
- [ ] Proper security headers configured
- [ ] Rate limiting on authentication endpoints

## 🔄 Migration Guide

### From Legacy Services

1. **Replace old extension class names**:

   - `ModernServiceExtensions` → `ServiceExtensions`
   - `EnterpriseServiceExtensions` → `ApplicationServiceExtensions`

2. **Update method calls**:

   - `AddModernCaching` → `AddCachingServices`
   - `AddEnterpriseServices` → `AddApplicationServices`
   - `UseEnterpriseMiddleware` → `UseApplicationMiddleware`

3. **Adopt new service patterns**:

   ```csharp
   // Old approach
   var userId = HttpContext.User.GetUserId();

   // New approach
   var userId = _requestContext.GetUserId(); // via IRequestContextService
   ```

## 🏗️ Architecture Patterns

### Service Layer Architecture

- **Clean Separation**: Clear boundaries between business logic and infrastructure
- **Dependency Injection**: Proper service registration and lifetime management
- **Interface Segregation**: Focused service contracts for specific concerns
- **Testability**: Easy mocking and unit testing of services

### Middleware Pipeline

The application middleware pipeline is automatically configured in the correct order:

1. **Security Headers**: Applied early for all responses
2. **CSP Nonce**: Generates Content Security Policy nonces
3. **Request Context**: Extracts and validates request context
4. **Exception Handling**: Global exception handling with standardized responses
5. **Rate Limiting**: Basic and advanced rate limiting
6. **Compression**: Response compression for performance

### Error Handling

- **Global Exception Handling**: Centralized error handling with standardized responses
- **Domain Errors**: Business logic errors with proper HTTP status codes
- **Validation Errors**: Model validation with consistent error format
- **Logging**: Comprehensive error logging with correlation IDs

## 📚 API Reference

### Core Services

- `ICacheService` - Caching service supporting in-memory and distributed caching
- `IRequestContextService` - Request context management and correlation
- `ISecurityHeaderService` - HTTP security header management
- `IDeviceDetectionService` - Device type detection and capabilities
- `IMetricsService` - Metrics collection and monitoring

### Configuration

- `ResilienceConfiguration` - Circuit breaker and retry policy configuration
- `RequestContextConfiguration` - Request context and security settings
- `CachingConfiguration` - Cache settings and Redis configuration

### Middleware

- `GlobalExceptionMiddleware` - Global exception handling
- `RequestContextMiddleware` - Request context extraction
- `SecurityHeadersMiddleware` - Security header application
- `RateLimitingMiddleware` - Rate limiting implementation

## 🚀 Performance Features

### Caching Strategies

- **In-Memory Caching**: Fast local caching for frequently accessed data
- **Distributed Caching**: Redis-based caching for multi-instance applications
- **Cache-Aside Pattern**: Automatic cache population and invalidation
- **TTL Management**: Configurable expiration times for different data types

### Resilience Patterns

- **Circuit Breaker**: Prevents cascading failures in distributed systems
- **Retry Policies**: Exponential backoff with jitter for transient failures
- **Timeout Policies**: Configurable timeouts for external service calls
- **Bulkhead Isolation**: Resource isolation for different service types

### Rate Limiting

- **Basic Rate Limiting**: Global rate limits for all endpoints
- **Advanced Rate Limiting**: Per-endpoint and per-user rate limiting
- **Sliding Window**: Smooth rate limiting without burst effects
- **Configurable Policies**: Different limits for different client types

## 💡 Common Use Cases & Best Practices

### 🎯 When to Use Each Service

#### ICacheService
- **Use in-memory caching** for single-instance applications with moderate memory usage
- **Use distributed caching (Redis)** for multi-instance deployments and shared state
- **Cache frequently accessed data** like user preferences, configuration, and lookup tables
- **Avoid caching** user-specific data that changes frequently or sensitive information

#### ICircuitBreakerService
- **Use for external API calls** to prevent cascading failures
- **Use for database operations** that might become slow or unavailable
- **Use for third-party service integration** like payment gateways or weather APIs
- **Don't use for** simple, fast operations that rarely fail

#### IDeviceDetectionService
- **Use for responsive design** and mobile-first applications
- **Use for content optimization** based on device capabilities
- **Use for analytics** and user experience tracking
- **Don't rely solely on** user agent strings for critical functionality

#### IRequestContextService
- **Use for correlation tracking** across microservices
- **Use for tenant isolation** in multi-tenant applications
- **Use for audit logging** and request tracing
- **Always validate** tenant and client IDs before use

#### ISecurityHeaderService
- **Use in production** to protect against common web vulnerabilities
- **Configure CSP carefully** to avoid breaking legitimate functionality
- **Test headers** in development before deploying to production
- **Monitor security headers** using browser developer tools

### 🔄 Caching Strategies

#### Cache Key Design
```csharp
// Good: Descriptive and hierarchical
"user:123:preferences"
"products:category:electronics:page:1:size:20"
"weather:city:london:country:uk"

// Avoid: Generic or unclear keys
"data"
"temp"
"123"
```

#### Cache Invalidation Patterns
```csharp
// Pattern 1: Time-based expiration
await _cache.SetAsync("user:123", userData, TimeSpan.FromHours(1));

// Pattern 2: Event-based invalidation
await _cache.RemoveAsync("user:123");
await _cache.RemoveAsync("users:list");

// Pattern 3: Version-based caching
await _cache.SetAsync("products:v2:123", product, TimeSpan.FromDays(1));
```

### 🛡️ Security Best Practices

#### Content Security Policy
```csharp
// Configure CSP for your specific needs
var cspConfig = new
{
    DefaultSrc = "'self'",
    ScriptSrc = "'self' 'unsafe-inline' 'nonce-" + _securityHeaders.GenerateCspNonce() + "'",
    StyleSrc = "'self' 'unsafe-inline' https://fonts.googleapis.com",
    ImgSrc = "'self' data: https:",
    ConnectSrc = "'self' https://api.example.com"
};
```

#### Client ID Validation
```csharp
// Always validate client IDs
public class ClientValidationAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var requestContext = context.HttpContext.RequestServices
            .GetRequiredService<IRequestContextService>();
        
        var clientId = requestContext.GetClientId();
        if (!IsValidClient(clientId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        
        base.OnActionExecuting(context);
    }
}
```

### 📱 Device Detection Best Practices

#### Progressive Enhancement
```csharp
public class ContentService
{
    public async Task<ContentResponse> GetContentAsync(HttpContext context)
    {
        var deviceType = _deviceDetection.DetectDeviceType(context);
        var capabilities = _deviceDetection.GetDeviceCapabilities(
            context.Request.Headers.UserAgent.ToString());

        var baseContent = await GetBaseContentAsync();
        
        return deviceType switch
        {
            DeviceType.Mobile when capabilities.SupportsTouch => 
                EnhanceForTouch(baseContent),
            DeviceType.Mobile => 
                EnhanceForMobile(baseContent),
            DeviceType.Tablet => 
                EnhanceForTablet(baseContent),
            _ => baseContent
        };
    }
}
```

### 🔄 Resilience Patterns

#### Fallback Strategies
```csharp
public class ResilientService
{
    public async Task<T> ExecuteWithFallbackAsync<T>(
        Func<Task<T>> primaryOperation,
        Func<Task<T>> fallbackOperation,
        string operationName)
    {
        try
        {
            return await _circuitBreaker.ExecuteAsync(primaryOperation, operationName);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Primary operation failed, using fallback");
            return await fallbackOperation();
        }
    }
}
```

#### Bulkhead Isolation
```csharp
// Use different circuit breakers for different service types
await _circuitBreaker.ExecuteAsync(operation, "database-service");
await _circuitBreaker.ExecuteAsync(operation, "external-api-service");
await _circuitBreaker.ExecuteAsync(operation, "payment-service");
```

## 🔧 Troubleshooting

### Common Issues

#### Cache Service Issues

- **Distributed cache statistics are empty**: This is expected - Redis and other distributed caches don't expose statistics through `IDistributedCache`
- **Clear operations don't work with distributed cache**: This is a platform limitation - distributed caches don't support clearing all entries

#### Circuit Breaker Issues

- **Circuit breakers always show as Closed**: Ensure resilience configuration is properly loaded and circuit breaker is enabled
- **Health checks show degraded status**: Check if any circuit breakers are open due to recent failures

#### Configuration Issues

- **Services not registering**: Ensure you're calling `AddAcontplusServices(configuration)` in your startup
- **Configuration not loading**: Verify your `appsettings.json` structure matches the documented format

#### Health Check Issues

- **Request context health check fails**: This is expected when running outside HTTP context (e.g., during startup)
- **Device detection tests fail**: Verify user agent parsing logic if you've modified the detection patterns

### Performance Tips

- Use in-memory caching for single-instance applications
- Use distributed caching (Redis) for multi-instance deployments
- Configure circuit breaker thresholds based on your service's error tolerance
- Monitor health check endpoints for early problem detection

## 🤝 Contributing

When adding new features:

1. Follow the established patterns (Services, Filters, Policies)
2. Add comprehensive logging
3. Include functional health checks for new services
4. Update this documentation with configuration examples
5. Add unit tests for new functionality
6. Document any platform limitations or known issues

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](../../LICENSE) file for details.

## 🆘 Support

- 📧 Email: proyectos@acontplus.com
- 🐛 Issues: [GitHub Issues](https://github.com/Acontplus-S-A-S/acontplus-dotnet-libs/issues)
- 📖 Documentation: [Wiki](https://github.com/Acontplus-S-A-S/acontplus-dotnet-libs/wiki)

## 👨‍💻 Author

**Ivan Paz** - [@iferpaz7](https://linktr.ee/iferpaz7)

## 🏢 Company

**[Acontplus S.A.S.](https://acontplus.com.ec)** - Enterprise software solutions, Ecuador

---

**Built with ❤️ for the .NET community using the latest .NET 9 features**
