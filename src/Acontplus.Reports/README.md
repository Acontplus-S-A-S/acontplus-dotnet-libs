# Acontplus.Reports

[![NuGet](https://img.shields.io/nuget/v/Acontplus.Reports.svg)](https://www.nuget.org/packages/Acontplus.Reports)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A modern .NET 9+ library for RDLC (Report Definition Language Client-side) report generation, export, and management. Includes PDF/Excel export, template support, and ReportViewer integration.

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
        var report = _reportService.GenerateReport("ReportPath", reportData);
        return File(report, "application/pdf");
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

### 3. Export Reports
Export the generated reports to different formats such as PDF, Excel, etc.
```csharp
var pdfReport = _reportService.ExportReportToPdf("ReportPath", reportData);
var excelReport = _reportService.ExportReportToExcel("ReportPath", reportData);
```

## 📚 API Documentation

- `IRdlcReportService` - Main report service interface
- `RdlcPrinterService` - Print/export helpers
- `RdlcPrintRequest` - Report request model
- `FileFormats` - Supported export formats

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