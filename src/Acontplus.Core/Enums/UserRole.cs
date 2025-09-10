namespace Acontplus.Core.Enums;

/// <summary>
/// User role enum for authorization and access control across applications
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Guest user with minimal access
    /// </summary>
    Guest = 1,

    /// <summary>
    /// Standard user with basic access
    /// </summary>
    User = 2,

    /// <summary>
    /// Customer with purchase access
    /// </summary>
    Customer = 3,

    /// <summary>
    /// Employee with internal access
    /// </summary>
    Employee = 4,

    /// <summary>
    /// Manager with team management access
    /// </summary>
    Manager = 5,

    /// <summary>
    /// Supervisor with oversight access
    /// </summary>
    Supervisor = 6,

    /// <summary>
    /// Administrator with elevated access
    /// </summary>
    Administrator = 7,

    /// <summary>
    /// Super administrator with full system access
    /// </summary>
    SuperAdmin = 8,

    /// <summary>
    /// System administrator with technical access
    /// </summary>
    SystemAdmin = 9,

    /// <summary>
    /// Support agent with help desk access
    /// </summary>
    Support = 10,

    /// <summary>
    /// Moderator with content moderation access
    /// </summary>
    Moderator = 11,

    /// <summary>
    /// Auditor with read-only audit access
    /// </summary>
    Auditor = 12,

    /// <summary>
    /// API user for system integrations
    /// </summary>
    ApiUser = 13,

    /// <summary>
    /// Service account for automated processes
    /// </summary>
    ServiceAccount = 14
}
