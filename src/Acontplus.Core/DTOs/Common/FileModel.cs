namespace Acontplus.Core.Dtos.Common;

public class FileModel
{
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public byte[]? Content { get; set; }
    public string? Base64 { get; set; }
}
