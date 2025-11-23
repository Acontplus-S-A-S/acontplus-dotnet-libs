namespace Acontplus.Core.Enums;

/// <summary>
/// Communication channel type enum for notifications and contact preferences.
/// Represents generic communication channel categories rather than specific platforms.
/// </summary>
public enum CommunicationChannelType
{
    /// <summary>
    /// Email communication
    /// </summary>
    Email = 1,

    /// <summary>
    /// SMS text message
    /// </summary>
    SMS = 2,

    /// <summary>
    /// Phone call
    /// </summary>
    Phone = 3,

    /// <summary>
    /// Push notification (mobile/web)
    /// </summary>
    Push = 4,

    /// <summary>
    /// In-app notification
    /// </summary>
    InApp = 5,

    /// <summary>
    /// Postal mail
    /// </summary>
    Mail = 6,

    /// <summary>
    /// Fax communication
    /// </summary>
    Fax = 7,

    /// <summary>
    /// Instant messaging (e.g., WhatsApp, Telegram, Slack, Teams)
    /// </summary>
    InstantMessaging = 8
}
