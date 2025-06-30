# Common.S3Application

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A lightweight .NET library providing a simple and consistent interface for AWS S3 storage operations. This package implements basic CRUD operations for S3 objects with async support.

## Features

- Asynchronous S3 operations
- Strongly-typed S3 object handling
- Custom response wrapping
- Simple CRUD interface
- Task-based operation support

## Installation

```bash
dotnet add package Common.S3Application
```

## Quick Start

### 1. Interface Implementation
```csharp
public interface IS3StorageService
{
    Task<S3Response> UploadAsync(S3ObjectCustom s3ObjectCustom);
    Task<S3Response> UpdateAsync(S3ObjectCustom s3ObjectCustom);
    Task<S3Response> DeleteAsync(S3ObjectCustom s3ObjectCustom);
}
```

### 2. Basic Usage
```csharp
public class MyS3Service
{
    private readonly IS3StorageService _s3StorageService;

    public MyS3Service(IS3StorageService s3StorageService)
    {
        _s3StorageService = s3StorageService;
    }

    public async Task UploadFileAsync(string fileName, byte[] content)
    {
        var s3Object = new S3ObjectCustom
        {
            FileName = fileName,
            Content = content
        };

        var response = await _s3StorageService.UploadAsync(s3Object);
    }
}
```

## Dependencies

- .NET Standard 2.0+
- AWS SDK for .NET
- Common.Core (for S3Response and S3ObjectCustom types)

## Configuration

Add the following to your appsettings.json:

```json
{
  "AWS": {
    "Region": "your-region",
    "BucketName": "your-bucket-name"
  }
}
```

## Service Registration

```csharp
services.AddScoped<IS3StorageService, S3StorageService>();
```

## Error Handling

The service returns `S3Response` objects which include:
- Success/failure status
- Error messages if applicable
- Operation metadata

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

MIT License

Copyright (c) 2024 [Acontplus S.A.S.]

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

## Author

[Ivan Paz](https://linktr.ee/iferpaz7)

## Company

[Acontplus S.A.S.](https://acontplus.com.ec)