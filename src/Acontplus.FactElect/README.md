# Acontplus.FactElect

[![NuGet](https://img.shields.io/nuget/v/Acontplus.FactElect.svg)](https://www.nuget.org/packages/Acontplus.FactElect)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A comprehensive .NET 9+ library for electronic invoicing and digital document handling in Ecuador, providing models, services, and utilities to facilitate integration with SRI (Servicio de Rentas Internas) systems.

## 🚀 Features

- Electronic document models (invoices, credit notes, etc.)
- XML document generation, validation, and parsing
- SRI (Ecuadorian Tax Authority) web service integration
- Document validation and verification
- CAPTCHA handling for human verification processes
- ID card (cédula) and RUC validation
- XML to HTML/PDF document conversion
- Token-based authentication management
- Dependency Injection support

## 📦 Installation

### NuGet Package Manager
```bash
Install-Package Acontplus.FactElect
```

### .NET CLI
```bash
dotnet add package Acontplus.FactElect
```

### PackageReference
```xml
<ItemGroup>
  <PackageReference Include="Acontplus.FactElect" Version="1.0.17" />
</ItemGroup>
```

## 🎯 Quick Start

### 1. Register Services
```csharp
// In Startup.cs or Program.cs
// Services are registered manually or through your DI container
// Example:
services.AddSingleton<ICedulaService, CedulaService>();
services.AddSingleton<IRucService, RucService>();
// Add other services as needed
```

### 2. Configuration in appsettings.json
```json
{
  "FactElect": {
    "Environment": "Development",
    "ValidateBeforeSend": true,
    "DefaultTimeoutSeconds": 30,
    "DocumentStoragePath": "Documents",
    "CompanyRuc": "0991234567001",
    "CompanyLegalName": "ACME COMPANY S.A.",
    "CompanyCommercialName": "ACME",
    "EnableCaching": true,
    "CacheDurationMinutes": 60,
    "SriConnection": {
      "BaseUrl": "https://celcer.sri.gob.ec/",
      "TimeoutSeconds": 30,
      "MaxRetryAttempts": 3,
      "ValidateSslCertificate": true
    }
  }
}
```

### 3. Usage Examples

#### Validate Ecuadorian ID Card
```csharp
public class IdentityValidator
{
    private readonly ICedulaService _cedulaService;
    public IdentityValidator(ICedulaService cedulaService) => _cedulaService = cedulaService;
    public bool ValidateIdentity(string cedula) => _cedulaService.ValidateCedula(cedula);
}
```

#### Generate Electronic Invoice XML
```csharp
public class InvoiceGenerator
{
    private readonly IXmlService _xmlService;
    public InvoiceGenerator(IXmlService xmlService) => _xmlService = xmlService;
    public string GenerateInvoice(ComprobanteElectronico comprobante) => _xmlService.GenerateInvoiceXml(comprobante);
}
```

#### Send Document to SRI
```csharp
public class DocumentSender
{
    private readonly ISriWebService _sriService;
    public DocumentSender(ISriWebService sriService) => _sriService = sriService;
    public async Task<ResponseSri> SendDocumentAsync(string xmlContent, string username, string password)
    {
        var token = await _sriService.AuthenticateAsync(username, password);
        return await _sriService.SendDocumentAsync(xmlContent, token);
    }
}
```

#### Convert XML to HTML for Display
```csharp
public class DocumentRenderer
{
    private readonly IDocumentConverter _converter;
    public DocumentRenderer(IDocumentConverter converter) => _converter = converter;
    public string RenderDocument(string xmlContent) => _converter.ConvertToHtml(xmlContent);
}
```

## 📚 API Documentation

- `ICedulaService`, `IRucService` - Ecuadorian ID and RUC validation
- `IWebServiceSri` - SRI web service integration
- `IDocumentConverter` - XML to HTML/PDF conversion
- `IXmlDocumentParser`, `IAtsXmlService` - XML parsing services
- `ComprobanteElectronico` - Electronic document model
- `ResponseSri` - SRI response model

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

**[Acontplus](https://www.acontplus.com)** - Software solutions

---

**Built with ❤️ for the .NET community**
