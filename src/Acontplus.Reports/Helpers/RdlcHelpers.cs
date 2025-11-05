namespace Acontplus.Reports.Helpers;

public static class RdlcHelpers
{
    public static MemoryStream LoadReportDefinition(string filePath)
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
