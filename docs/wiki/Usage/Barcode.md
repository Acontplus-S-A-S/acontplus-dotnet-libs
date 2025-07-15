# Acontplus.Barcode Usage Guide

Acontplus.Barcode provides barcode and QR code generation utilities for .NET applications.

## 📦 Installation

```bash
Install-Package Acontplus.Barcode
```

## 🚀 Features
- Barcode and QR code generation
- SkiaSharp and ZXing.Net integration
- Customizable output (format, size, label)

## 🛠️ Basic Usage

### Generate a Barcode
```csharp
var config = new BarcodeConfig { Text = "1234567890" };
var imageBytes = BarcodeGen.Create(config);
```

## 📖 See Also
- [API Reference](../Home.md)
- [Project README](../../src/Acontplus.Barcode/README.md) 