# Acontplus.FactElect

[![NuGet](https://img.shields.io/nuget/v/Acontplus.FactElect.svg)](https://www.nuget.org/packages/Acontplus.FactElect/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A comprehensive .NET library for electronic invoicing and digital document handling in Ecuador, providing models,
services, and utilities to facilitate integration with SRI (Servicio de Rentas Internas) systems.

## Overview

Acontplus.Common.FactElect is a specialized .NET package that provides essential tools for electronic invoicing and
digital document processing according to Ecuadorian tax regulations. The library handles captcha validation, XML
document generation, SRI integration, and more.

## Features

- Electronic document models (invoices, credit notes, etc.)
- XML document generation, validation, and parsing
- SRI (Ecuadorian Tax Authority) web service integration
- Document validation and verification
- CAPTCHA handling for human verification processes
- ID card (cédula) and RUC validation
- XML to HTML/PDF document conversion
- Token-based authentication management
- Dependency Injection support

## Installation

Install the package via NuGet Package Manager:

```bash
Install-Package Acontplus.FactElect
```

Or via .NET CLI:

```bash
dotnet add package Acontplus.FactElect
```

## Quick Start

### Configure Services

Add FactElect services to your dependency injection container:

```csharp
// In Startup.cs or Program.cs
services.AddFactElectServices(Configuration);
```

### Configuration in appsettings.json

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

### Usage Examples

#### Validating an Ecuadorian ID Card

```csharp
// Using dependency injection
public class IdentityValidator
{
    private readonly ICedulaService _cedulaService;

    public IdentityValidator(ICedulaService cedulaService)
    {
        _cedulaService = cedulaService;
    }

    public bool ValidateIdentity(string cedula)
    {
        return _cedulaService.ValidateCedula(cedula);
    }
}
```

#### Generating an Electronic Invoice XML

```csharp
// Using dependency injection
public class InvoiceGenerator
{
    private readonly IXmlService _xmlService;

    public InvoiceGenerator(IXmlService xmlService)
    {
        _xmlService = xmlService;
    }

    public string GenerateInvoice(ComprobanteElectronico comprobante)
    {
        return _xmlService.GenerateInvoiceXml(comprobante);
    }
}
```

#### Sending a Document to SRI

```csharp
// Using dependency injection
public class DocumentSender
{
    private readonly ISriWebService _sriService;

    public DocumentSender(ISriWebService sriService)
    {
        _sriService = sriService;
    }

    public async Task<ResponseSri> SendDocumentAsync(
        string xmlContent, 
        string username, 
        string password)
    {
        // Authenticate with SRI
        var token = await _sriService.AuthenticateAsync(username, password);
        
        // Send document to SRI
        return await _sriService.SendDocumentAsync(xmlContent, token);
    }
}
```

#### Converting XML to HTML for Display

```csharp
// Using dependency injection
public class DocumentRenderer
{
    private readonly IDocumentConverter _converter;

    public DocumentRenderer(IDocumentConverter converter)
    {
        _converter = converter;
    }

    public string RenderDocument(string xmlContent)
    {
        return _converter.ConvertToHtml(xmlContent);
    }
}
```

## Project Structure

```
Acontplus.FactElect/
│
├── Configuration/        # Configuration settings
├── Constants/            # Constants and enumerations
├── Exceptions/           # Custom exceptions
├── Extensions/           # Extension methods
├── Interfaces/           # Service interfaces
├── Models/               # Domain models
│   ├── Authentication/
│   ├── Documents/
│   ├── Responses/
│   └── Validation/
├── Resources/            # Resource files
│   ├── Images/
│   └── Schemas/
├── Services/             # Service implementations
│   ├── Authentication/
│   ├── Conversion/
│   ├── Documents/
│   ├── External/
│   └── Validation/
└── Utilities/            # Helper classes
```

## Requirements

- .NET Standard 2.0+ or .NET 8.0+
- Internet connection for SRI web service integration

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Author

[Ivan Paz](https://linktr.ee/iferpaz7)

## Company

[Acontplus S.A.S.](https://acontplus.com.ec)