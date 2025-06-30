# Acontplus.Reports

**Acontplus.Reports** is a .NET library designed to simplify the generation of RDLC (Report Definition Language Client-side) reports. It includes features for creating, exporting, and managing reports using RDLC in your .NET applications.

## Features

- **RDLC Report Generation**: Easily create and manage RDLC reports.
- **Flexible Report Export**: Export reports to PDF, Excel, or other formats supported by RDLC.
- **Customizable Report Templates**: Use your own report templates or modify existing ones.
- **Integrated with .NET Core/8.0**: Fully compatible with .NET Core and .NET 8.0 applications.

## Installation

You can install the package via [NuGet](https://www.nuget.org/packages/Acontplus.Reports/).

### .NET CLI

```
dotnet add package Acontplus.Reports
```

### Package Manager Console
```
Install-Package Acontplus.Reports
```
## Usage
### 1. Configure Report Service
Inject the RdlcReportService into your .NET Core application to generate reports.

```
using Acontplus.Reports.Services;

public class ReportController : Controller
{
    private readonly IRdlcReportService _reportService;

    public ReportController(IRdlcReportService reportService)
    {
        _reportService = reportService;
    }

    public IActionResult GenerateReport()
    {
        var report = _reportService.GenerateReport("ReportPath", reportData);
        return File(report, "application/pdf");
    }
}
```
### 2. Add Report Files
Make sure that your RDLC files are included in your project and set to be copied to the output directory.
````
<ItemGroup>
    <None Update="Reports\MyReport.rdlc">
        <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
</ItemGroup>
````
### 3. Export Reports
You can export the generated reports to different formats such as PDF, Excel, etc.

#### Export to PDF
````
var pdfReport = _reportService.ExportReportToPdf("ReportPath", reportData);
````
#### Export to Excel
````
var excelReport = _reportService.ExportReportToExcel("ReportPath", reportData);
````
## Dependencies
- **.NET 8.0 or later:** Make sure you're using .NET 8.0 or higher.
- **RDLC ReportViewer:** The RDLC ReportViewer is required to generate and display reports.
## Contributing
We welcome contributions! Please submit any issues or feature requests via our GitHub repository, or feel free to fork the project and submit pull requests.

## License
This project is licensed under the MIT License - see the LICENSE file for details.

## Author

[Ivan Paz](https://linktr.ee/iferpaz7)

## Company

[Acontplus S.A.S.](https://acontplus.com.ec)