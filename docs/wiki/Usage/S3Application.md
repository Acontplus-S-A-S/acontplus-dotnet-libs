# Acontplus.S3Application Usage Guide

Acontplus.S3Application provides a simple and consistent interface for AWS S3 storage operations.

## 📦 Installation

```bash
Install-Package Acontplus.S3Application
```

## 🚀 Features
- Asynchronous S3 operations
- Strongly-typed S3 object handling
- Simple CRUD interface

## 🛠️ Basic Usage

### Register Service
```csharp
services.AddScoped<IS3StorageService, S3StorageService>();
```

### Upload a File
```csharp
var s3Object = new S3ObjectCustom { FileName = "file.txt", Content = fileBytes };
var response = await s3StorageService.UploadAsync(s3Object);
```

## 📖 See Also
- [API Reference](../Home.md)
- [Project README](../../src/Acontplus.S3Application/README.md) 