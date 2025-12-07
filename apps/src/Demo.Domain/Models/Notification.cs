namespace Demo.Domain.Models;

public class Notification
{
    public bool HasFile { get; set; }
    public string IsReport { get; set; }
    public Dictionary<string, object> ReportParams { get; set; }
    public Dictionary<string, object> SpParams { get; set; }
    public bool WithTableNames { get; set; } = false;
}
