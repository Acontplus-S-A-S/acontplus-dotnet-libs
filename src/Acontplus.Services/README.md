# Acontplus.Services

Acontplus.Services is a .NET NuGet package that provides a collection of reusable, common services and extensions designed to streamline the development of modern .NET applications. This package aims to offer robust solutions for configuration management, user context handling, identity and authentication setup, and centralized exception handling.

## Table of Contents

  - [Features](https://www.google.com/search?q=%23features)
  - [Installation](https://www.google.com/search?q=%23installation)
  - [Usage](https://www.google.com/search?q=%23usage)
      - [AppConfiguration](https://www.google.com/search?q=%23appconfiguration)
      - [ClaimsPrincipalExtensions](https://www.google.com/search?q=%23claimsprincipalextensions)
      - [IdentityServiceExtensions](https://www.google.com/search?q=%23identityserviceextensions)
      - [UserContext](https://www.google.com/search?q=%23usercontext)
      - [ExceptionMiddleware](https://www.google.com/search?q=%23exceptionmiddleware)
      - [MobileRequestMiddleware](https://www.google.com/search?q=%23mobilerequestmiddleware)
  - [Contributing](https://www.google.com/search?q=%23contributing)
  - [License](https://www.google.com/search?q=%23license)
  - [Author](https://www.google.com/search?q=%23author)
  - [Company](https://www.google.com/search?q=%23company)

## Features

This package includes the following key features:

  * **Platform-aware Configuration Loading**: Dynamically loads application settings and shared configurations based on the operating system and environment.
  * **Claims-based User Context Extensions**: Provides convenient extension methods for `ClaimsPrincipal` to easily extract common user information (username, email, role, user ID) and generic claim values.
  * **Identity and Authentication Setup**: Simplifies the integration of JWT Bearer authentication with flexible and strict validation options.
  * **User Context Service**: A service that abstracts access to user claims from the `HttpContext`, making it easier to retrieve current user details.
  * **Global Exception Handling Middleware**: A robust middleware for logging and handling unhandled exceptions, providing clear error responses in both development and production environments.
  * **Mobile Request Detection Middleware**: Identifies incoming requests from mobile devices based on User-Agent headers and custom `X-Is-Mobile` headers.

## Installation

You can install `Acontplus.Services` via NuGet Package Manager or the .NET CLI.

**NuGet Package Manager:**

```powershell
Install-Package Acontplus.Services
```

**dotnet CLI:**

```bash
dotnet add package Acontplus.Services
```

## Usage

Here's how to integrate and use the services provided by this package in your .NET application.

### AppConfiguration

The `AppConfiguration` static class helps in loading application settings, including environment-specific and shared configurations.

**Configuration Files:**

Ensure you have `appsettings.json`, `appsettings.{environment}.json` (e.g., `appsettings.Development.json`), and optionally `sharedsettings.{environment}.json` files.

Example `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "JwtSettings": {
    "SecurityKey": "YourSuperSecretKeyThatIsLongEnoughAndSecure",
    "Issuer": "YourAppIssuer",
    "Audience": "YourAppAudience"
  },
  "SharedPaths": {
    "Windows": "C:\\ProgramData\\YourApp\\SharedSettings",
    "Linux": "/etc/yourapp/sharedsettings",
    "OSX": "/Library/Application Support/YourApp/SharedSettings"
  }
}
```

Example `sharedsettings.Development.json` (located in the path specified by `SharedPaths`):

```json
{
  "SomeSharedSetting": "ValueFromSharedSettingsDevelopment"
}
```

**Loading Configuration:**

In your `Program.cs` or startup class:

```csharp
using Acontplus.Services.Configuration;
using Microsoft.Extensions.Configuration;

public class Program
{
    public static void Main(string[] args)
    {
        var config = AppConfiguration.Load();

        // Now 'config' contains all loaded settings, including shared ones
        var securityKey = config["JwtSettings:SecurityKey"];
        var someSharedSetting = config["SomeSharedSetting"];

        CreateHostBuilder(args, config).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration config) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .ConfigureAppConfiguration((hostingContext, configurationBuilder) =>
            {
                configurationBuilder.AddConfiguration(config); // Add the pre-loaded config
            });
}

```

You can also override the shared folder path using the environment variable `SHARED_SETTINGS_PATH`.

### ClaimsPrincipalExtensions

These extension methods simplify accessing claims from a `ClaimsPrincipal` object.

```csharp
using System.Security.Claims;
using Acontplus.Services.Extensions;

public class MyService
{
    public void ProcessUser(ClaimsPrincipal user)
    {
        var username = user.GetUsername();
        var email = user.GetEmail();
        var roleName = user.GetRoleName();
        var userId = user.GetUserId(); // int type

        // Get custom claims
        int companyId = user.GetClaimValue<int>("companyId");
        string idCardCompany = user.GetClaimValue<string>("idCardCompany");
        bool isActive = user.GetClaimValue<bool>("isActive");
        Guid tenantId = user.GetClaimValue<Guid>("tenantId");

        Console.WriteLine($"User: {username}, Email: {email}, Role: {roleName}, ID: {userId}");
        Console.WriteLine($"Company ID: {companyId}, ID Card Company: {idCardCompany}, Is Active: {isActive}, Tenant ID: {tenantId}");
    }
}
```

### IdentityServiceExtensions

These extension methods configure JWT Bearer authentication and authorization for your application.

**1. `AddIdentityService` (Flexible Validation):**

Use this for basic JWT validation where issuer and audience are not strictly validated, but a `SecurityKey` is required.

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Acontplus.Services.Extensions;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddIdentityService(Configuration);
        // Other services...
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        // Other middleware...
    }
}
```

**2. `AddIdentityServiceStrict` (Strict Validation):**

Use this when you need strict validation of `Issuer` and `Audience` in your JWT tokens, in addition to the `SecurityKey`.

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Acontplus.Services.Extensions;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddIdentityServiceStrict(Configuration);
        // Other services...
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        // Other middleware...
    }
}
```

Ensure your `appsettings.json` (or environment-specific files) contains the `JwtSettings`:

```json
{
  "JwtSettings": {
    "SecurityKey": "YourSuperSecretKeyThatIsLongEnoughAndSecureAndComplexForProduction",
    "Issuer": "https://yourdomain.com",
    "Audience": "your_api_client"
  }
}
```

### UserContext

The `UserContext` service provides an injectable way to access current user claims from anywhere in your application, leveraging `IHttpContextAccessor`.

**1. Register the Service:**

```csharp
using Microsoft.Extensions.DependencyInjection;
using Acontplus.Services.Extensions; // For IUserContext and UserContext
using Microsoft.AspNetCore.Http; // For IHttpContextAccessor

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpContextAccessor(); // Required for UserContext
        services.AddScoped<IUserContext, UserContext>();
        // Other services...
    }
}
```

**2. Inject and Use:**

```csharp
using Acontplus.Services.Extensions; // For IUserContext

public class SomeApplicationService
{
    private readonly IUserContext _userContext;

    public SomeApplicationService(IUserContext userContext)
    {
        _userContext = userContext;
    }

    public void DoSomethingBasedOnUser()
    {
        var userId = _userContext.GetUserId();
        var userName = _userContext.GetUserName();
        var companyId = _userContext.GetClaimValue<int>("companyId");

        Console.WriteLine($"Service call by User ID: {userId}, Name: {userName}, Company ID: {companyId}");
    }
}
```

### ExceptionMiddleware

This middleware provides global exception handling, logging errors, and returning standardized problem details.

**1. Register the Middleware:**

```csharp
using Acontplus.Services.Middleware;

public class Startup
{
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Must be registered early in the pipeline
        app.UseMiddleware<ExceptionMiddleware>();

        // Other middleware like HSTS, HTTPS Redirection, Static Files, etc.
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
```

When an unhandled exception occurs, the middleware will log the exception (including request body and query string for POST/PUT requests) and return a JSON response conforming to `ProblemDetails`. In development, it includes the stack trace.

### MobileRequestMiddleware

This middleware detects if a request originates from a mobile device based on the User-Agent header or a custom `X-Is-Mobile` header.

**1. Register the Middleware:**

```csharp
using Acontplus.Services.Middleware;

public class Startup
{
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseMiddleware<MobileRequestMiddleware>();
        // Other middleware...
    }
}
```

**2. Access in your Controllers/Services:**

You can access the detected mobile status via `HttpContext.Items`.

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

[ApiController]
[Route("[controller]")]
public class MyController : ControllerBase
{
    [HttpGet("check-mobile")]
    public IActionResult CheckMobile()
    {
        var fromMobileHeader = HttpContext.Items["FromMobile"] as bool? ?? false;
        var isMobileUserAgent = HttpContext.Items["IsMobileUserAgent"] as bool? ?? false;

        return Ok(new { FromMobileHeader = fromMobileHeader, IsMobileUserAgent = isMobileUserAgent });
    }
}
```

A client can set the `X-Is-Mobile` header to `true` to explicitly indicate a mobile request:

```
GET /api/check-mobile
X-Is-Mobile: true
User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36
```

-----

## Contributing

We welcome contributions\! Please submit any issues or feature requests via our GitHub repository, or feel free to fork the project and submit pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE](https://www.google.com/search?q=LICENSE) file for details.

-----

## Author

[Ivan Paz](https://linktr.ee/iferpaz7)

## Company

[Acontplus S.A.S.](https://acontplus.com.ec)

-----

**MIT License**

Copyright (c) 2025 [Ivan Paz]

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

-----