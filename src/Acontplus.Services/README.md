# Acontplus.Services

Modern service library providing enterprise-grade patterns for ASP.NET Core applications.

## Features

### 🏗️ Enterprise Architecture Patterns

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

## Quick Start

### 1. Basic Setup

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Add enterprise services with all patterns
    services.AddEnterpriseServices(Configuration);

    // Add authorization policies
    services.AddEnterpriseAuthorizationPolicies();

    // Add MVC with enterprise filters
    services.AddEnterpriseMvc();

    // Add health checks
    services.AddEnterpriseHealthChecks(Configuration);
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    // Use enterprise middleware pipeline
    app.UseEnterpriseMiddleware(env);

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
    "AllowedClientIds": ["web-app", "mobile-app", "admin-portal"]
  }
}
```

## Services

### IRequestContextService

Manages request context information across the application:

```csharp
public class MyController : ControllerBase
{
    private readonly IRequestContextService _requestContext;

    public MyController(IRequestContextService requestContext)
    {
        _requestContext = requestContext;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var context = _requestContext.GetRequestContext();
        var isMobile = _requestContext.IsMobileRequest();

        return Ok(new { context, isMobile });
    }
}
```

### ISecurityHeaderService

Manages HTTP security headers:

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

### IDeviceDetectionService

Detects device types and capabilities:

```csharp
public class DeviceController : ControllerBase
{
    private readonly IDeviceDetectionService _deviceDetection;

    [HttpGet("capabilities")]
    public IActionResult GetCapabilities()
    {
        var userAgent = Request.Headers.UserAgent.ToString();
        var capabilities = _deviceDetection.GetDeviceCapabilities(userAgent);

        return Ok(capabilities);
    }
}
```

## Authorization Policies

### Client-ID Based Access

```csharp
[Authorize(Policy = "RequireClientId")]
[HttpGet("secure")]
public IActionResult SecureEndpoint()
{
    return Ok("Access granted");
}
```

### Tenant Isolation

```csharp
[Authorize(Policy = "RequireTenant")]
[HttpGet("tenant-data")]
public IActionResult GetTenantData()
{
    return Ok("Tenant-specific data");
}
```

### Device Type Restrictions

```csharp
[Authorize(Policy = "MobileOnly")]
[HttpGet("mobile-only")]
public IActionResult MobileOnlyEndpoint()
{
    return Ok("Mobile access only");
}
```

## Action Filters

### Global Filters

Automatically applied to all controllers:

- `SecurityHeaderActionFilter`: Applies security headers
- `RequestLoggingActionFilter`: Logs request details and performance
- `ValidationActionFilter`: Handles model validation with standardized responses

### Manual Filter Application

```csharp
[ServiceFilter(typeof(RequestLoggingActionFilter))]
public class ApiController : ControllerBase
{
    // Controller actions
}
```

## Middleware Pipeline

The enterprise middleware pipeline is automatically configured in the correct order:

1. **Security Headers**: Applied early for all responses
2. **CSP Nonce**: Generates Content Security Policy nonces
3. **Request Context**: Extracts and validates request context
4. **Exception Handling**: Global exception handling with standardized responses

## Health Checks

Access health information at `/health`:

```json
{
  "status": "Healthy",
  "results": {
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

## Migration Guide

### From Legacy Services

1. **Replace old extension class names**:

   - `IdentityServiceExtensions` → `JwtAuthenticationExtensions`
   - `ServiceConfigurationExtensions` → `InfrastructureServiceExtensions`
   - `ExceptionHandlingExtensions` → `GlobalExceptionHandlingExtensions`

2. **Update configuration classes**:

   - `RequestContextOptions` → `RequestContextConfiguration`
   - `AppConfiguration` → `ApplicationConfigurationBuilder`

3. **Adopt new service patterns**:

   ```csharp
   // Old approach
   var userId = HttpContext.User.GetUserId();

   // New approach
   var userId = _requestContext.GetUserId(); // via IRequestContextService
   ```

## Best Practices

1. **Use dependency injection** for all services
2. **Apply authorization policies** at the controller or action level
3. **Configure security headers** appropriate for your environment
4. **Monitor health checks** in production
5. **Use structured logging** with correlation IDs
6. **Validate device headers** for mobile applications

## Version History

- **v1.2.0**: Added enterprise service patterns and authorization policies
- **v1.1.x**: Enhanced middleware and security features
- **v1.0.x**: Initial release with basic services

## Contributing

When adding new features:

1. Follow the established patterns (Services, Filters, Policies)
2. Add comprehensive logging
3. Include health checks for new services
4. Update this documentation
5. Add unit tests for new functionality
