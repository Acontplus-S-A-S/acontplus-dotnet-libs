# Acontplus.Barcode

[![NuGet](https://img.shields.io/nuget/v/Acontplus.Barcode.svg)](https://www.nuget.org/packages/Acontplus.Barcode)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

Advanced barcode generation library with ZXing.Net integration. Supports QR codes, 1D/2D barcodes, custom styling, and high-performance image generation using SkiaSharp for cross-platform applications.

## 🚀 Features

### 📱 Barcode Formats
- **QR Codes** - High-capacity 2D barcodes with error correction
- **Code 128** - High-density linear barcode for alphanumeric data
- **Code 39** - Industrial barcode standard for alphanumeric data
- **EAN-13** - European Article Number for retail products
- **EAN-8** - Compact version of EAN for small products
- **UPC-A** - Universal Product Code for retail products
- **UPC-E** - Compact UPC for small products
- **PDF417** - High-capacity 2D barcode for large data
- **Data Matrix** - Compact 2D barcode for small items

### 🎨 Customization Options
- **Custom Colors** - Foreground and background color customization
- **Size Control** - Adjustable width, height, and margins
- **Error Correction** - Configurable error correction levels for QR codes
- **Format Options** - PNG, JPEG, and other image formats
- **Cross-Platform** - SkiaSharp rendering for consistent output

## 📦 Installation

### NuGet Package Manager
```bash
Install-Package Acontplus.Barcode
```

### .NET CLI
```bash
dotnet add package Acontplus.Barcode
```

### PackageReference
```xml
<PackageReference Include="Acontplus.Barcode" Version="1.0.2" />
```

## 🎯 Quick Start

### 1. Basic QR Code Generation

```csharp
using Acontplus.Barcode.Utils;
using Acontplus.Barcode.Models;

// Generate a simple QR code
var config = new BarcodeConfig
{
    Text = "https://example.com",
    Format = ZXing.BarcodeFormat.QR_CODE,
    Width = 300,
    Height = 300
};
var qrCode = BarcodeGen.Create(config);
await File.WriteAllBytesAsync("qr-code.png", qrCode);
```

### 2. Customized Barcode

```csharp
// Generate a Code 128 barcode with custom settings
var config = new BarcodeConfig
{
    Text = "123456789",
    Format = ZXing.BarcodeFormat.CODE_128,
    Width = 400,
    Height = 100,
    Margin = 10,
    IncludeLabel = false,
    OutputFormat = SkiaSharp.SKEncodedImageFormat.Png,
    Quality = 100
};

var barcode = BarcodeGen.Create(config);
await File.WriteAllBytesAsync("barcode.png", barcode);
```

### 3. Multiple Barcode Formats

```csharp
### 3. Multiple Barcode Formats

```csharp
// Generate different barcode types
var formats = new[]
{
    ZXing.BarcodeFormat.QR_CODE,
    ZXing.BarcodeFormat.CODE_128,
    ZXing.BarcodeFormat.CODE_39,
    ZXing.BarcodeFormat.EAN_13,
    ZXing.BarcodeFormat.PDF_417
};

foreach (var format in formats)
{
    var config = new BarcodeConfig
    {
        Text = "Sample Data",
        Format = format,
        Width = 300,
        Height = 100
    };
    
    var barcode = BarcodeGen.Create(config);
    await File.WriteAllBytesAsync($"{format}.png", barcode);
}
```
```

## 🔧 Advanced Usage

### QR Code with Custom Options

```csharp
var config = new BarcodeConfig
{
    Text = "Important data that needs error correction",
    Format = ZXing.BarcodeFormat.QR_CODE,
    Width = 400,
    Height = 400,
    Margin = 20,
    AdditionalOptions = new ZXing.Common.EncodingOptions
    {
        ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.H
    }
};

var qrCode = BarcodeGen.Create(config);
```

### Batch Generation

```csharp
var items = new[]
{
    "Item001",
    "Item002", 
    "Item003",
    "Item004"
};

for (int i = 0; i < items.Length; i++)
{
    var config = new BarcodeConfig
    {
        Text = items[i],
        Format = ZXing.BarcodeFormat.CODE_128,
        Width = 300,
        Height = 80
    };
    
    var barcode = BarcodeGen.Create(config);
    await File.WriteAllBytesAsync($"item_{i + 1}.png", barcode);
}
```

## 📊 Barcode Format Comparison

| Format | Type | Data Capacity | Use Case |
|--------|------|---------------|----------|
| QR Code | 2D | High | URLs, contact info, general data |
| Code 128 | 1D | Medium | Inventory, shipping, logistics |
| Code 39 | 1D | Medium | Industrial, automotive, defense |
| EAN-13 | 1D | Low | Retail products, point of sale |
| UPC-A | 1D | Low | Retail products, North America |
| PDF417 | 2D | High | Government IDs, shipping labels |
| Data Matrix | 2D | Medium | Small items, electronics |

## 🎨 Configuration Options

### BarcodeConfig Properties

```csharp
public class BarcodeConfig
{
    public string Text { get; set; } = string.Empty;
    public ZXing.BarcodeFormat Format { get; set; } = ZXing.BarcodeFormat.CODE_128;
    public int Width { get; set; } = 300;
    public int Height { get; set; } = 100;
    public bool IncludeLabel { get; set; } = false;
    public int Margin { get; set; } = 10;
    public SkiaSharp.SKEncodedImageFormat OutputFormat { get; set; } = SkiaSharp.SKEncodedImageFormat.Png;
    public int Quality { get; set; } = 100;
    public ZXing.Common.EncodingOptions? AdditionalOptions { get; set; }
}
```

### Error Correction Levels (QR Codes)

- **L (Low)** - 7% recovery capacity
- **M (Medium)** - 15% recovery capacity  
- **Q (Quartile)** - 25% recovery capacity
- **H (High)** - 30% recovery capacity

## 🔍 Best Practices

### 1. Choose the Right Format
```csharp
// For URLs and general data
var qrConfig = new BarcodeConfig { Format = BarcodeFormat.QR_CODE };

// For inventory systems
var code128Config = new BarcodeConfig { Format = BarcodeFormat.CODE_128 };

// For retail products
var eanConfig = new BarcodeConfig { Format = BarcodeFormat.EAN_13 };
```

### 2. Optimize Size and Quality
```csharp
// High-quality QR code for printing
var highQuality = new BarcodeConfig
{
    Text = "https://example.com",
    Format = BarcodeFormat.QR_CODE,
    Width = 600,
    Height = 600,
    ErrorCorrection = ErrorCorrectionLevel.H
};

// Compact barcode for web display
var compact = new BarcodeConfig
{
    Text = "123456789",
    Format = BarcodeFormat.CODE_128,
    Width = 200,
    Height = 60
};
```

### 3. Handle Large Data Sets
```csharp
### Handle Large Data Sets
```csharp
// For large amounts of data, use 2D formats
var largeData = new BarcodeConfig
{
    Text = "Large amount of data that needs to be encoded...",
    Format = ZXing.BarcodeFormat.PDF_417,
    Width = 400,
    Height = 200
};
```
```

## 🚀 Performance Tips

### 1. Reuse Configuration Objects
```csharp
var baseConfig = new BarcodeConfig
{
    Format = BarcodeFormat.CODE_128,
    Width = 300,
    Height = 100
};

// Reuse for multiple barcodes
foreach (var item in items)
{
    baseConfig.Text = item;
    var barcode = BarcodeGen.GenerateBarcode(baseConfig);
}
```

### 2. Batch Processing
```csharp
// Process multiple barcodes efficiently
var tasks = items.Select(async item =>
{
    var config = new BarcodeConfig
    {
        Text = item,
        Format = BarcodeFormat.CODE_128,
        Width = 300,
        Height = 100
    };
    
    return await Task.Run(() => BarcodeGen.GenerateBarcode(config));
});

var barcodes = await Task.WhenAll(tasks);
```

## 📚 API Reference

### BarcodeGen Class

```csharp
public static class BarcodeGen
{
    // Generate barcode with custom configuration
    public static byte[] Create(BarcodeConfig config);
}
```

### BarcodeConfig Class

```csharp
public class BarcodeConfig
{
    public string Text { get; set; }
    public BarcodeFormat Format { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int Margin { get; set; }
    public Color ForegroundColor { get; set; }
    public Color BackgroundColor { get; set; }
    public ErrorCorrectionLevel ErrorCorrection { get; set; }
}
```

## 🔧 Dependencies

- **.NET 10+** - Modern .NET framework
- **ZXing.Net** - Barcode generation library
- **ZXing.Net.Bindings.SkiaSharp** - SkiaSharp bindings for ZXing
- **SkiaSharp** - Cross-platform graphics library
- **SkiaSharp.NativeAssets.Linux** - Native assets for Linux
- **System.Drawing.Common** - Image processing support

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details.

### Development Setup
```bash
git clone https://github.com/acontplus/acontplus-dotnet-libs.git
cd acontplus-dotnet-libs
dotnet restore
dotnet build
```

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

- 📧 Email: proyectos@acontplus.com
- 🐛 Issues: [GitHub Issues](https://github.com/acontplus/acontplus-dotnet-libs/issues)
- 📖 Documentation: [Wiki](https://github.com/acontplus/acontplus-dotnet-libs/wiki)

## 👨‍💻 Author

**Ivan Paz** - [@iferpaz7](https://linktr.ee/iferpaz7)

## 🏢 Company

**[Acontplus](https://www.acontplus.com)** - Software solutions

---

**Built with ❤️ for the .NET community using cutting-edge .NET features**
