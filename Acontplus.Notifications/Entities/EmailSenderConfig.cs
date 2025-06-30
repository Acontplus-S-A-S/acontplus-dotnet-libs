namespace Acontplus.Notifications.Entities;

public class EmailSenderConfig : BaseEntity
{
    // Scope of the configuration
    public int? CompanyId { get; set; } // Nullable: NULL for global configuration
    public bool IsGlobal { get; set; } // True for global settings, false for company-specific

    // Email configuration
    [Required, MaxLength(150)] public required string SenderEmail { get; set; } // Example: no-reply@example.com
    [Required, MaxLength(300)] public required string SenderName { get; set; } // Example: ERP Notifications
    [Required, MaxLength(150)] public required string SmtpServer { get; set; } // Example: smtp.example.com
    public int SmtpPort { get; set; } // Example: 587
    public bool UseSsl { get; set; } // True if SSL/TLS is required
    [Required, MaxLength(150)] public required string Username { get; set; } // SMTP authentication username
    public byte[]? EncryptedPassword { get; set; } // Encrypted password
    public string? PasswordHash { get; set; }
    public string? Password { get; set; }
}