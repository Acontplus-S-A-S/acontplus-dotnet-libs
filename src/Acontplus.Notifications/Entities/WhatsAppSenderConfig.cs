namespace Acontplus.Notifications.Entities;

public class WhatsAppSenderConfig : BaseEntity
{
    public int? CompanyId { get; set; } // Nullable: NULL for global configuration
    public bool IsGlobal { get; set; } // True for global settings, false for company-specific
    [Required, MaxLength(50)] public required string AccountId { get; set; }
    [Required, MaxLength(300)] public required string AuthToken { get; set; }
    [MaxLength(25)] public string? PhoneNumber { get; set; }
    [MaxLength(250)] public string? UrlService { get; set; }
}