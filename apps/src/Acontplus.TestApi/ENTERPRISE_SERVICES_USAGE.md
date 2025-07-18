# Enterprise Services Usage Guide

This guide demonstrates how to use the modernized Acontplus.Services library with enterprise patterns in a real application.

## 🚀 Quick Setup

### 1. Configuration in Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);

// Configure enterprise services with modern patterns
builder.Services.AddEnterpriseServices(builder.Configuration);
builder.Services.AddEnterpriseAuthorizationPolicies(new List<string> 
{ 
    "web-app", "mobile-app", "admin-portal", "test-client" 
});
builder.Services.AddEnterpriseMvc();
builder.Services.AddEnterpriseHealthChecks(builder.Configuration);

var app = builder.Build();

// Use enterprise middleware pipeline
app.UseEnterpriseMiddleware(app.Environment);

// Map health checks
app.MapHealthChecks("/health");

app.MapControllers();
await app.RunAsync();
```

### 2. Configuration in appsettings.json

```json
{
  "RequestContext": {
    "EnableSecurityHeaders": true,
    "FrameOptionsDeny": true,
    "ReferrerPolicy": "strict-origin-when-cross-origin",
    "RequireClientId": false,
    "AnonymousClientId": "test-client",
    "AllowedClientIds": [
      "web-app",
      "mobile-app", 
      "admin-portal",
      "test-client"
    ]
  }
}
```

## 📋 Available Endpoints

### Controller-Based Examples

#### 1. Request Context Information
```http
GET /api/EnterpriseExamples/context
```

**Response:**
```json
{
  "context": {
    "requestId": "12345-67890",
    "correlationId": "abc-def-123",
    "tenantId": "tenant-001",
    "clientId": "web-app",
    "deviceType": "Desktop",
    "isMobileRequest": false,
    "timestamp": "2024-01-15T10:30:00Z"
  },
  "isMobile": false,
  "deviceType": "Desktop",
  "message": "Request context successfully retrieved"
}
```

#### 2. Device Detection
```http
GET /api/EnterpriseExamples/device
```

**Response:**
```json
{
  "userAgent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
  "detectedType": "Desktop",
  "isMobile": false,
  "capabilities": {
    "type": "Desktop",
    "isMobile": false,
    "isTablet": false,
    "supportsTouch": false,
    "operatingSystem": "Windows",
    "browser": "Chrome",
    "version": "120.0"
  },
  "message": "Device information successfully detected"
}
```

#### 3. Security Headers Information
```http
GET /api/EnterpriseExamples/security-headers
```

**Response:**
```json
{
  "recommendedHeaders": {
    "X-Content-Type-Options": "nosniff",
    "X-Frame-Options": "DENY",
    "X-XSS-Protection": "1; mode=block",
    "Referrer-Policy": "strict-origin-when-cross-origin",
    "Permissions-Policy": "camera=(), microphone=(), geolocation=(), payment=()"
  },
  "cspNonce": "abc123def456",
  "environment": "Development",
  "message": "Security headers information retrieved"
}
```

#### 4. Secure Endpoint (Requires Client-ID)
```http
GET /api/EnterpriseExamples/secure
Headers:
  Client-Id: web-app
