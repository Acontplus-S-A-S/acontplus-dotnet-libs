# Acontplus.Reports

[![NuGet](https://img.shields.io/nuget/v/Acontplus.Reports.svg)](https://www.nuget.org/packages/Acontplus.Reports)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A .NET 9+ library for RDLC (Report Definition Language Client-side) report generation, export, and management. Includes PDF/Excel export, template support, and ReportViewer integration.

## 🚀 Features

- RDLC report generation and management
- Export to PDF, Excel, and other formats
- Customizable report templates
- Integrated with .NET 9+ and ReportViewer
- Dependency Injection support

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
  <PackageReference Include="Acontplus.Reports" Version="1.0.15" />
</ItemGroup>
```

## 🎯 Quick Start

### 1. Configure Report Service
```csharp
using Acontplus.Reports.Services;
public class ReportController : Controller
{
    private readonly IRdlcReportService _reportService;
    public ReportController(IRdlcReportService reportService) => _reportService = reportService;
    public IActionResult GenerateReport()
    {
        // Prepare parameters and data DataSets
        var parameters = new DataSet();
        var data = new DataSet();
        // ... populate DataSets ...
        
        var report = _reportService.GetReport(parameters, data);
        return File(report.Content, report.ContentType, report.FileName);
    }
}
```

### 2. Add Report Files
Ensure your RDLC files are included in your project and set to be copied to the output directory.
```xml
<ItemGroup>
    <None Update="Reports\MyReport.rdlc">
        <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
</ItemGroup>
```

### 2. Export Reports
The report generation automatically handles different formats based on the RDLC report configuration. The response contains the file contents, content type, and download name.

```csharp
var reportResponse = _reportService.GetReport(parameters, data);
if (reportResponse != null)
{
    return File(reportResponse.FileContents, reportResponse.ContentType, reportResponse.FileDownloadName);
}
```

## 📚 API Documentation

- `IRdlcReportService` - Main report service interface with GetReport method
- `RdlcPrinterService` - Print/export helpers
- `ReportResponse` - Report response model with file contents and metadata
- `FileFormats` - Supported export formats

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
