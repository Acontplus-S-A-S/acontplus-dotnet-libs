namespace Acontplus.Core.Enums;

/// <summary>
/// User role type enum for authorization and access control across applications.
/// Represents generic role levels rather than context-specific positions.
/// Higher numeric values generally indicate higher privilege levels.
/// </summary>
public enum UserRoleType
{
    /// <summary>
    /// Guest user with minimal access (unauthenticated or limited access)
    /// </summary>
    Guest = 1,

    /// <summary>
    /// Standard authenticated user with basic access to their own data
    /// </summary>
    User = 2,

    /// <summary>
    /// Employee with internal organizational access
    /// </summary>
    Employee = 3,

    /// <summary>
    /// Manager with team/department management capabilities
    /// </summary>
    Manager = 4,

    /// <summary>
    /// Administrator with elevated organizational access and user management
    /// </summary>
    Administrator = 5,

    /// <summary>
    /// Super administrator with full system access and configuration capabilities
    /// </summary>
    SuperAdmin = 6,

    /// <summary>
    /// Service account for automated processes, system integrations, and APIs
    /// </summary>
    ServiceAccount = 7
}
