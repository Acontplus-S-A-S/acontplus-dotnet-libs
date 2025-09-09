namespace Acontplus.Core.Enums;

/// <summary>
/// Marital status enum for person entities across business applications
/// </summary>
public enum MaritalStatus
{
    /// <summary>
    /// Not specified or prefer not to disclose
    /// </summary>
    NotSpecified = 0,

    /// <summary>
    /// Single/Never married
    /// </summary>
    Single = 1,

    /// <summary>
    /// Married
    /// </summary>
    Married = 2,

    /// <summary>
    /// Divorced
    /// </summary>
    Divorced = 3,

    /// <summary>
    /// Widowed
    /// </summary>
    Widowed = 4,

    /// <summary>
    /// Separated (but not legally divorced)
    /// </summary>
    Separated = 5,

    /// <summary>
    /// In a domestic partnership or civil union
    /// </summary>
    DomesticPartnership = 6,

    /// <summary>
    /// Common law marriage
    /// </summary>
    CommonLaw = 7
}