```

**Response:**
```json
{
  "clientId": "web-app",
  "message": "Access granted to secure endpoint",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

#### 5. Tenant-Specific Data
```http
GET /api/EnterpriseExamples/tenant-data
Headers:
  Tenant-Id: tenant-001
```

**Response:**
```json
{
  "tenantId": "tenant-001",
  "data": "Tenant-specific data for tenant-001",
  "message": "Tenant data retrieved successfully"
}
```

#### 6. Mobile-Only Endpoint
```http
GET /api/EnterpriseExamples/mobile-only
Headers:
  Device-Type: Mobile
```

**Response:**
```json
{
  "deviceType": "Mobile",
  "message": "Mobile-only access granted",
  "features": [
    "Touch optimized",
    "Offline sync", 
    "Push notifications"
  ]
}
```

#### 7. Desktop-Only Endpoint
```http
GET /api/EnterpriseExamples/desktop-only
Headers:
  Device-Type: Desktop
```

**Response:**
```json
{
  "message": "Desktop access granted",
  "features": [
    "Full keyboard support",
    "Multi-window",
    "Advanced reporting"
  ]
}
```

#### 8. Validation Example
```http
POST /api/EnterpriseExamples/validate
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john@example.com",
  "age": 30,
  "phoneNumber": "+1234567890"
}
```

**Response:**
```json
{
  "message": "Data validation passed",
  "receivedData": {
    "name": "John Doe",
    "email": "john@example.com",
    "age": 30,
    "phoneNumber": "+1234567890"
  }
}
```

### Minimal API Examples

#### 1. Request Context (Minimal API)
```http
GET /api/enterprise-minimal/context
```

#### 2. Device Information (Minimal API)
```http
GET /api/enterprise-minimal/device
```

#### 3. Secure Data (Minimal API)
```http
GET /api/enterprise-minimal/secure
Headers:
  Client-Id: web-app
```

#### 4. Tenant-Specific Data (Minimal API)
```http
GET /api/enterprise-minimal/tenant/tenant-001
Headers:
  Tenant-Id: tenant-001
```

#### 5. Mobile Features (Minimal API)
```http
GET /api/enterprise-minimal/mobile-features
Headers:
  Device-Type: Mobile
```

#### 6. Health Summary (Minimal API)
```http
GET /api/enterprise-minimal/health-summary
```

**Response:**
```json
{
  "services": {
    "requestContext": {
      "status": "Healthy",
      "message": "Service is operational"
    },
    "deviceDetection": {
      "status": "Healthy", 
      "message": "Service is operational"
    },
    "securityHeaders": {
      "status": "Healthy",
      "message": "Service is operational"
    }
  },
  "timestamp": "2024-01-15T10:30:00Z",
  "apiType": "Minimal API",
  "message": "Enterprise services health summary"
}
```

## 🔒 Authorization Policies

### Available Policies

1. **RequireClientId** - Requires valid Client-Id header
2. **RequireTenant** - Requires valid Tenant-Id header
3. **MobileOnly** - Only allows mobile devices
4. **MobileAndTablet** - Allows mobile and tablet devices
5. **DesktopOnly** - Only allows desktop devices
6. **KnownDevicesOnly** - Excludes unknown device types

### Usage in Controllers

```csharp
[Authorize(Policy = "RequireClientId")]
[HttpGet("secure")]
public IActionResult SecureEndpoint()
{
    // Your secure logic here
}

[Authorize(Policy = "MobileOnly")]
[HttpGet("mobile-features")]
public IActionResult MobileFeatures()
{
    // Mobile-specific features
}
```

### Usage in Minimal APIs

```csharp
app.MapGet("/secure", GetSecureData)
   .RequireAuthorization("RequireClientId");

app.MapGet("/mobile", GetMobileData)
   .RequireAuthorization("MobileOnly");
```

## 🏥 Health Checks

### Health Check Endpoint
```http
GET /health
```

**Response:**
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.0123456",
  "entries": {
    "request-context": {
      "status": "Healthy",
      "description": "Request context service is operational",
      "duration": "00:00:00.0012345"
    },
    "security-headers": {
      "status": "Healthy", 
      "description": "Security header service is operational",
      "duration": "00:00:00.0012345"
    },
    "device-detection": {
      "status": "Healthy",
      "description": "Device detection service is operational", 
      "duration": "00:00:00.0012345"
    },
    "memory": {
      "status": "Healthy",
      "description": "Memory usage is within acceptable limits",
      "duration": "00:00:00.0012345"
    }
  }
}
```

## 🛠️ Service Injection Examples

### In Controllers

```csharp
public class MyController : ControllerBase
{
    private readonly IRequestContextService _requestContext;
    private readonly IDeviceDetectionService _deviceDetection;
    private readonly ISecurityHeaderService _securityHeaders;

    public MyController(
        IRequestContextService requestContext,
        IDeviceDetectionService deviceDetection,
        ISecurityHeaderService securityHeaders)
    {
        _requestContext = requestContext;
        _deviceDetection = deviceDetection;
        _securityHeaders = securityHeaders;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var context = _requestContext.GetRequestContext();
        var isMobile = _requestContext.IsMobileRequest();
        var deviceType = _deviceDetection.DetectDeviceType(HttpContext);
        
        return Ok(new { context, isMobile, deviceType });
    }
}
```

### In Minimal APIs

```csharp
app.MapGet("/context", (IRequestContextService requestContext) =>
{
    var context = requestContext.GetRequestContext();
    return Results.Ok(context);
});

