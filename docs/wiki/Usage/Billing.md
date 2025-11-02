# Acontplus.Billing Usage Guide

Acontplus.Billing is a comprehensive library for electronic invoicing and SRI integration in Ecuador.

## 📦 Installation

```bash
Install-Package Acontplus.Billing
```

## 🚀 Features
- Electronic document models (invoices, credit notes, etc.)
- XML generation, validation, and parsing
- SRI (Ecuadorian Tax Authority) web service integration
- CAPTCHA and ID validation
- XML to HTML/PDF conversion
- Token-based authentication

## 🛠️ Basic Usage

### Configure Services
```csharp
services.AddBillingServices(Configuration);
```

### Validate an Invoice XML
```csharp
var errors = XmlValidator.Validate(xmlString, xsdStream);
```

### SRI Integration
```csharp
var response = await sriWebService.SendAsync(document);
```

## 📖 See Also
- [API Reference](../Home.md)
- [Project README](https://github.com/Acontplus-S-A-S/acontplus-dotnet-libs/blob/main/src/Acontplus.Billing/README.md) 
