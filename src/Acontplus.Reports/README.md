# Acontplus.Reports

[![NuGet](https://img.shields.io/nuget/v/Acontplus.Reports.svg)](https://www.nuget.org/packages/Acontplus.Reports)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A high-performance .NET 9+ library for RDLC report generation with advanced features for enterprise applications. Optimized for high concurrency, large reports, and production workloads.

## 🚀 Features

### Core Capabilities
- ✅ **Async/Await Support** - Fully asynchronous API for better scalability
- ✅ **RDLC Report Generation** - Support for PDF, Excel, Word, HTML5 exports
- ✅ **High Concurrency** - Built-in concurrency limiting and thread-safe operations
- ✅ **Memory Optimization** - Stream pooling and efficient memory management for large reports
- ✅ **Smart Caching** - Configurable report definition caching with TTL and size limits
- ✅ **Comprehensive Logging** - Structured logging with performance metrics
- ✅ **Error Handling** - Custom exceptions with detailed error context
- ✅ **Dependency Injection** - Full DI support with extension methods
- ✅ **Timeout Protection** - Configurable timeouts to prevent runaway report generation
- ✅ **Size Limits** - Configurable maximum report sizes

### Performance Optimizations
- Report definition caching reduces file I/O
- Concurrency limiting prevents resource exhaustion
- Async operations improve scalability under load
- Memory pooling reduces GC pressure
- Cancellation token support for graceful shutdowns

## 📦 Installation

### NuGet Package Manager
```bash
Install-Package Acontplus.Reports
```

### .NET CLI
```bash
dotnet add package Acontplus.Reports
```

### PackageReference
```xml
<ItemGroup>
  <PackageReference Include="Acontplus.Reports" Version="1.3.15" />
</ItemGroup>
```

## 🎯 Quick Start

### 1. Register Services

```csharp
using Acontplus.Reports.Extensions;

// In Program.cs or Startup.cs
builder.Services.AddReportServices(builder.Configuration);

// Or with custom configuration
builder.Services.AddReportServices(options =>
{
    options.MainDirectory = "Reports";
    options.MaxConcurrentReports = 20;
    options.MaxReportSizeBytes = 50 * 1024 * 1024; // 50 MB
    options.ReportGenerationTimeoutSeconds = 180; // 3 minutes
    options.EnableReportDefinitionCache = true;
    options.MaxCachedReportDefinitions = 100;
    options.CacheTtlMinutes = 60;
    options.EnableDetailedLogging = true;
});
```

### 2. Configure appsettings.json

```json
{
  "Reports": {
    "MainDirectory": "Reports",
    "ExternalDirectory": "C:\\ExternalReports",
    "MaxReportSizeBytes": 104857600,
    "ReportGenerationTimeoutSeconds": 300,
    "EnableReportDefinitionCache": true,
    "MaxCachedReportDefinitions": 100,
    "CacheTtlMinutes": 60,
    "EnableMemoryPooling": true,
    "MaxConcurrentReports": 10,
    "EnableDetailedLogging": false
  }
}
```

### 3. Generate Reports (Async - Recommended)

```csharp
using Acontplus.Reports.Interfaces;

public class ReportController : ControllerBase
{
    private readonly IRdlcReportService _reportService;
    private readonly ILogger<ReportController> _logger;

    public ReportController(IRdlcReportService reportService, ILogger<ReportController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateReportAsync(
        [FromBody] ReportRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Prepare parameters DataSet
            var parameters = new DataSet();
            var reportProps = new DataTable("ReportProps");
            reportProps.Columns.Add("ReportPath", typeof(string));
            reportProps.Columns.Add("ReportName", typeof(string));
            reportProps.Columns.Add("ReportFormat", typeof(string));
            reportProps.Rows.Add("Reports/InvoiceReport.rdlc", "Invoice", "PDF");
            parameters.Tables.Add(reportProps);

            // Prepare data DataSet
            var data = new DataSet();
            var invoiceData = new DataTable("Invoice");
            // ... populate with your data
            data.Tables.Add(invoiceData);

            var report = await _reportService.GetReportAsync(
                parameters,
                data,
                externalDirectory: false,
                cancellationToken: cancellationToken);

            return File(report.FileContents, report.ContentType, report.FileDownloadName);
        }
        catch (ReportTimeoutException ex)
        {
            _logger.LogWarning(ex, "Report generation timed out");
            return StatusCode(504, "Report generation timed out");
        }
        catch (ReportSizeExceededException ex)
        {
            _logger.LogWarning(ex, "Report size exceeded limit");
            return StatusCode(413, "Report too large");
        }
        catch (ReportNotFoundException ex)
        {
            _logger.LogError(ex, "Report template not found");
            return NotFound("Report template not found");
        }
        catch (ReportGenerationException ex)
        {
            _logger.LogError(ex, "Report generation failed");
            return StatusCode(500, "Report generation failed");
        }
    }
}
```

### 4. Add Report Files

Ensure your RDLC files are included in your project:

```xml
<ItemGroup>
    <None Update="Reports\**\*.rdlc">
        <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
</ItemGroup>
```

## 📚 Advanced Configuration

### Configuration Options

| Option | Default | Description |
|--------|---------|-------------|
| `MainDirectory` | `"Reports"` | Main directory for RDLC files |
| `ExternalDirectory` | `null` | External directory for offline reports |
| `MaxReportSizeBytes` | `104857600` (100MB) | Maximum output size |
| `ReportGenerationTimeoutSeconds` | `300` (5min) | Generation timeout |
| `EnableReportDefinitionCache` | `true` | Enable template caching |
| `MaxCachedReportDefinitions` | `100` | Max cached templates |
| `CacheTtlMinutes` | `60` | Cache expiration time |
| `EnableMemoryPooling` | `true` | Enable memory pooling |
| `MaxConcurrentReports` | `10` | Max concurrent generations |
| `EnableDetailedLogging` | `false` | Detailed performance logging |

### Exception Handling

The library provides specific exception types:

- `ReportGenerationException` - Base exception for report errors
- `ReportNotFoundException` - Report template not found
- `ReportTimeoutException` - Generation exceeded timeout
- `ReportSizeExceededException` - Output exceeded size limit

### Performance Recommendations

#### For High Concurrency (Many simultaneous users):
```csharp
options.MaxConcurrentReports = 50;
options.EnableReportDefinitionCache = true;
options.ReportGenerationTimeoutSeconds = 120;
```

#### For Large Reports (Complex reports with lots of data):
```csharp
options.MaxReportSizeBytes = 500 * 1024 * 1024; // 500 MB
options.ReportGenerationTimeoutSeconds = 600; // 10 minutes
options.EnableMemoryPooling = true;
```

#### For Memory-Constrained Environments:
```csharp
options.MaxConcurrentReports = 5;
options.MaxCachedReportDefinitions = 20;
options.CacheTtlMinutes = 30;
```

## � API Documentation

### IRdlcReportService

```csharp
/// <summary>
/// Generates a report asynchronously (recommended)
/// </summary>
Task<ReportResponse> GetReportAsync(
    DataSet parameters,
    DataSet data,
    bool externalDirectory = false,
    CancellationToken cancellationToken = default);

/// <summary>
/// Legacy synchronous method
/// </summary>
[Obsolete("Use GetReportAsync for better performance")]
ReportResponse GetReport(
    DataSet parameters,
    DataSet data,
    bool externalDirectory = false);

/// <summary>
/// Gets a default error report
/// </summary>
Task<ReportResponse> GetErrorAsync();
```

### Report Response Model

```csharp
public class ReportResponse : IDisposable
{
    public byte[] FileContents { get; set; }
    public string ContentType { get; set; }
    public string FileDownloadName { get; set; }
}
```

### Supported Export Formats

- **PDF** - `application/pdf`
- **Excel** - `application/vnd.ms-excel`
- **Excel OpenXML** - `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`
- **Word OpenXML** - `application/vnd.openxmlformats-officedocument.wordprocessingml.document`
- **HTML5** - `text/html`
- **Image** - `image/jpeg`

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

**Built with ❤️ for the .NET community**
