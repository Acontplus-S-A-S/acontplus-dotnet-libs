# Acontplus.Services

[![NuGet](https://img.shields.io/badge/nuget-v1.4.4-blue.svg)](https://www.nuget.org/packages/Acontplus.Services)
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
<PackageReference Include="Acontplus.Services" Version="1.4.4" />
```

## 🎯 Quick Start

### 1. Basic Setup

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Add application services with all patterns
    services.AddApplicationServices(Configuration);

    // Add authorization policies
    services.AddAuthorizationPolicies();

    // Add MVC with application filters
    services.AddApplicationMvc();

    // Add health checks
    services.AddApplicationHealthChecks(Configuration);
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    // Use application middleware pipeline
    app.UseApplicationMiddleware(env);

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseEndpoints(endpoints => endpoints.MapControllers());
}
```

### 2. Configuration

Add to your `appsettings.json`:

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
      "AllowedImageSources": [
        "https://i.ytimg.com",
        "https://example.com"
      ],
      "AllowedStyleSources": [
        "https://fonts.googleapis.com"
      ],
      "AllowedFontSources": [
        "https://fonts.gstatic.com"
      ],
      "AllowedScriptSources": [
        "https://cdnjs.cloudflare.com"
      ],
      "AllowedConnectSources": [
        "https://api.example.com"
      ]
    }
  },
  "Resilience": {
    "CircuitBreaker": {
      "Enabled": true,
      "FailureThreshold": 5,
      "RecoveryTime": "00:01:00"
    },
    "Retry": {
      "Enabled": true,
      "MaxRetries": 3,
      "BackoffPower": 2
    }
  },
  "Caching": {
    "DefaultExpirationMinutes": 60,
    "RedisConnectionString": "localhost:6379"
  }
}
```

## 🔧 Modern Service Registration

### Core Services

```csharp
// Add caching services (in-memory and Redis)
services.AddCachingServices(Configuration);

// Add resilience services (circuit breakers, retry policies)
services.AddResilienceServices(Configuration);

// Add resilient HTTP clients
services.AddResilientHttpClients(Configuration);

// Add monitoring and telemetry
services.AddMonitoringServices(Configuration);
```

### Infrastructure Services

```csharp
// Add response compression
services.AddResponseCompressionServices();

// Add basic rate limiting
services.AddBasicRateLimiting();

// Add advanced rate limiting (if needed)
services.AddAdvancedRateLimiting(Configuration);
```

### Application Services

```csharp
// Add all application services
services.AddApplicationServices(Configuration);

// Add authorization policies
services.AddAuthorizationPolicies(new List<string>
{
    "web-app", "mobile-app", "admin-portal"
});

// Add MVC with filters
services.AddApplicationMvc();

// Add health checks
services.AddApplicationHealthChecks(Configuration);
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
        return await _cache.GetOrCreateAsync(
            cacheKey,
            async () => await _repository.GetByIdAsync(id),
            TimeSpan.FromMinutes(30)
        );
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

Access health information at `/health`:

```json
{
  "status": "Healthy",
  "results": {
    "memory": {
      "status": "Healthy",
      "description": "Memory usage is within acceptable limits"
    },
    "request-context": {
      "status": "Healthy",
      "description": "Request context service is operational"
    },
    "security-headers": {
      "status": "Healthy",
      "description": "Security header service is operational"
    },
    "device-detection": {
      "status": "Healthy",
      "description": "Device detection service is operational"
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

## 🤝 Contributing

When adding new features:

1. Follow the established patterns (Services, Filters, Policies)
2. Add comprehensive logging
3. Include health checks for new services
4. Update this documentation
5. Add unit tests for new functionality

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
