# Acontplus.ApiDocumentation Usage Guide

Acontplus.ApiDocumentation provides standardized API versioning and OpenAPI/Swagger documentation for .NET APIs.

## 📦 Installation

```bash
Install-Package Acontplus.ApiDocumentation
```

## 🚀 Features
- API versioning (multi-version support)
- Swagger/OpenAPI UI
- Minimal & controller API support
- JWT Bearer Auth UI
- XML comments integration
- Custom metadata from appsettings.json

## 🛠️ Basic Usage

### Register Services
```csharp
services.AddApiVersioningAndDocumentation();
```

### Configure SwaggerInfo in appsettings.json
```json
"SwaggerInfo": {
  "ContactName": "Acontplus Development Team",
  "ContactEmail": "proyectos@acontplus.com",
  "ContactUrl": "https://acontplus.com.ec",
  "LicenseName": "MIT License",
  "LicenseUrl": "https://opensource.org/licenses/MIT"
}
```

## 📖 See Also
- [API Reference](../Home.md)
- [Project README](../../src/Acontplus.ApiDocumentation/README.md) 