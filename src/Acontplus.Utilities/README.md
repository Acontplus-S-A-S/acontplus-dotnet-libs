# Acontplus.Utilities

[![NuGet](https://img.shields.io/nuget/v/Acontplus.Utilities.svg)](https://www.nuget.org/packages/Acontplus.Utilities)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

A comprehensive .NET 9+ utility library providing common functionality for enterprise applications. Modern async, extension methods, minimal API support, and more.

## 🚀 Features

- **Encryption** - Data encryption/decryption utilities
- **External Validations** - Third-party validation integrations
- **Barcode Generation** - Barcode creation and processing
- **Custom Logging** - Enhanced logging capabilities
- **Enum Extensions** - Enhanced enum functionality
- **Picture Helper** - Image processing utilities
- **Text Handlers** - Text manipulation and processing
- **API Response Extensions** - Convert results to `IActionResult` or `IResult` for MVC/Minimal APIs
- **Pagination & Metadata** - Helpers for API metadata, pagination, and diagnostics

## 📦 Installation

### NuGet Package Manager
```bash
Install-Package Acontplus.Utilities
```

### .NET CLI
```bash
dotnet add package Acontplus.Utilities
```

### PackageReference
```xml
<ItemGroup>
  <PackageReference Include="Acontplus.Utilities" Version="1.0.12" />
</ItemGroup>
```

## 🎯 Quick Start

### 1. API Response Extensions (Minimal API)
```csharp
using Acontplus.Utilities.Extensions;

app.MapGet("/example", () =>
{
    var response = new ApiResponse<string>("Hello World");
    return response.ToMinimalApiResult();
});
```

### 2. API Response Extensions (Controller)
```csharp
using Acontplus.Utilities.Extensions;

public IActionResult Get()
{
    var response = new ApiResponse<string>("Hello World");
    return response.ToActionResult();
}
```

### 3. Encryption Example
```csharp
var encryptionService = new SensitiveDataEncryptionService();
byte[] encrypted = await encryptionService.EncryptToBytesAsync("password", "data");
string decrypted = await encryptionService.DecryptFromBytesAsync("password", encrypted);
```

### 4. Pagination Metadata Example
```csharp
var metadata = new Dictionary<string, object>()
    .WithPagination(page: 1, pageSize: 10, totalItems: 100);
```

## 🔧 Advanced Usage

### File Name Sanitization
```csharp
string safeName = FileExtensions.SanitizeFileName("my*illegal:file?.txt");
```

### Base64 Conversion
```csharp
string base64 = FileExtensions.GetBase64FromByte(myBytes);
```

### Compression Utilities
```csharp
byte[] compressed = CompressionUtils.CompressGZip(data);
byte[] decompressed = CompressionUtils.DecompressGZip(compressed);
```

## 📚 API Documentation

- `SensitiveDataEncryptionService` - AES encryption/decryption helpers
- `FileExtensions` - File name and byte array utilities
- `CompressionUtils` - GZip/Deflate compression helpers
- `TextHandlers` - String formatting and splitting
- `ApiResponseExtensions` - API response helpers for MVC/Minimal APIs
- `DirectoryHelper` / `EnvironmentHelper` - Runtime and environment utilities

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