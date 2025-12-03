# Acontplus.Services

[![NuGet](https://img.shields.io/nuget/v/Acontplus.Services.svg)](https://www.nuget.org/packages/Acontplus.Services)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

A comprehensive .NET service library providing business-grade patterns, security, device detection, request management, and **intelligent exception handling** for ASP.NET Core applications. Built with modern .NET 10 features and best practices.

> **💡 Infrastructure Services**: For caching, circuit breakers, resilience patterns, and HTTP client factory, use **[Acontplus.Infrastructure](https://www.nuget.org/packages/Acontplus.Infrastructure)**

## 🚀 Features

### 🏗️ Service Architecture Patterns

- **Service Layer**: Clean separation of concerns with dependency injection
- **Lookup Service**: Cached lookup/reference data management with flexible SQL mapping
- **Action Filters**: Reusable cross-cutting concerns (validation, logging, security)
- **Authorization Policies**: Fine-grained access control for multi-tenant scenarios
- **Middleware Pipeline**: Properly ordered middleware for security and context management

### 🛡️ Advanced Exception Handling **NEW!**

- **Flexible Design**: Works with or without catch blocks - your choice!
- **Smart Exception Translation**: Preserves custom error codes from business logic
- **DomainException Support**: Automatic handling of domain exceptions with proper HTTP status codes
- **Consistent API Responses**: Standardized error format with categories and severity
- **Intelligent Logging**: Context-aware logging with appropriate severity levels
- **Distributed Tracing**: Correlation IDs and trace IDs for request tracking
- **Multi-tenancy Support**: Tenant ID tracking across requests

📖 **[Complete Exception Handling Guide](Middleware/ApiExceptionMiddleware.README.md)**

### 🔒 Security & Compliance

- **Security Headers**: Comprehensive HTTP security header management
- **Content Security Policy**: CSP nonce generation and management
- **Client Validation**: Client-ID based access control
- **Tenant Isolation**: Multi-tenant security policies
- **JWT Authentication**: Enterprise-grade JWT token validation

### 📱 Device & Context Awareness

- **Device Detection**: Smart device type detection from headers and user agents
- **Request Context**: Correlation IDs, tenant isolation, and request tracking
- **Device-Aware Policies**: Mobile and tablet-aware authorization policies

### 📊 Observability

- **Request Logging**: Structured logging with performance metrics
- **Health Checks**: Comprehensive health monitoring for application services
- **Application Insights**: Optional integration for telemetry and monitoring

## 📦 Installation

### Required Packages

```bash
# Application services (this package)
dotnet add package Acontplus.Services

# Infrastructure services (caching, resilience, etc.)
dotnet add package Acontplus.Infrastructure
```

### NuGet Package Manager

```bash
Install-Package Acontplus.Services
Install-Package Acontplus.Infrastructure
```

### PackageReference

```xml
<PackageReference Include="Acontplus.Services" Version="1.5.0" />
<PackageReference Include="Acontplus.Infrastructure" Version="1.0.0" />
```

## 🎯 Quick Start

### 1. Add to Your Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add application services (authentication, security, device detection, exception handling)
builder.Services.AddApplicationServices(builder.Configuration);

// Add infrastructure services (caching, resilience, HTTP clients)
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Use application middleware pipeline (includes exception handling)
app.UseApplicationMiddleware(builder.Environment);

app.MapControllers();
app.Run();
```

### 2. Exception Handling - No Catch Needed! **NEW!**

```csharp
// Business Layer - Just throw, middleware handles everything
public async Task<Customer> GetCustomerAsync(int id)
{
    var customer = await _repository.GetByIdAsync(id);

    if (customer is null)
    {
        throw new GenericDomainException(
            ErrorType.NotFound,
            "CUSTOMER_NOT_FOUND",
            "Customer not found");
    }

    return customer;
}
```

**Automatic Response:**
```json
{
  "success": false,
  "code": "404",
  "message": "Customer not found",
  "errors": [{
    "code": "CUSTOMER_NOT_FOUND",
    "message": "Customer not found",
    "category": "business",
    "severity": "warning"
  }],
  "correlationId": "abc-123"
}
```

**Or Use Result Pattern:**
```csharp
public async Task<Result<Customer, DomainError>> GetCustomerAsync(int id)
{
    try
    {
        var customer = await _repository.GetByIdAsync(id);
        return customer ?? DomainError.NotFound("CUSTOMER_NOT_FOUND", "Not found");
    }
    catch (SqlDomainException ex)
    {
        return ex.ToDomainError();
    }
}

// Controller
[HttpGet("{id}")]
public Task<IActionResult> GetCustomer(int id)
{
    return _service.GetCustomerAsync(id).ToActionResultAsync();
}
```

### 3. Basic Configuration

Add to your `appsettings.json`:

```json
{
  "RequestContext": {
    "EnableSecurityHeaders": true,
    "RequireClientId": false,
    "Csp": {
      "AllowedFrameSources": ["https://www.youtube-nocookie.com"],
      "AllowedScriptSources": ["https://cdn.jsdelivr.net"],
      "AllowedConnectSources": ["https://api.yourdomain.com"]
    }
  },
  "ExceptionHandling": {
    "IncludeDebugDetailsInResponse": false,
    "IncludeRequestDetails": true,
    "LogRequestBody": false
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
    private readonly IRequestContextService _context;

    public HelloController(ICacheService cache, IRequestContextService context)
    {
        _cache = cache;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var message = await _cache.GetOrCreateAsync("hello",
            () => Task.FromResult("Hello from Acontplus.Services!"),
            TimeSpan.FromMinutes(5));

        return Ok(new {
            Message = message,
            CorrelationId = _context.GetCorrelationId()
        });
    }
}
```

## 🎯 Usage Examples

### 🟢 Basic Usage - Simple Setup

Perfect for small applications or getting started quickly.

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add application and infrastructure services
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add controllers
builder.Services.AddControllers();

var app = builder.Build();

// Complete middleware pipeline in one call
app.UseApplicationMiddleware(builder.Environment);

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

### 🟡 Intermediate Usage - Granular Control

For applications that need fine-grained control over services and middleware.

```csharp
// Program.cs with granular control
var builder = WebApplication.CreateBuilder(args);

// Add services individually for more control
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddCachingServices(builder.Configuration);
builder.Services.AddResilienceServices(builder.Configuration);
builder.Services.AddAuthorizationPolicies(new List<string> { "web-app", "mobile-app" });

// Add health checks
builder.Services.AddApplicationHealthChecks(builder.Configuration);
builder.Services.AddInfrastructureHealthChecks();

// Add controllers with custom filters
builder.Services.AddControllers(options =>
{
    options.Filters.Add<SecurityHeaderActionFilter>();
    options.Filters.Add<RequestLoggingActionFilter>();
    options.Filters.Add<ValidationActionFilter>();
});

var app = builder.Build();

// Configure middleware pipeline manually
app.UseSecurityHeaders(builder.Environment);
app.UseMiddleware<CspNonceMiddleware>();
app.UseMiddleware<RateLimitingMiddleware>();
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
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add controllers
builder.Services.AddControllers();

var app = builder.Build();

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseApplicationMiddleware(app.Environment);

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

## ⚙️ Configuration Examples

### Complete Configuration

```json
{
  "RequestContext": {
    "EnableSecurityHeaders": true,
    "FrameOptionsDeny": true,
    "ReferrerPolicy": "strict-origin-when-cross-origin",
    "RequireClientId": true,
    "AllowedClientIds": ["web-app", "mobile-app", "admin-portal"],
    "Csp": {
      "AllowedImageSources": ["https://cdn.example.com"],
      "AllowedStyleSources": ["https://fonts.googleapis.com"],
      "AllowedScriptSources": ["https://cdn.example.com"],
      "AllowedConnectSources": ["https://api.example.com"]
    }
  },
  "Caching": {
    "UseDistributedCache": false,
    "MemoryCacheSizeLimit": 104857600
  },
  "Resilience": {
    "CircuitBreaker": {
      "Enabled": true,
      "ExceptionsAllowedBeforeBreaking": 5
    },
    "RetryPolicy": {
      "Enabled": true,
      "MaxRetries": 3
    }
  },
  "JwtSettings": {
    "Issuer": "https://auth.acontplus.com",
    "Audience": "api.acontplus.com",
    "SecurityKey": "your-super-secret-key-at-least-32-characters-long",
    "ClockSkew": "5",
    "RequireHttps": "true"
  }
}
```

## 📚 Core Services Reference

### What's in Acontplus.Services

✅ **Application Services**
- `IRequestContextService` - Request context management and correlation
- `ISecurityHeaderService` - HTTP security headers and CSP management
- `IDeviceDetectionService` - Device type detection and capabilities
- `ILookupService` - Cached lookup/reference data management (NEW!)

✅ **Action Filters**
- `ValidationActionFilter` - Model validation
- `RequestLoggingActionFilter` - Request/response logging
- `SecurityHeaderActionFilter` - Security header injection

✅ **Authorization Policies**
- `RequireClientIdPolicy` - Client ID validation
- `TenantIsolationPolicy` - Multi-tenant isolation
- `DeviceTypePolicy` - Device-aware authorization

✅ **Middleware**
- `RequestContextMiddleware` - Request context extraction
- `CspNonceMiddleware` - CSP nonce generation
- `ApiExceptionMiddleware` - Global exception handling

### What's in Acontplus.Infrastructure

> **Note**: These services require `Acontplus.Infrastructure` package

✅ **Infrastructure Services** (from Acontplus.Infrastructure)
- `ICacheService` - Caching (in-memory and Redis)
- `ICircuitBreakerService` - Circuit breaker patterns
- `RetryPolicyService` - Retry policies
- `ResilientHttpClientFactory` - Resilient HTTP clients

✅ **Middleware** (from Acontplus.Infrastructure)
- `RateLimitingMiddleware` - Rate limiting

✅ **Health Checks** (from Acontplus.Infrastructure)
- `CacheHealthCheck` - Cache service health
- `CircuitBreakerHealthCheck` - Circuit breaker health

## 🚀 Features Examples

### Lookup Service (NEW!)

Manage cached lookup/reference data from database queries with automatic caching.

```csharp
// 1. Register in Program.cs
builder.Services.AddLookupService();

// 2. Use in controller
public class LookupsController : ControllerBase
{
    private readonly ILookupService _lookupService;

    public LookupsController(ILookupService lookupService)
    {
        _lookupService = lookupService;
    }

    [HttpGet]
    public async Task<IActionResult> GetLookups(
        [FromQuery] string? module = null,
        [FromQuery] string? context = null)
    {
        var filterRequest = new FilterRequest
        {
            Filters = new Dictionary<string, object>
            {
                ["module"] = module ?? "default",
                ["context"] = context ?? "general"
            }
        };

        var result = await _lookupService.GetLookupsAsync(
            "YourSchema.GetLookups", // Stored procedure name
            filterRequest);

        return result.Match(
            success => Ok(ApiResponse.Success(success)),
            error => BadRequest(ApiResponse.Failure(error)));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshLookups()
    {
        var result = await _lookupService.RefreshLookupsAsync(
            "YourSchema.GetLookups",
            new FilterRequest());

        return result.Match(
            success => Ok(ApiResponse.Success(success)),
            error => BadRequest(ApiResponse.Failure(error)));
    }
}
```

**Features:**
- ✅ Automatic caching (30-minute TTL)
- ✅ Works with SQL Server and PostgreSQL
- ✅ Flexible SQL query mapping (all nullable properties)
- ✅ Supports hierarchical data (ParentId)
- ✅ Grouped results by table name
- ✅ Cache refresh on demand

**SQL Stored Procedure Example:**
```sql
CREATE PROCEDURE [YourSchema].[GetLookups]
    @Module NVARCHAR(100) = NULL,
    @Context NVARCHAR(100) = NULL
AS
BEGIN
    SELECT
        'Countries' AS TableName,
        Id, Code, [Name] AS [Value], DisplayOrder,
        NULL AS ParentId, IsDefault, IsActive,
        Description, NULL AS Metadata
    FROM Countries
    WHERE IsActive = 1
    ORDER BY DisplayOrder;
END
```

**Response Format:**
```json
{
  "status": "Success",
  "data": {
    "countries": [
      {
        "id": 1,
        "code": "US",
        "value": "United States",
        "displayOrder": 1,
        "isDefault": true,
        "isActive": true,
        "description": "United States of America",
        "metadata": null
      }
    ]
  }
}
```

📖 **[Complete Lookup Service Guide](../../docs/lookup-service-quick-reference.md)**

### Caching Service

> **Requires**: `Acontplus.Infrastructure` package

```csharp
public class ProductService
{
    private readonly ICacheService _cache;

    public ProductService(ICacheService cache) => _cache = cache;

    public async Task<Product?> GetProductAsync(int id)
    {
        var cacheKey = $"product:{id}";

        // Async caching with factory pattern
        return await _cache.GetOrCreateAsync(
            cacheKey,
            async () => await _repository.GetByIdAsync(id),
            TimeSpan.FromMinutes(30)
        );
    }
}
```

### Device Detection

```csharp
public class ProductController : ControllerBase
{
    private readonly IDeviceDetectionService _deviceDetection;

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
        var correlationId = _requestContext.GetCorrelationId();
        var tenantId = _requestContext.GetTenantId();
        var clientId = _requestContext.GetClientId();

        _logger.LogInformation("Creating order for tenant {TenantId}", tenantId);

        return Ok(new { OrderId = request.OrderId, CorrelationId = correlationId });
    }
}
```

### Context Extensions

```csharp
public class AdvancedController : ControllerBase
{
    [HttpGet("context-info")]
    public IActionResult GetContextInfo()
    {
        // HTTP context extensions
        var userAgent = HttpContext.GetUserAgent();
        var ipAddress = HttpContext.GetClientIpAddress();
        var requestPath = HttpContext.GetRequestPath();

        // Claims principal extensions
        var userId = User.GetUserId();
        var email = User.GetEmail();
        var roles = User.GetRoles();
        var isAdmin = User.HasRole("admin");

        return Ok(new
        {
            Request = new { UserAgent = userAgent, IpAddress = ipAddress, Path = requestPath },
            User = new { UserId = userId, Email = email, Roles = roles, IsAdmin = isAdmin }
        });
    }
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
        var cspNonce = _securityHeaders.GenerateCspNonce();

        return Ok(new { Headers = headers, CspNonce = cspNonce });
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

## 📊 Health Checks

Access comprehensive health information at `/health`:

```json
{
  "status": "Healthy",
  "results": {
    "request-context": {
      "status": "Healthy",
      "description": "Request context service is fully operational"
    },
    "security-headers": {
      "status": "Healthy",
      "description": "Security header service is operational"
    },
    "device-detection": {
      "status": "Healthy",
      "description": "Device detection service is fully operational"
    },
    "cache": {
      "status": "Healthy",
      "description": "Cache service is fully operational",
      "data": {
        "totalEntries": 150,
        "hitRatePercentage": 85.5
      }
    }
  }
}
```

## 🔐 JWT Authentication Usage

### Quick Start

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

### Configuration

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

### Controller Example

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
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

## 📚 API Reference

### Core Services

- `IRequestContextService` - Request context management and correlation
- `ISecurityHeaderService` - HTTP security headers and CSP management
- `IDeviceDetectionService` - Device type detection and capabilities

### Configuration

- `RequestContextConfiguration` - Request context and security settings
- `JwtSettings` - JWT authentication configuration

### Middleware

- `RequestContextMiddleware` - Request context extraction
- `CspNonceMiddleware` - CSP nonce generation
- `ApiExceptionMiddleware` - Global exception handling

## 🤝 Contributing

When adding new features:

1. Follow the established patterns (Services, Filters, Policies)
2. Add comprehensive logging
3. Include functional health checks for new services
4. Update this documentation with configuration examples
5. Add unit tests for new functionality

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](../../LICENSE) file for details.

## 📋 Package Comparison

| Feature | Acontplus.Services | Acontplus.Infrastructure |
|---------|-------------------|-------------------------|
| Request Context | ✅ | ❌ |
| Security Headers | ✅ | ❌ |
| Device Detection | ✅ | ❌ |
| JWT Authentication | ✅ | ❌ |
| Authorization Policies | ✅ | ❌ |
| Caching | ❌ | ✅ |
| Circuit Breaker | ❌ | ✅ |
| Retry Policies | ❌ | ✅ |
| HTTP Client Factory | ❌ | ✅ |
| Rate Limiting | ❌ | ✅ |

## 🎯 Best Practices

### ✅ Do's
- Use `AddApplicationServices()` for application-level concerns
- Use `AddInfrastructureServices()` for infrastructure concerns
- **NEW**: Let DomainExceptions bubble up for simpler code
- **NEW**: Use Result pattern for complex workflows
- Always validate client IDs and tenant IDs in multi-tenant scenarios
- Configure CSP policies carefully to avoid breaking functionality
- Monitor health check endpoints regularly
- Use correlation IDs for request tracking across services

### ❌ Don'ts
- Don't disable security headers in production
- Don't use weak JWT security keys (minimum 32 characters)
- Don't expose internal errors in API responses
- Don't cache sensitive user data
- Don't ignore health check failures
- Don't use generic cache keys
- **NEW**: Don't catch and swallow DomainExceptions (let middleware handle them)
