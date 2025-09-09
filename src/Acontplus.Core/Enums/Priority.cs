namespace Acontplus.Core.Enums;

/// <summary>
/// Priority levels commonly used in business applications
/// </summary>
public enum Priority
{
    /// <summary>
    /// Lowest priority level
    /// </summary>
    Low = 1,

    /// <summary>
    /// Normal priority level (default)
    /// </summary>
    Normal = 2,

    /// <summary>
    /// High priority level
    /// </summary>
    High = 3,

    /// <summary>
    /// Critical priority level - requires immediate attention
    /// </summary>
    Critical = 4,

    /// <summary>
    /// Emergency priority level - highest possible priority
    /// </summary>
    Emergency = 5
}
