# Acontplus.Services Usage Guide

Acontplus.Services provides API services, authentication, claims, middleware, and configuration for robust APIs.

## 📦 Installation

```bash
Install-Package Acontplus.Services
```

## 🚀 Features
- Platform-aware configuration loading
- Claims-based user context extensions
- Identity & JWT authentication
- Global exception handling middleware
- Mobile request detection middleware

## 🛠️ Basic Usage

### Add JWT Authentication
```csharp
services.AddIdentityService(Configuration); // or AddIdentityServiceStrict
```

### Use ClaimsPrincipal Extensions
```csharp
var username = user.GetUsername();
var email = user.GetEmail();
```

### Global Exception Middleware
```csharp
app.UseAcontplusExceptionHandling();
```

## 📖 See Also
- [API Reference](../Home.md)
- [Project README](https://github.com/Acontplus-S-A-S/acontplus-dotnet-libs/blob/main/src/Acontplus.Services/README.md) 