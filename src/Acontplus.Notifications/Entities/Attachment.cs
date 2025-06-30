namespace Acontplus.Notifications.Entities;

public class Attachment : BaseEntity
{
    public int NotificationId { get; set; }
    public required Notification Notification { get; set; }
    [Required, MaxLength(300)] public required string FileName { get; set; }
    [Required, MaxLength(50)] public required string FileType { get; set; }
    public int? FileSize { get; set; }
    [MaxLength(300)] public string? FilePath { get; set; } // Nullable for optional local storage
    [MaxLength(300)] public string? S3ObjectKey { get; set; } // Nullable for optional cloud storage
    [MaxLength(300)] public string? S3ObjectUrl { get; set; } // Nullable for optional cloud URL
}