# Acontplus.Services

[![NuGet](https://img.shields.io/nuget/v/Acontplus.Services.svg)](https://www.nuget.org/packages/Acontplus.Services)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

A modern .NET 9+ library for API services, authentication, claims, middleware, and configuration. Includes JWT, user context, and exception handling for robust enterprise APIs.

## 🚀 Features

- **Platform-aware Configuration Loading** - Dynamic app and shared config loading
- **Claims-based User Context Extensions** - Easy access to user info and claims
- **Identity & JWT Authentication** - Flexible and strict JWT setup
- **User Context Service** - Abstracted access to user claims
- **Global Exception Handling Middleware** - Robust error logging and API error responses
- **Mobile Request Detection Middleware** - Detects mobile clients via headers

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
<ItemGroup>
  <PackageReference Include="Acontplus.Services" Version="1.0.12" />
</ItemGroup>
```

## 🎯 Quick Start

### 1. Add JWT Authentication
```csharp
using Microsoft.Extensions.DependencyInjection;
using Acontplus.Services.Extensions;

public void ConfigureServices(IServiceCollection services)
{
    services.AddIdentityService(Configuration); // or AddIdentityServiceStrict
}

public void Configure(IApplicationBuilder app)
{
    app.UseAuthentication();
    app.UseAuthorization();
}
```

### 2. Use ClaimsPrincipal Extensions
```csharp
using System.Security.Claims;
using Acontplus.Services.Extensions;

var username = user.GetUsername();
var email = user.GetEmail();
var userId = user.GetUserId();
```

### 3. Global Exception Middleware
```csharp
app.UseMiddleware<ApiExceptionMiddleware>();
```

## 🔧 Advanced Usage

### AppConfiguration
```csharp
using Acontplus.Services.Configuration;
var config = AppConfiguration.Load();
```

### User Context Service
```csharp
using Acontplus.Services.Extensions;
services.AddHttpContextAccessor();
services.AddScoped<IUserContext, UserContext>();
```

## 📚 API Documentation

- `AppConfiguration` - Platform/environment config loader
- `ClaimsPrincipalExtensions` - User/claim helpers
- `ApiExceptionMiddleware` - Centralized error handling
- `UserContext` - User info abstraction
- `RequestContextOptions` - Request context config

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details.

### Development Setup
```bash
git clone https://github.com/Acontplus-S-A-S/acontplus-dotnet-libs.git
cd acontplus-dotnet-libs
dotnet restore
dotnet build
```

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

- 📧 Email: proyectos@acontplus.com
- 🐛 Issues: [GitHub Issues](https://github.com/Acontplus-S-A-S/acontplus-dotnet-libs/issues)
- 📖 Documentation: [Wiki](https://github.com/Acontplus-S-A-S/acontplus-dotnet-libs/wiki)

## 👨‍💻 Author

**Ivan Paz** - [@iferpaz7](https://linktr.ee/iferpaz7)

## 🏢 Company

**[Acontplus S.A.S.](https://acontplus.com.ec)** - Enterprise software solutions

---

**Built with ❤️ for the .NET community**