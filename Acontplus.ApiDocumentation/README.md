# Acontplus.ApiDocumentation

This package provides a standardized and simple way to add API versioning and Swagger/OpenAPI documentation to an ASP.NET Core web API.

---

## 1. Installation

Install the package into your ASP.NET Core project using the .NET CLI:

```bash
dotnet add package Acontplus.ApiDocumentation
````

## 2\. Usage

### Step 2.1: Enable XML Documentation

To display your code comments (`<summary>`, `<remarks>`, etc.) in the Swagger UI, enable XML documentation generation in your API's `.csproj` file.

```xml
<PropertyGroup>
  <TargetFramework>net9.0</TargetFramework>
  <Nullable>enable</Nullable>
  <ImplicitUsings>enable</ImplicitUsings>
  
  <!-- This line enables XML documentation generation -->
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
</PropertyGroup>
```

### Step 2.2: Configure `appsettings.json`

Add a `SwaggerInfo` section to your `appsettings.json` file. This information will be displayed at the top of your Swagger documentation page. If this section is omitted, default values will be used.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "SwaggerInfo": {
    "ContactName": "Acontplus Development Team",
    "ContactEmail": "proyectos@acontplus.com",
    "ContactUrl": "[https://acontplus.com.ec](https://acontplus.com.ec)",
    "LicenseName": "MIT License",
    "LicenseUrl": "[https://opensource.org/licenses/MIT](https://opensource.org/licenses/MIT)"
  }
}
```

### Step 2.3: Configure `Program.cs`

In your `Program.cs` file, call the extension methods to register the services and configure the middleware.

```csharp
using Acontplus.ApiDocumentation; // Add this using statement

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// 1. Add API versioning and documentation services from the package.
builder.Services.AddApiVersioningAndDocumentation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // 2. Use the Swagger and versioning middleware from the package.
    app.UseApiVersioningAndDocumentation();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

### Step 2.4: Decorate Your Controllers

Apply versioning attributes to your API controllers. The `[ApiVersion]` attribute specifies which version(s) the controller belongs to. The route template must include the `v{version:apiVersion}` segment.

#### Example: Version 1.0 Controller

```csharp
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace MyApi.Controllers.v1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class DataController : ControllerBase
{
    /// <summary>
    /// Gets the version 1.0 data.
    /// </summary>
    /// <remarks>
    /// This is the first version of the endpoint.
    /// </remarks>
    /// <returns>A sample string for V1.</returns>
    [HttpGet]
    public string Get()
    {
        return "This is response from API version 1.0";
    }
}
```

#### Example: Version 2.0 Controller (with a deprecated version)

```csharp
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace MyApi.Controllers.v2;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("2.0")]
[ApiVersion("1.5", Deprecated = true)] // This version is supported but marked as deprecated
public class DataController : ControllerBase
{
    /// <summary>
    /// Gets the version 2.0 data.
    /// </summary>
    /// <remarks>
    /// This is the newer, more complete endpoint.
    /// </remarks>
    /// <returns>A sample string for V2.</returns>
    [HttpGet]
    public string Get()
    {
        return "This is the enhanced response from API version 2.0";
    }
}
```

-----

## Author & Company

* **Author:** [Ivan Paz](https://linktr.ee/iferpaz7)
* **Company:** [Acontplus S.A.S.](https://acontplus.com.ec)
* **Contact:** [proyectos@acontplus.com](mailto:proyectos@acontplus.com)

## License

This project is licensed under the **MIT License**.