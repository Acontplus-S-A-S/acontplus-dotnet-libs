namespace Acontplus.Core.Enums;

/// <summary>
/// Address type enum for different types of addresses in business applications
/// </summary>
public enum AddressType
{
    /// <summary>
    /// Home/residential address
    /// </summary>
    Home = 1,

    /// <summary>
    /// Work/business address
    /// </summary>
    Work = 2,

    /// <summary>
    /// Billing address for payments
    /// </summary>
    Billing = 3,

    /// <summary>
    /// Shipping/delivery address
    /// </summary>
    Shipping = 4,

    /// <summary>
    /// Mailing address for correspondence
    /// </summary>
    Mailing = 5,

    /// <summary>
    /// Temporary address
    /// </summary>
    Temporary = 6,

    /// <summary>
    /// Primary address (main)
    /// </summary>
    Primary = 7,

    /// <summary>
    /// Secondary address (alternative)
    /// </summary>
    Secondary = 8,

    /// <summary>
    /// Emergency contact address
    /// </summary>
    Emergency = 9,

    /// <summary>
    /// Business headquarters address
    /// </summary>
    Headquarters = 10,

    /// <summary>
    /// Branch office address
    /// </summary>
    Branch = 11,

    /// <summary>
    /// Warehouse address
    /// </summary>
    Warehouse = 12
}
