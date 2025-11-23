namespace Acontplus.Notifications.Entities;

public class WhatsAppMessage : BaseEntity
{
    private string? _decompressedMessage;
    public int NotificationId { get; set; }
    public required Notification Notification { get; set; }
    public int WhatsAppSenderConfigId { get; set; }
    public required WhatsAppSenderConfig WhatsAppSenderConfig { get; set; }
    [Required] public required byte[] CompressedMessage { get; set; }
    [MaxLength(150)] public string? Os { get; set; }
    [MaxLength(150)] public string? Browser { get; set; }
    [Required, MaxLength(25)] public required string RecipientPhoneNumber { get; set; }
    public int PriorityId { get; set; }
    public required Priority Priority { get; set; }
    public int StatusId { get; set; }
    public required Status Status { get; set; }
    public DateTime? ScheduledAt { get; set; } // When the whatsapp message is scheduled to be sent
    public DateTime? SentAt { get; set; } // When the whatsapp message was actually sent

    [NotMapped]
    public string Message
    {
        get
        {
            if (_decompressedMessage == null)
            {
                var decompressedBytes = CompressionUtils.DecompressGZip(CompressedMessage);
                _decompressedMessage = Encoding.UTF8.GetString(decompressedBytes);
            }

            return _decompressedMessage;
        }
        set
        {
            var stringBytes = Encoding.UTF8.GetBytes(value);
            CompressedMessage = CompressionUtils.CompressGZip(stringBytes);
            _decompressedMessage = value; // Cache the value
        }
    }
}
