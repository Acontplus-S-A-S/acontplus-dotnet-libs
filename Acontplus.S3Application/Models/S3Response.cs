namespace Acontplus.S3Application.Models;

public class S3Response
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public byte[]? Content { get; set; }
    public string? ContentType { get; set; }
    public string? FileName { get; set; }
}