app.MapGet("/device", (IDeviceDetectionService deviceDetection, HttpContext httpContext) =>
{
    var deviceType = deviceDetection.DetectDeviceType(httpContext);
    return Results.Ok(new { deviceType });
});
```

## 🔧 Advanced Configuration

### Custom Client IDs

```csharp
builder.Services.AddEnterpriseAuthorizationPolicies(new List<string>
{
    "web-client",
    "mobile-app-ios",
    "mobile-app-android", 
    "admin-dashboard",
    "reporting-service"
});
```

### Environment-Specific Settings

```json
{
  "RequestContext": {
    "EnableSecurityHeaders": true,
    "FrameOptionsDeny": true,
    "ReferrerPolicy": "strict-origin-when-cross-origin",
    "RequireClientId": true,
    "AllowedClientIds": ["production-client"]
  }
}
```

### Custom Middleware Order

```csharp
// Manual middleware configuration (if needed)
app.UseSecurityHeaders(app.Environment);
app.UseMiddleware<CspNonceMiddleware>();
app.UseMiddleware<RequestContextMiddleware>();
app.UseAcontplusExceptionHandling();
```

## 🧪 Testing Examples

### Testing with Headers

```bash
# Test with Client-ID
curl -H "Client-Id: web-app" http://localhost:5000/api/EnterpriseExamples/secure

# Test with Tenant-ID
curl -H "Tenant-Id: tenant-001" http://localhost:5000/api/EnterpriseExamples/tenant-data

# Test with Device Type
curl -H "Device-Type: Mobile" http://localhost:5000/api/EnterpriseExamples/mobile-only

# Test with multiple headers
curl -H "Client-Id: web-app" -H "Tenant-Id: tenant-001" -H "Device-Type: Desktop" \
     http://localhost:5000/api/EnterpriseExamples/context
```

### Testing Validation

```bash
# Valid data
curl -X POST -H "Content-Type: application/json" \
     -d '{"name":"John Doe","email":"john@example.com","age":30}' \
     http://localhost:5000/api/EnterpriseExamples/validate

# Invalid data (will trigger validation errors)
curl -X POST -H "Content-Type: application/json" \
     -d '{"name":"Jo","email":"invalid-email","age":15}' \
     http://localhost:5000/api/EnterpriseExamples/validate
```

## 📊 Monitoring and Observability

### Request Logging
All requests are automatically logged with:
- Request ID and Correlation ID
- Performance metrics
- Device information
- Client and tenant context

### Error Handling
Global exception handling provides:
- Standardized error responses
- Correlation ID tracking
- Environment-appropriate error details
- Structured logging

### Security Headers
Automatically applied headers:
- X-Content-Type-Options: nosniff
- X-Frame-Options: DENY
- X-XSS-Protection: 1; mode=block
- Referrer-Policy: strict-origin-when-cross-origin
- Permissions-Policy: camera=(), microphone=(), geolocation=(), payment=()

## 🎯 Best Practices

1. **Always use correlation IDs** for request tracking
2. **Validate client IDs** in production environments
3. **Implement tenant isolation** for multi-tenant applications
4. **Use device-aware policies** for mobile applications
5. **Monitor health checks** in production
6. **Configure security headers** appropriately for your environment
7. **Use structured logging** with request context
8. **Test authorization policies** thoroughly

## 🔍 Troubleshooting

### Common Issues

1. **Missing Client-ID**: Ensure Client-Id header is sent with requests
2. **Invalid Tenant-ID**: Verify Tenant-Id matches user's tenant
3. **Device Detection**: Check User-Agent or Device-Type headers
4. **Authorization Failures**: Verify policy configuration and headers
5. **Health Check Failures**: Check service dependencies and configuration

### Debug Endpoints

- `/health` - Overall system health
- `/api/enterprise-minimal/health-summary` - Service-specific health
- `/api/EnterpriseExamples/context` - Request context debugging
- `/api/EnterpriseExamples/device` - Device detection debugging

This comprehensive guide demonstrates all the enterprise service patterns available in the modernized Acontplus.Services library!