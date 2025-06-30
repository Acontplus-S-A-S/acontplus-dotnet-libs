namespace Acontplus.Notifications.Entities;

public class NotificationType : BaseEntity
{
    [Required, MaxLength(5)] public required string Code { get; set; }
    public required string Name { get; set; } // e.g., "Email", "Sms", "Push", "WhatsApp"
}