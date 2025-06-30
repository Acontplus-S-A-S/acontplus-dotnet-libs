namespace Acontplus.Notifications.Entities;

public class NotificationRecipient : BaseEntity
{
    // Links to Notification
    public int NotificationId { get; set; }
    public required Notification Notification { get; set; }
    public int? GroupId { get; set; } // Nullable for user-specific notifications
    public NotificationGroup? Group { get; set; } // Navigation property for group notifications

    // Status
    public bool IsRead { get; set; } // Tracks if the recipient has read the notification
    public DateTime? ReadAt { get; set; } // Timestamp for when it was read
}