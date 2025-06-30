namespace Acontplus.Core.DTOs.Shared;

public class FileModel
{
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public required byte[] Content { get; set; }
    public string? Base64 { get; set; }
}
