# Acontplus.S3Application

[![NuGet](https://img.shields.io/nuget/v/Acontplus.S3Application.svg)](https://www.nuget.org/packages/Acontplus.S3Application)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

A .NET 9+ library providing a simple, robust, and strongly-typed interface for AWS S3 storage operations. Includes async CRUD, presigned URLs, and business-grade error handling.

---

## 📑 Table of Contents
- [Features](#features)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Advanced Usage](#advanced-usage)
- [Configuration](#configuration)
- [Dependencies](#dependencies)
- [Error Handling](#error-handling)
- [Contributing](#contributing)
- [License](#license)
- [Author](#author)
- [Company](#company)

---

## 🚀 Features
- **Async S3 CRUD**: Upload, update, delete, and retrieve S3 objects asynchronously
- **Presigned URLs**: Generate secure, time-limited download links
- **Strong Typing**: Models for S3 objects, credentials, and responses
- **Business Error Handling**: Consistent, metadata-rich responses
- **NET 9+**: Nullable, required properties, and latest C# features
- **Easy Integration**: Designed for DI and configuration-first usage

---

## 📦 Installation

### NuGet Package Manager
```bash
Install-Package Acontplus.S3Application
```

### .NET CLI
```bash
dotnet add package Acontplus.S3Application
```

### PackageReference
```xml
<PackageReference Include="Acontplus.S3Application" Version="1.0.4" />
```

---

## 🎯 Quick Start

### 1. Register the Service
```csharp
services.AddScoped<IS3StorageService, S3StorageService>();
```

### 2. Configure appsettings.json
```json
{
  "S3Bucket": {
    "Name": "your-bucket-name",
    "Region": "your-region"
  },
  "AwsConfiguration": {
    "AWSAccessKey": "your-access-key",
    "AWSSecretKey": "your-secret-key"
  }
}
```

### 3. Basic Usage Example
```csharp
public class MyS3Consumer
{
    private readonly IS3StorageService _s3Service;
    public MyS3Consumer(IS3StorageService s3Service) => _s3Service = s3Service;

    public async Task UploadFileAsync(IFormFile file)
    {
        var config = ... // get IConfiguration from DI
        var s3Object = new S3ObjectCustom(config);
        await s3Object.Initialize("uploads/", file);
        var response = await _s3Service.UploadAsync(s3Object);
        if (response.StatusCode == 201)
        {
            // Success
        }
    }
}
```

---

## 🛠️ Advanced Usage

### Downloading an Object
```csharp
var s3Object = new S3ObjectCustom(config);
s3Object.Initialize("uploads/myfile.pdf");
var response = await _s3Service.GetObjectAsync(s3Object);
if (response.StatusCode == 200)
{
    var fileBytes = response.Content;
    var contentType = response.ContentType;
}
```

### Generating a Presigned URL
```csharp
var s3Object = new S3ObjectCustom(config);
s3Object.Initialize("uploads/myfile.pdf");
var urlResponse = await _s3Service.GetPresignedUrlAsync(s3Object, expirationInMinutes: 30);
var presignedUrl = urlResponse.FileName; // URL string
```

### Checking if an Object Exists
```csharp
var exists = await _s3Service.DoesObjectExistAsync(s3Object);
```

---

## ⚙️ Configuration
- **S3Bucket:Name**: Your S3 bucket name
- **S3Bucket:Region**: AWS region (e.g., us-east-1)
- **AwsConfiguration:AWSAccessKey**: AWS access key
- **AwsConfiguration:AWSSecretKey**: AWS secret key

All configuration is injected via `IConfiguration` for seamless integration with ASP.NET Core and other .NET apps.

---

## 📚 Dependencies
- .NET 9+
- [AWSSDK.Core](https://www.nuget.org/packages/AWSSDK.Core)
- [AWSSDK.S3](https://www.nuget.org/packages/AWSSDK.S3)
- [Microsoft.Extensions.Configuration.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Abstractions)

---

## 🛡️ Error Handling
All service methods return an `S3Response` object:
- `StatusCode`: HTTP-like status code (e.g., 200, 201, 404, 500)
- `Message`: Success or error message
- `Content`: File bytes (for downloads)
- `ContentType`: MIME type (for downloads)
- `FileName`: File name or presigned URL (for presigned requests)

---

## 🤝 Contributing
Contributions are welcome! Please open an issue or submit a pull request.

---

## 📄 License
MIT License. See [LICENSE](../LICENSE) for details.

---

## 👤 Author
[Ivan Paz](https://linktr.ee/iferpaz7)

---

## 🏢 Company
[Acontplus](https://www.acontplus.com)
