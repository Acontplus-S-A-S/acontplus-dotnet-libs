using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Acontplus.S3Application.Models;

/// <summary>
/// Represents a strongly-typed S3 object, including content, metadata, and AWS configuration.
/// </summary>
public sealed class S3ObjectCustom : IDisposable
{
    private readonly IConfiguration _configuration;
    private bool _disposed;

    /// <summary>
    /// Gets the AWS region for the S3 bucket.
    /// </summary>
    public string Region { get; private set; }

    /// <summary>
    /// Gets the S3 bucket name.
    /// </summary>
    public string BucketName { get; private set; }

    /// <summary>
    /// Gets the file content as a byte array.
    /// </summary>
    public byte[]? Content { get; private set; }

    /// <summary>
    /// Gets the S3 object key (path/filename in the bucket).
    /// </summary>
    public string S3ObjectKey { get; private set; }

    /// <summary>
    /// Gets the full S3 object URL.
    /// </summary>
    public string S3ObjectUrl { get; private set; }

    /// <summary>
    /// Gets the AWS credentials for this object.
    /// </summary>
    public AwsCredentials? AwsCredentials { get; private set; }

    /// <summary>
    /// Gets the MIME type of the file.
    /// </summary>
    public string ContentType { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="S3ObjectCustom"/> class using configuration.
    /// </summary>
    /// <param name="configuration">The application configuration containing S3 and AWS settings.</param>
    public S3ObjectCustom(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        BucketName = configuration["S3Bucket:Name"];
        Region = configuration["S3Bucket:Region"];
        AwsCredentials = new AwsCredentials
        {
            Key = configuration["AwsConfiguration:AWSAccessKey"],
            Secret = configuration["AwsConfiguration:AWSSecretKey"]
        };
        ValidateConfiguration();
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrEmpty(BucketName))
            throw new InvalidOperationException("S3 bucket name is not configured");
        if (string.IsNullOrEmpty(Region))
            throw new InvalidOperationException("S3 region is not configured");
        if (string.IsNullOrEmpty(AwsCredentials.Key) || string.IsNullOrEmpty(AwsCredentials.Secret))
            throw new InvalidOperationException("AWS credentials are not properly configured");
    }

    /// <summary>
    /// Initializes the S3 object with file content and metadata from an uploaded file.
    /// </summary>
    /// <param name="filePath">The S3 folder path or prefix.</param>
    /// <param name="file">The uploaded file (IFormFile).</param>
    /// <param name="s3ObjectKey">Optional custom S3 object key.</param>
    /// <param name="contentType">Optional custom MIME type.</param>
    public async Task Initialize(string filePath, IFormFile file, string? s3ObjectKey = null, string? contentType = null)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));
        if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));
        ThrowIfDisposed();
        var fileExt = Path.GetExtension(file.FileName);
        S3ObjectKey = s3ObjectKey ?? $"{filePath}{Guid.NewGuid()}{fileExt}";
        S3ObjectUrl = $"https://{BucketName}.s3.{Region}.amazonaws.com/{S3ObjectKey}";
        ContentType = contentType ?? file.ContentType;
        using (var ms = new MemoryStream())
        {
            await file.CopyToAsync(ms);
            Content = ms.ToArray();
        }
    }

    /// <summary>
    /// Initializes the S3 object with an existing S3 object key.
    /// </summary>
    /// <param name="s3ObjectKey">The S3 object key (path/filename).</param>
    public void Initialize(string s3ObjectKey)
    {
        if (string.IsNullOrEmpty(s3ObjectKey)) throw new ArgumentNullException(nameof(s3ObjectKey));
        ThrowIfDisposed();
        S3ObjectKey = s3ObjectKey;
        S3ObjectUrl = $"https://{BucketName}.s3.{Region}.amazonaws.com/{S3ObjectKey}";
    }

    /// <summary>
    /// Sets the file content and optionally the MIME type.
    /// </summary>
    /// <param name="content">The file content as a byte array.</param>
    /// <param name="contentType">Optional MIME type.</param>
    public void SetContent(byte[] content, string? contentType = null)
    {
        if (content == null) throw new ArgumentNullException(nameof(content));
        ThrowIfDisposed();
        Content = content;
        if (contentType != null)
        {
            ContentType = contentType;
        }
    }

    /// <summary>
    /// Disposes the object and clears sensitive data.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Clear sensitive data
                Content = null;
                AwsCredentials = null;
            }
            _disposed = true;
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(S3ObjectCustom));
    }

    /// <summary>
    /// Finalizer to ensure resources are released.
    /// </summary>
    ~S3ObjectCustom()
    {
        Dispose(false);
    }
}
