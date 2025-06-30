using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Acontplus.S3Application.Models;

public sealed class S3ObjectCustom : IDisposable
{
    private readonly IConfiguration _configuration;
    private bool _disposed;

    public string Region { get; private set; }
    public string BucketName { get; private set; }
    public byte[]? Content { get; private set; }
    public string S3ObjectKey { get; private set; }
    public string S3ObjectUrl { get; private set; }
    public AwsCredentials? AwsCredentials { get; private set; }
    public string ContentType { get; private set; }

    public S3ObjectCustom(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        // Initialize default values from configuration
        BucketName = configuration["S3Bucket:Name"];
        Region = configuration["S3Bucket:Region"];

        // Initialize AWS credentials
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

    public void Initialize(string s3ObjectKey)
    {
        if (string.IsNullOrEmpty(s3ObjectKey)) throw new ArgumentNullException(nameof(s3ObjectKey));

        ThrowIfDisposed();

        S3ObjectKey = s3ObjectKey;
        S3ObjectUrl = $"https://{BucketName}.s3.{Region}.amazonaws.com/{S3ObjectKey}";
    }

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

    ~S3ObjectCustom()
    {
        Dispose(false);
    }
}
