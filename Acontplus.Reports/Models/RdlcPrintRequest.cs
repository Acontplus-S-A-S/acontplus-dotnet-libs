namespace Acontplus.Reports.Models;

public class RdlcPrintRequest
{
    public Dictionary<string, List<Dictionary<string, string>>> DataSources { get; set; }
    public Dictionary<string, string> ReportParams { get; set; }
}
