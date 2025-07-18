# Acontplus.Reports Usage Guide

Acontplus.Reports enables RDLC report generation, export (PDF/Excel), and template management for .NET apps.

## 📦 Installation

```bash
Install-Package Acontplus.Reports
```

## 🚀 Features
- RDLC report generation and management
- Export to PDF, Excel, and other formats
- Customizable report templates
- Dependency Injection support

## 🛠️ Basic Usage

### Configure Report Service
```csharp
services.AddScoped<IRdlcReportService, RdlcReportService>();
```

### Generate and Export a Report
```csharp
var report = rdlcReportService.GenerateReport("ReportPath", reportData);
return File(report, "application/pdf");
```

## 📖 See Also
- [API Reference](../Home.md)
- [Project README](https://github.com/Acontplus-S-A-S/acontplus-dotnet-libs/blob/main/src/Acontplus.Reports/README.md) 