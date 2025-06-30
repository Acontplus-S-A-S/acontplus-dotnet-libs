namespace Acontplus.Notifications.Entities;

public class Priority : BaseEntity
{
    [Required, MaxLength(5)] public required string Code { get; set; }
    public string? Name { get; set; } // e.g., "High", "Medium", "Low"
}