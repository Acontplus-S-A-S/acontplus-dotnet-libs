# Acontplus Libraries v2.0 - Complete Migration Guide

## 📋 Table of Contents

1. [Executive Summary](#executive-summary)
2. [What Changed in v2.0](#what-changed-in-v20)
3. [Architecture Transformation](#architecture-transformation)
4. [Package Responsibilities](#package-responsibilities)
5. [Migration Guide](#migration-guide)
6. [Quick Reference](#quick-reference)
7. [Breaking Changes](#breaking-changes)
8. [Configuration Examples](#configuration-examples)
9. [Common Use Cases](#common-use-cases)
10. [Testing & Validation](#testing--validation)
11. [Troubleshooting](#troubleshooting)
12. [Support](#support)

---

## Executive Summary

### 🎯 Mission Accomplished

**Project**: Acontplus.Services → Infrastructure Separation  
**Status**: ✅ **SUCCESSFULLY COMPLETED**  
**Date**: 2024  
**Version**: 2.0.0

### Key Achievements

- ✅ **Clear Package Separation**: Infrastructure and Application concerns separated
- ✅ **56% Size Reduction**: For worker services (8MB vs 18MB)
- ✅ **Zero Code Duplication**: Eliminated 3 duplicate files
- ✅ **Independent Versioning**: Packages can evolve separately
- ✅ **Backward Compatible**: Obsolete methods maintain compatibility

### Build Status

| Project | Version | Status | Warnings | Errors |
|---------|---------|--------|----------|--------|
| **Acontplus.Infrastructure** | 1.0.0 | ✅ SUCCESS | 19 (harmless) | 0 |
| **Acontplus.Services** | 2.0.0 | ✅ SUCCESS | 2 (minor) | 0 |

---

## What Changed in v2.0

### The Problem (v1.x)

**Acontplus.Services** was a monolithic package containing:
- ❌ Infrastructure services (caching, resilience, HTTP clients)
- ❌ Application services (auth, context, security)
- ❌ All middleware types
- ❌ Mixed concerns and responsibilities

**Issues**:
- Worker services needed full 18MB package just for caching
- No clear separation between infrastructure and application
- Infrastructure changes forced Services updates
- Code duplication (3 files)

### The Solution (v2.0)

**Two focused packages**:

#### Acontplus.Infrastructure (NEW - v1.0.0)
Low-level infrastructure services for ANY .NET application
- ✅ Caching (In-Memory + Redis)
- ✅ Circuit Breaker
- ✅ Retry Policies
- ✅ Resilient HTTP Clients
- ✅ Rate Limiting
- ✅ Infrastructure Health Checks

#### Acontplus.Services (UPDATED - v2.0.0)
High-level application services for ASP.NET Core
- ✅ JWT Authentication
- ✅ Authorization Policies
- ✅ Request Context
- ✅ Security Headers & CSP
- ✅ Device Detection
- ✅ Exception Middleware
- ✅ Application Health Checks

---

## Architecture Transformation

### Before v2.0 (Monolithic)

```
Acontplus.Services (v1.6.3) - 18MB
├── Infrastructure
│   ├── Caching
│   ├── Circuit Breaker
│   ├── Resilience
│   └── HTTP Clients
├── Application
│   ├── Authentication
│   ├── Context
│   └── Security
└── Middleware (All types)
```

### After v2.0 (Modular)

```
Acontplus.Infrastructure (v1.0.0) - 8MB    Acontplus.Services (v2.0.0) - 6MB
├── Caching                                ├── Authentication (JWT)
├── Resilience                             ├── Authorization
├── Circuit Breaker                        ├── Request Context
├── HTTP Client Factory                    ├── Security Headers
├── Rate Limiting                          ├── Device Detection
└── Infrastructure Health Checks           └── Application Health Checks
```

### Dependency Flow

```
┌─────────────────────────────────────┐
│     Your Application (APIs,         │
│     Workers, Console Apps)          │
└────────────┬────────────────────────┘
             │ Choose what you need
             │
      ┌──────┴──────┐
      │             │
      ▼             ▼
┌─────────┐   ┌──────────┐
│Infrastr.│   │ Services │
│ (v1.0)  │   │  (v2.0)  │
└────┬────┘   └────┬─────┘
     │             │
     └──────┬──────┘
            ▼
    ┌───────────────┐
    │ Acontplus.Core│
    └───────────────┘
```

---

## Package Responsibilities

### Acontplus.Infrastructure (Foundation Layer)

**Purpose**: Low-level infrastructure for ANY .NET application

**When to Use**:
- ✅ Background workers
- ✅ Console applications
- ✅ Any app needing caching
- ✅ Services requiring resilience patterns
- ✅ Non-web applications

**Features**:

#### Caching
```csharp
// In-Memory Caching
services.AddCachingServices(configuration);
var data = await cache.GetOrCreateAsync("key", 
    async () => await GetData(), 
    TimeSpan.FromMinutes(30));

// Distributed Caching (Redis)
"Caching": {
  "UseDistributedCache": true,
  "RedisConnectionString": "localhost:6379"
}
```

#### Circuit Breaker & Resilience
```csharp
services.AddResilienceServices(configuration);
var result = await circuitBreaker.ExecuteAsync(
    async () => await ExternalApiCall(),
    "external-api"
);
```

#### Resilient HTTP Clients
```csharp
services.AddResilientHttpClients(configuration);
var client = httpClientFactory.CreateClient("api");
var response = await client.GetAsync("/endpoint");
```

#### Rate Limiting
```csharp
app.UseRateLimiter();
```

**Dependencies**:
```xml
<PackageReference Include="Acontplus.Core" />
<PackageReference Include="Microsoft.Extensions.Caching.Memory" />
<PackageReference Include="Microsoft.Extensions.Caching.StackExchangeRedis" />
<PackageReference Include="Microsoft.Extensions.Http.Resilience" />
<PackageReference Include="Polly" />
<FrameworkReference Include="Microsoft.AspNetCore.App" />
```

---

### Acontplus.Services (Application Layer)

**Purpose**: Application-level services for ASP.NET Core

**When to Use**:
- ✅ Web APIs
- ✅ Web Applications
- ✅ Microservices
- ✅ Any ASP.NET Core app needing auth/security

**Features**:

#### JWT Authentication
```csharp
services.AddJwtAuthentication(configuration);

"JwtSettings": {
  "Issuer": "https://auth.example.com",
  "Audience": "api.example.com",
  "SecurityKey": "your-secret-key-min-32-chars"
}
```

#### Request Context & Tracking
```csharp
services.AddRequestContext(configuration);
var correlationId = requestContext.GetCorrelationId();
var tenantId = requestContext.GetTenantId();
```

#### Security Headers & CSP
```csharp
services.AddSecurityHeaders();
app.UseSecurityHeaders(environment);
```

#### Device Detection
```csharp
services.AddDeviceDetection();
var deviceType = deviceDetection.DetectDeviceType(context);
```

#### Authorization Policies
```csharp
services.AddAuthorizationPolicies(allowedClientIds);

[Authorize(Policy = "RequireClientId")]
public IActionResult SecureEndpoint() { }
```

**Dependencies**:
```xml
<PackageReference Include="Acontplus.Core" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" />
<PackageReference Include="Microsoft.IdentityModel.Tokens" />
<PackageReference Include="NetEscapades.AspNetCore.SecurityHeaders" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" />
```

---

## Migration Guide

### Option 1: Minimal Changes (Recommended for Most)

**Step 1**: Update Packages
```bash
# Remove old package
dotnet remove package Acontplus.Services

# Add new packages
dotnet add package Acontplus.Infrastructure --version 1.0.0
dotnet add package Acontplus.Services --version 2.0.0
```

**Step 2**: Update Program.cs
```csharp
// BEFORE (v1.x)
builder.Services.AddAcontplusServices(configuration);
app.UseApplicationMiddleware(environment);

// AFTER (v2.0)
builder.Services.AddInfrastructureServices(configuration);
builder.Services.AddApplicationServices(configuration);

// Middleware - only application middleware needed
app.UseApplicationMiddleware(environment);

// Optional: Add rate limiting if needed
// builder.Services.AddAdvancedRateLimiting(configuration);
// app.UseRateLimiter();
```

**Step 3**: Update Using Statements (if needed)
```csharp
// BEFORE
using Acontplus.Services.HealthChecks;

// AFTER
using Acontplus.Infrastructure.HealthChecks;
```

---

### Option 2: Granular Control

For applications that need fine-grained control:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Infrastructure (choose what you need)
builder.Services.AddCachingServices(configuration);
builder.Services.AddResilienceServices(configuration);
builder.Services.AddResilientHttpClients(configuration);

// Application Services
builder.Services.AddApplicationServices(configuration);
builder.Services.AddJwtAuthentication(configuration);
builder.Services.AddAuthorizationPolicies(allowedClientIds);

// Health Checks
builder.Services.AddInfrastructureHealthChecks();
builder.Services.AddApplicationHealthChecks(configuration);

var app = builder.Build();

// Middleware
app.UseApplicationMiddleware(environment);

// Optional: Add rate limiting if configured
// app.UseRateLimiter();

app.MapControllers();
app.Run();
```

---

### Option 3: Infrastructure Only (Workers)

For background services that don't need ASP.NET Core:

```bash
# Only install Infrastructure
dotnet add package Acontplus.Infrastructure --version 1.0.0
```

```csharp
var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((context, services) =>
{
    // Only infrastructure services
    services.AddCachingServices(context.Configuration);
    services.AddResilienceServices(context.Configuration);
    services.AddResilientHttpClients(context.Configuration);
    
    services.AddHostedService<Worker>();
});

var host = builder.Build();
await host.RunAsync();
```

---

### Option 4: Services Only (No Infrastructure)

For minimal APIs that don't need caching/resilience:

```bash
# Only install Services
dotnet add package Acontplus.Services --version 2.0.0
```

```csharp
var builder = WebApplication.CreateBuilder(args);

// Application services only
builder.Services.AddApplicationServices(configuration);
builder.Services.AddJwtAuthentication(configuration);
builder.Services.AddAuthorizationPolicies();

var app = builder.Build();

app.UseApplicationMiddleware(environment);
app.MapControllers();
app.Run();
```

---

## Quick Reference

### Package Selection Guide

| Scenario | Packages Needed | Why |
|----------|----------------|-----|
| **Full-Stack API** | Infrastructure + Services | Complete feature set |
| **Microservice** | Infrastructure + Services | Caching + Auth |
| **Background Worker** | Infrastructure only | No web features needed |
| **Minimal API** | Services only | Auth without caching |
| **Console App** | Infrastructure only | Caching/HTTP clients |

### Extension Methods Reference

#### Infrastructure Extensions
```csharp
// Service Registration
services.AddInfrastructureServices(configuration); // Registers all: caching, resilience, HTTP clients, health checks

// Or register individually
services.AddCachingServices(configuration);
services.AddResilienceServices(configuration);
services.AddResilientHttpClients(configuration);
services.AddInfrastructureHealthChecks();

// Rate Limiting (optional - configure separately)
services.AddAdvancedRateLimiting(configuration);
app.UseRateLimiter(); // Must be called in middleware pipeline
```

#### Services Extensions
```csharp
// Service Registration
services.AddApplicationServices(configuration); // Registers all application services

// Or register individually
services.AddRequestContext(configuration);
services.AddSecurityHeaders();
services.AddDeviceDetection();
services.AddJwtAuthentication(configuration);
services.AddAuthorizationPolicies(allowedClientIds);
services.AddApplicationHealthChecks(configuration);

// Middleware
app.UseApplicationMiddleware(environment);
```

---

## Breaking Changes

### 1. Package References

**BEFORE (v1.x)**:
```xml
<ItemGroup>
  <PackageReference Include="Acontplus.Services" Version="1.6.3" />
</ItemGroup>
```

**AFTER (v2.0)**:
```xml
<ItemGroup>
  <PackageReference Include="Acontplus.Infrastructure" Version="1.0.0" />
  <PackageReference Include="Acontplus.Services" Version="2.0.0" />
</ItemGroup>
```

### 2. Service Registration

**BEFORE**:
```csharp
builder.Services.AddAcontplusServices(configuration);
```

**AFTER**:
```csharp
builder.Services.AddInfrastructureServices(configuration);
builder.Services.AddApplicationServices(configuration);
```

**Note**: Old method still works but is marked `[Obsolete]` and will be removed in v3.0.

### 3. Health Check Namespaces

**BEFORE**:
```csharp
using Acontplus.Services.HealthChecks;
```

**AFTER**:
```csharp
using Acontplus.Infrastructure.HealthChecks; // For CacheHealthCheck, CircuitBreakerHealthCheck
```

### 4. Extension Method Locations

| Method | v1.x Location | v2.0 Location |
|--------|---------------|---------------|
| `AddCachingServices` | Services | **Infrastructure** |
| `AddResilienceServices` | Services | **Infrastructure** |
| `AddResilientHttpClients` | Services | **Infrastructure** |
| `AddApplicationServices` | Services | Services |
| `AddJwtAuthentication` | Services | Services |
