namespace Acontplus.Notifications.Entities;

public class EmailQueue : BaseEntity
{
    private string? _decompressedBody;
    public int NotificationId { get; set; } // Links to Notification table
    public required Notification Notification { get; set; }
    public int EmailSenderConfigId { get; set; } // Sender email sender configuration
    public required EmailSenderConfig EmailSenderConfig { get; set; }
    [Required, MaxLength(254)] public required string RecipientEmail { get; set; } // Email address of the recipient
    [Required, MaxLength(300)] public required string Subject { get; set; }
    [MaxLength(1000)] public string? Cc { get; set; }
    public bool IsHtml { get; set; }
    [Required] public required byte[] CompressedBody { get; set; }
    public int PriorityId { get; set; }
    public required Priority Priority { get; set; }
    public int StatusId { get; set; }
    public required Status Status { get; set; }
    public int? RetryCount { get; set; } // Number of retry attempts
    public DateTime? ScheduledAt { get; set; } // When the email is scheduled to be sent
    public DateTime? SentAt { get; set; } // When the email was actually sent
    [MaxLength(150)] public string? Template { get; set; }

    [NotMapped]
    public string Content
    {
        get
        {
            switch (_decompressedBody)
            {
                case null when true:
                    {
                        var decompressedBytes = CompressionUtils.DecompressGZip(CompressedBody);
                        _decompressedBody = Encoding.UTF8.GetString(decompressedBytes);
                        break;
                    }
            }

            return _decompressedBody;
        }
        set
        {
            var stringBytes = Encoding.UTF8.GetBytes(value);
            CompressedBody = CompressionUtils.CompressGZip(stringBytes);
            _decompressedBody = value; // Cache the value
        }
    }
}