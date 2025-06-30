namespace Acontplus.Notifications.Entities;

public class Status : BaseEntity
{
    [Required, MaxLength(5)] public required string Code { get; set; }
    public string? Name { get; set; } // e.g., "Queued", "Processing", "Sent", "Failed"
}