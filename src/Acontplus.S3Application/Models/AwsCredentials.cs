namespace Acontplus.S3Application.Models;

/// <summary>
/// Represents AWS credentials required for S3 operations.
/// </summary>
public class AwsCredentials
{
    /// <summary>
    /// Gets or sets the AWS access key.
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    /// Gets or sets the AWS secret key.
    /// </summary>
    public required string Secret { get; set; }
}
