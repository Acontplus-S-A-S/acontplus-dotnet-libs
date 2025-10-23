# Acontplus.ApiDocumentation

[![NuGet](https://img.shields.io/nuget/v/Acontplus.ApiDocumentation.svg)](https://www.nuget.org/packages/Acontplus.ApiDocumentation)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

A .NET 9+ library for standardized API versioning and OpenAPI/Swagger documentation. Supports both controller-based and minimal APIs, with advanced customization and versioning support.

---

## 🚀 Features

- **API Versioning** - Flexible, multi-version API support
- **Swagger/OpenAPI UI** - Beautiful, customizable API docs
- **Minimal & Controller API Support** - Works with both paradigms
- **JWT Bearer Auth UI** - Secure your APIs and test with tokens
- **XML Comments** - Show your code docs in Swagger UI
- **Custom Metadata** - Configure contact, license, and more from `appsettings.json`
- **.NET 9+ Ready** - Fast and future-proof

---

## 📦 Installation

```bash
dotnet add package Acontplus.ApiDocumentation
```

---

## 🎯 Quick Start (Controller-based API)

1. **Enable XML Documentation** in your `.csproj`:
   ```xml
   <GenerateDocumentationFile>true</GenerateDocumentationFile>
   ```

2. **Add SwaggerInfo to `appsettings.json`**:
   ```json
   "SwaggerInfo": {
     "ContactName": "Acontplus Development Team",
     "ContactEmail": "proyectos@acontplus.com",
     "ContactUrl": "https://www.acontplus.com",
     "LicenseName": "MIT License",
     "LicenseUrl": "https://opensource.org/licenses/MIT"
   }
   ```

3. **Register services in `Program.cs`:**
   ```csharp
   using Acontplus.ApiDocumentation;

   var builder = WebApplication.CreateBuilder(args);
   builder.Services.AddControllers();
   builder.Services.AddApiVersioningAndDocumentation();

   var app = builder.Build();

   if (app.Environment.IsDevelopment())
   {
       app.UseApiVersioningAndDocumentation();
   }

   app.UseHttpsRedirection();
   app.UseAuthorization();
   app.MapControllers();
   app.Run();
   ```

---

## 🎯 Quick Start (Minimal API)

1. **Add the package:**
   ```bash
   dotnet add package Microsoft.AspNetCore.OpenApi
   ```

2. **Register OpenAPI in `Program.cs`:**
   ```csharp
   var builder = WebApplication.CreateBuilder(args);
   builder.Services.AddOpenApi();

   var app = builder.Build();

   if (app.Environment.IsDevelopment())
   {
       app.MapOpenApi();
   }

   app.MapGet("/", () => "Hello world!");
   app.Run();
   ```

---

## 🗂️ API Versioning Example

```csharp
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class DataController : ControllerBase
{
    [HttpGet]
    public string Get() => "This is response from API version 1.0";
}
```

---

## ⚡ Migration & Troubleshooting Notes

- **Do NOT reference `Microsoft.OpenApi` directly.**
  - For controller-based APIs, use `Swashbuckle.AspNetCore.SwaggerGen` and `Swashbuckle.AspNetCore.SwaggerUI` only. These packages bring the correct OpenAPI model types.
  - For minimal APIs, use `Microsoft.AspNetCore.OpenApi`.
- If you see errors like `The type or namespace name 'OpenApiInfo' could not be found`:
  - Make sure you have **not** added `Microsoft.OpenApi` directly.
  - Ensure you have the following using statement:
    ```csharp
    using Microsoft.OpenApi.Models;
    ```
  - Swashbuckle will bring in the correct version of `Microsoft.OpenApi.Models`.
- For advanced UI and controller-based APIs, continue using Swashbuckle (`AddSwaggerGen`, `UseSwaggerUI`).
- For minimal APIs, use the new built-in OpenAPI support in .NET 9+.

---

## 🛠️ Advanced Customization

- **Custom Swagger Info**: Edit the `SwaggerInfo` section in `appsettings.json`.
- **JWT Bearer Auth**: UI is pre-configured for Bearer tokens.
- **Multiple API Versions**: Decorate controllers with `[ApiVersion]` and use versioned routes.
- **XML Comments**: Enable in `.csproj` for rich documentation.

---

## 📄 License & Info

- **License:** [MIT](../LICENSE)
- **Authors:** Ivan Paz
- **Company:** [Acontplus](https://www.acontplus.com)
- **Repository:** [github.com/Acontplus-S-A-S/acontplus-dotnet-libs](https://github.com/acontplus/acontplus-dotnet-libs)
- **Contact:** [proyectos@acontplus.com](mailto:proyectos@acontplus.com)

---

© 2024 Acontplus All rights reserved.
