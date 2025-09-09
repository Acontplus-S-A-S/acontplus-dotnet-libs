namespace Acontplus.Core.Enums;

/// <summary>
/// Event types for business applications and audit trails
/// </summary>
public enum EventType
{
    /// <summary>
    /// General or unspecified event
    /// </summary>
    General = 0,

    /// <summary>
    /// User authentication event
    /// </summary>
    Authentication = 1,

    /// <summary>
    /// User authorization event
    /// </summary>
    Authorization = 2,

    /// <summary>
    /// Data creation event
    /// </summary>
    Create = 3,

    /// <summary>
    /// Data update or modification event
    /// </summary>
    Update = 4,

    /// <summary>
    /// Data deletion event
    /// </summary>
    Delete = 5,

    /// <summary>
    /// Data read or access event
    /// </summary>
    Read = 6,

    /// <summary>
    /// System configuration change
    /// </summary>
    Configuration = 7,

    /// <summary>
    /// Business workflow or process event
    /// </summary>
    Workflow = 8,

    /// <summary>
    /// Payment or financial transaction
    /// </summary>
    Payment = 9,

    /// <summary>
    /// Communication event (email, notification)
    /// </summary>
    Communication = 10,

    /// <summary>
    /// Import or export operation
    /// </summary>
    Import = 11,

    /// <summary>
    /// Export operation
    /// </summary>
    Export = 12,

    /// <summary>
    /// System error or exception
    /// </summary>
    Error = 13,

    /// <summary>
    /// Security-related event
    /// </summary>
    Security = 14,

    /// <summary>
    /// Performance or monitoring event
    /// </summary>
    Performance = 15,

    /// <summary>
    /// Integration with external systems
    /// </summary>
    Integration = 16,

    /// <summary>
    /// Backup or maintenance operation
    /// </summary>
    Maintenance = 17,

    /// <summary>
    /// Compliance or regulatory event
    /// </summary>
    Compliance = 18
}
