namespace Acontplus.Core.Enums;

/// <summary>
/// Common business status states that can be applied across different business applications
/// </summary>
public enum BusinessStatus
{
    /// <summary>
    /// Entity is in draft state, not yet submitted or processed
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Entity is pending review or approval
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Entity is currently being processed
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// Entity has been approved
    /// </summary>
    Approved = 3,

    /// <summary>
    /// Entity has been rejected
    /// </summary>
    Rejected = 4,

    /// <summary>
    /// Entity has been completed successfully
    /// </summary>
    Completed = 5,

    /// <summary>
    /// Entity has been cancelled
    /// </summary>
    Cancelled = 6,

    /// <summary>
    /// Entity is on hold or suspended
    /// </summary>
    OnHold = 7,

    /// <summary>
    /// Entity has expired
    /// </summary>
    Expired = 8,

    /// <summary>
    /// Entity is active and operational
    /// </summary>
    Active = 9,

    /// <summary>
    /// Entity is inactive or disabled
    /// </summary>
    Inactive = 10,

    /// <summary>
    /// Entity has been archived
    /// </summary>
    Archived = 11,

    /// <summary>
    /// Entity has been deleted (soft delete)
    /// </summary>
    Deleted = 12
}
