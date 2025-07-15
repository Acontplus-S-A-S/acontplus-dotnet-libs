namespace Acontplus.S3Application.Models;

/// <summary>
/// Represents the result of an S3 storage operation, including status, message, and optional file content.
/// </summary>
public class S3Response
{
    /// <summary>
    /// Gets or sets the HTTP-like status code for the operation (e.g., 200, 201, 404, 500).
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Gets or sets a message describing the result or error.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the file content as a byte array (for downloads).
    /// </summary>
    public byte[]? Content { get; set; }

    /// <summary>
    /// Gets or sets the MIME type of the file (for downloads).
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// Gets or sets the file name or presigned URL (for presigned requests).
    /// </summary>
    public string? FileName { get; set; }
}
