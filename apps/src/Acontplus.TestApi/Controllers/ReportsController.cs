using Acontplus.Reports.Interfaces;
using Acontplus.Reports.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Acontplus.TestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IRdlcReportService _reportService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(IRdlcReportService reportService, ILogger<ReportsController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    /// <summary>
    /// Generate a sample invoice report with hardcoded data
    /// </summary>
    [HttpGet("sample-invoice")]
    public async Task<IActionResult> GenerateSampleInvoice(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Generating sample invoice report");

            // Create parameters DataSet
            var parameters = new DataSet();

            // Add ReportProps table
            var reportPropsTable = new DataTable("ReportProps");
            reportPropsTable.Columns.Add("ReportPath", typeof(string));
            reportPropsTable.Columns.Add("ReportName", typeof(string));
            reportPropsTable.Columns.Add("ReportFormat", typeof(string));
            reportPropsTable.Rows.Add("SampleInvoice.rdlc", "Sample_Invoice", "PDF");
            parameters.Tables.Add(reportPropsTable);

            // Add ReportParams table for logo and other parameters
            var reportParamsTable = new DataTable("ReportParams");
            reportParamsTable.Columns.Add("paramName", typeof(string));
            reportParamsTable.Columns.Add("paramValue", typeof(byte[]));
            reportParamsTable.Columns.Add("isPicture", typeof(bool));
            reportParamsTable.Columns.Add("isCompressed", typeof(bool));

            // Add company name parameter
            reportParamsTable.Rows.Add("CompanyName", System.Text.Encoding.UTF8.GetBytes("Acontplus Demo Company"), false, false);
            reportParamsTable.Rows.Add("InvoiceTitle", System.Text.Encoding.UTF8.GetBytes("SAMPLE INVOICE"), false, false);
            parameters.Tables.Add(reportParamsTable);

            // Create data DataSet
            var data = new DataSet();

            // Add Invoice Header data
            var invoiceHeaderTable = new DataTable("InvoiceHeader");
            invoiceHeaderTable.Columns.Add("InvoiceNumber", typeof(string));
            invoiceHeaderTable.Columns.Add("InvoiceDate", typeof(string));
            invoiceHeaderTable.Columns.Add("CustomerName", typeof(string));
            invoiceHeaderTable.Columns.Add("CustomerAddress", typeof(string));
            invoiceHeaderTable.Columns.Add("CustomerTaxId", typeof(string));
            invoiceHeaderTable.Columns.Add("Subtotal", typeof(decimal));
            invoiceHeaderTable.Columns.Add("Tax", typeof(decimal));
            invoiceHeaderTable.Columns.Add("Total", typeof(decimal));

            invoiceHeaderTable.Rows.Add(
                "INV-2024-001",
                DateTime.Now.ToString("yyyy-MM-dd"),
                "ABC Corporation",
                "123 Business St, Suite 100, City, Country",
                "1234567890001",
                1000.00m,
                120.00m,
                1120.00m
            );
            data.Tables.Add(invoiceHeaderTable);

            // Add Invoice Items data
            var invoiceItemsTable = new DataTable("InvoiceItems");
            invoiceItemsTable.Columns.Add("ItemNumber", typeof(int));
            invoiceItemsTable.Columns.Add("Description", typeof(string));
            invoiceItemsTable.Columns.Add("Quantity", typeof(int));
            invoiceItemsTable.Columns.Add("UnitPrice", typeof(decimal));
            invoiceItemsTable.Columns.Add("Amount", typeof(decimal));

            invoiceItemsTable.Rows.Add(1, "Professional Services - Web Development", 20, 25.00m, 500.00m);
            invoiceItemsTable.Rows.Add(2, "Consulting Services - System Architecture", 10, 30.00m, 300.00m);
            invoiceItemsTable.Rows.Add(3, "Software License - Enterprise Edition", 1, 200.00m, 200.00m);

            data.Tables.Add(invoiceItemsTable);

            // Generate the report
            var report = await _reportService.GetReportAsync(parameters, data, false, cancellationToken);

            _logger.LogInformation("Sample invoice report generated successfully");

            return File(report.FileContents, report.ContentType, report.FileDownloadName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating sample invoice report");
            return StatusCode(500, new { error = "Failed to generate report", details = ex.Message });
        }
    }

    /// <summary>
    /// Generate a sample customer list report
    /// </summary>
    [HttpGet("sample-customers")]
    public async Task<IActionResult> GenerateSampleCustomerList(
        [FromQuery] string format = "PDF",
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating sample customer list report in {Format} format", format);

            // Create parameters DataSet
            var parameters = new DataSet();

            var reportPropsTable = new DataTable("ReportProps");
            reportPropsTable.Columns.Add("ReportPath", typeof(string));
            reportPropsTable.Columns.Add("ReportName", typeof(string));
            reportPropsTable.Columns.Add("ReportFormat", typeof(string));
            reportPropsTable.Rows.Add("CustomerList.rdlc", "Customer_List", format.ToUpper());
            parameters.Tables.Add(reportPropsTable);

            // Add report parameters
            var reportParamsTable = new DataTable("ReportParams");
            reportParamsTable.Columns.Add("paramName", typeof(string));
            reportParamsTable.Columns.Add("paramValue", typeof(byte[]));
            reportParamsTable.Columns.Add("isPicture", typeof(bool));
            reportParamsTable.Columns.Add("isCompressed", typeof(bool));

            reportParamsTable.Rows.Add("ReportDate", System.Text.Encoding.UTF8.GetBytes(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")), false, false);
            reportParamsTable.Rows.Add("ReportTitle", System.Text.Encoding.UTF8.GetBytes("Customer List Report"), false, false);
            parameters.Tables.Add(reportParamsTable);

            // Create data DataSet
            var data = new DataSet();

            // Add Customers data
            var customersTable = new DataTable("Customers");
            customersTable.Columns.Add("CustomerId", typeof(int));
            customersTable.Columns.Add("CustomerName", typeof(string));
            customersTable.Columns.Add("Email", typeof(string));
            customersTable.Columns.Add("Phone", typeof(string));
            customersTable.Columns.Add("City", typeof(string));
            customersTable.Columns.Add("TotalPurchases", typeof(decimal));
            customersTable.Columns.Add("Status", typeof(string));

            // Add sample data
            customersTable.Rows.Add(1, "ABC Corporation", "contact@abc.com", "+1-555-0101", "New York", 15000.50m, "Active");
            customersTable.Rows.Add(2, "XYZ Industries", "info@xyz.com", "+1-555-0102", "Los Angeles", 23500.75m, "Active");
            customersTable.Rows.Add(3, "Tech Solutions LLC", "hello@techsol.com", "+1-555-0103", "San Francisco", 8900.00m, "Active");
            customersTable.Rows.Add(4, "Global Trading Co", "sales@global.com", "+1-555-0104", "Chicago", 45000.25m, "Premium");
            customersTable.Rows.Add(5, "Smart Systems Inc", "contact@smart.com", "+1-555-0105", "Boston", 12300.00m, "Active");
            customersTable.Rows.Add(6, "Future Enterprises", "info@future.com", "+1-555-0106", "Seattle", 5600.80m, "Inactive");
            customersTable.Rows.Add(7, "Digital Dynamics", "hello@digital.com", "+1-555-0107", "Miami", 19800.50m, "Active");
            customersTable.Rows.Add(8, "Innovative Partners", "contact@innov.com", "+1-555-0108", "Denver", 31200.00m, "Premium");

            data.Tables.Add(customersTable);

            // Generate the report
            var report = await _reportService.GetReportAsync(parameters, data, false, cancellationToken);

            _logger.LogInformation("Customer list report generated successfully");

            return File(report.FileContents, report.ContentType, report.FileDownloadName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating customer list report");
            return StatusCode(500, new { error = "Failed to generate report", details = ex.Message });
        }
    }

    /// <summary>
    /// Test endpoint to verify report service configuration
    /// </summary>
    [HttpGet("test-configuration")]
    public IActionResult TestConfiguration()
    {
        return Ok(new
        {
            message = "Report service is configured and ready",
            timestamp = DateTime.UtcNow,
            supportedFormats = new[] { "PDF", "EXCEL", "EXCELOPENXML", "WORDOPENXML", "HTML5", "IMAGE" }
        });
    }
}
