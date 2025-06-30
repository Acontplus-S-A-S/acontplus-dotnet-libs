namespace Common.Logging;

public class LoggingOptions
{
    public bool EnableLocalFile { get; set; } = true;
    public bool Buffered { get; set; } = true;
    public bool Shared { get; set; } = false; //If buffered is false, can set shared to true
    public string LocalFilePath { get; set; } = "logs/log-.log";
    public string RollingInterval { get; set; } = "Day";
    public int? RetainedFileCountLimit { get; set; } = 7;
    public long? FileSizeLimitBytes { get; set; } = 10 * 1024 * 1024;
    public bool EnableS3Logging { get; set; } = false;
    public string S3BucketName { get; set; }
    public string S3AccessKey { get; set; }
    public string S3SecretKey { get; set; }
    public bool EnableDatabaseLogging { get; set; } = false;
    public string DatabaseConnectionString { get; set; }
    public string TimeZoneId { get; set; } = "UTC"; // Default to UTC
}
