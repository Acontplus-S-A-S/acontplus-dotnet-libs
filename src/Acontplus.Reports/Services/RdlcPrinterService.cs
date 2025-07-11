using Acontplus.Utilities.Data;
using System.Drawing.Imaging;
using System.Drawing.Printing;

namespace Acontplus.Reports.Services;

public interface IRdlcPrinterService
{
    public bool Print(RdlcPrinter rdlcPrinter, RdlcPrintRequest printRequest);
}

public class RdlcPrinterService : IRdlcPrinterService
{
    public bool Print(RdlcPrinter rdlcPrinter, RdlcPrintRequest printRequest)
    {
        var streams = new List<Stream>();
        using var lr = new LocalReport();
        lr.LoadReportDefinition(LoadReportDefinition(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            rdlcPrinter.ReportsDirectory, rdlcPrinter.FileName)));
        if (printRequest.DataSources != null)
        {
            foreach (var item in printRequest.DataSources)
            {
                lr.DataSources.Add(new ReportDataSource(item.Key,
                    DataConverters.JsonToDataTable(JsonExtensions.SerializeModern(item.Value))));
            }
        }

        var reportParams = lr.GetParameters();

        if (reportParams.Count > 0 && printRequest.ReportParams != null)
        {
            foreach (var item in printRequest.ReportParams)
            {
                printRequest.ReportParams.TryGetValue("mimeType", out _);
                if (item.Key == "logo")
                {
                    var logoPath = "";
                    if (Directory.Exists(rdlcPrinter.LogoDirectory))
                    {
                        var fileEntries = Directory.GetFiles(rdlcPrinter.LogoDirectory);
                        foreach (var entry in fileEntries)
                        {
                            var fileName = Path.GetFileNameWithoutExtension(entry);
                            if (fileName == rdlcPrinter.LogoName)
                            {
                                logoPath = Path.Combine(rdlcPrinter.LogoDirectory, entry);
                                break;
                            }
                        }
                    }

                    lr.SetParameters(new ReportParameter(item.Key,
                        Convert.ToBase64String(File.ReadAllBytes(logoPath))));
                }
                else
                {
                    lr.SetParameters(new ReportParameter(item.Key, item.Value));
                }
            }
        }

        lr.Render(rdlcPrinter.Format, rdlcPrinter.DeviceInfo, (_, _, _, _, _) =>
        {
            var stream = new MemoryStream();
            streams.Add(stream);
            return stream;
        }, out _);

        foreach (var stream in streams)
            stream.Position = 0;

        if (streams == null || streams.Count == 0)
        {
            throw new Exception("Error: no stream to print.");
        }

        var printDoc = new PrintDocument();
        printDoc.PrinterSettings.PrinterName = rdlcPrinter.PrinterName;
        var pageSettings = new PrinterSettings();
        pageSettings.PrinterName = rdlcPrinter.PrinterName;
        printDoc.DefaultPageSettings = pageSettings.DefaultPageSettings;

        var currentPage = 0;
        switch (printDoc.PrinterSettings.IsValid)
        {
            case true:
                printDoc.PrintPage += (sender, e) =>
                {
                    var pageImage = new Metafile(streams[currentPage]);
                    e.Graphics.DrawImage(
                        pageImage, e.PageBounds);
                    currentPage++;
                    e.HasMorePages = currentPage < streams.Count;
                };
                printDoc.EndPrint += (sender, e) =>
                {
                    if (streams != null)
                    {
                        foreach (var item in streams)
                        {
                            item.Close();
                        }

                        streams.Clear();
                    }
                    if (printDoc.PrintController.IsPreview)
                    {
                    }
                };
                printDoc.PrinterSettings.Copies = rdlcPrinter.Copies;
                printDoc.Print();
                return true;
            default:
                return false;
        }
    }

    private static MemoryStream LoadReportDefinition(string filePath)
    {
        var memoryStream = new MemoryStream();
        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            fileStream.CopyTo(memoryStream);
        }

        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }
}
