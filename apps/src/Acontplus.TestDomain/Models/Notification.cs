namespace Acontplus.TestDomain.Models;

public class Notification
{
    public bool hasFile { get; set; }
    public string isReport { get; set; }
    public Dictionary<string, object> reportParams { get; set; }
    public Dictionary<string, object> spParams { get; set; }
    public bool withTableNames { get; set; } = false;
}
