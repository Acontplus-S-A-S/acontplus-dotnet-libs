namespace Acontplus.Notifications.Entities;

public class Notification : BaseEntity
{
    private string? _decompressedParameters;
    public int NotificationTypeId { get; set; }
    public required NotificationType NotificationType { get; set; }
    [Required] public required byte[] CompressedParameters { get; set; } // Any custom parameters
    [MaxLength(100)] public string? Title { get; set; }
    [MaxLength(300)] public string? Subject { get; set; }
    [MaxLength(300)] public string? Message { get; set; }
    public bool IsRead { get; set; }
    public ICollection<Attachment>? Attachments { get; set; }

    [NotMapped]
    public string Parameters
    {
        get
        {
            switch (_decompressedParameters)
            {
                case null when true:
                    {
                        var decompressedBytes = CompressionUtils.DecompressGZip(CompressedParameters);
                        _decompressedParameters = Encoding.UTF8.GetString(decompressedBytes);
                        break;
                    }
            }

            return _decompressedParameters;
        }
        set
        {
            var stringBytes = Encoding.UTF8.GetBytes(value);
            CompressedParameters = CompressionUtils.CompressGZip(stringBytes);
            _decompressedParameters = value; // Cache the value
        }
    }
}