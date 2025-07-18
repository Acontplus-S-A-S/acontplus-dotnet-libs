# Acontplus.Utilities Usage Guide

Acontplus.Utilities provides cross-cutting utilities and helpers for .NET applications.

## 📦 Installation

```bash
Install-Package Acontplus.Utilities
```

## 🚀 Features
- Encryption and decryption utilities
- Barcode generation
- Custom logging
- Enum and text extensions
- API response helpers
- Pagination and metadata utilities

## 🛠️ Basic Usage

### API Response Extensions (Minimal API)
```csharp
var response = new ApiResponse<string>("Hello World");
return response.ToMinimalApiResult();
```

### Encryption Example
```csharp
var encrypted = EncryptionHelper.Encrypt("secret");
```

## 📖 See Also
- [API Reference](../Home.md)
- [Project README](https://github.com/Acontplus-S-A-S/acontplus-dotnet-libs/blob/main/src/Acontplus.Utilities/README.md) 