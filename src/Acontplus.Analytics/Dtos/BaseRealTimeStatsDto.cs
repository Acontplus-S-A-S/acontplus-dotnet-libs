namespace Acontplus.Analytics.Dtos;

/// <summary>
/// Base real-time statistics DTO - Live operational metrics for any business
/// </summary>
public class BaseRealTimeStatsDto
{
    // === CURRENT ACTIVITY ===
    /// <summary>Active/pending operations right now</summary>
    public int ActiveOperations { get; set; }

    /// <summary>Operations in the last 5 minutes</summary>
    public int OperationsLast5Min { get; set; }

    /// <summary>Operations in the last 15 minutes</summary>
    public int OperationsLast15Min { get; set; }

    /// <summary>Operations in the current hour</summary>
    public int OperationsThisHour { get; set; }

    // === REVENUE TRACKING ===
    /// <summary>Revenue in the current hour</summary>
    public decimal CurrentHourRevenue { get; set; }

    /// <summary>Total revenue today</summary>
    public decimal TodayRevenue { get; set; }

    /// <summary>Average transaction/ticket size</summary>
    public decimal AverageTicketSize { get; set; }

    // === PROCESSING METRICS ===
    /// <summary>Items in queue/pending</summary>
    public int ItemsInQueue { get; set; }

    /// <summary>Items being processed</summary>
    public int ItemsInProcess { get; set; }

    /// <summary>Items completed/ready</summary>
    public int ItemsCompleted { get; set; }

    /// <summary>Average processing time in minutes</summary>
    public decimal AverageProcessingTime { get; set; }

    /// <summary>Oldest pending item/operation age in minutes</summary>
    public decimal OldestPendingMinutes { get; set; }

    /// <summary>Delayed/overdue operations count</summary>
    public int DelayedOperations { get; set; }

    // === PERFORMANCE INDICATORS ===
    /// <summary>Operations per hour rate</summary>
    public decimal OperationsPerHour { get; set; }

    /// <summary>Revenue per hour rate</summary>
    public decimal RevenuePerHour { get; set; }

    /// <summary>Current load status (Slow/Normal/Busy/Peak)</summary>
    public string? CurrentLoadStatus { get; set; }

    // === ALERTS & NOTIFICATIONS ===
    /// <summary>Critical alerts count</summary>
    public int CriticalAlerts { get; set; }

    /// <summary>Warning alerts count</summary>
    public int WarningAlerts { get; set; }

    // === CAPACITY METRICS ===
    /// <summary>Total capacity/resources available</summary>
    public int TotalCapacity { get; set; }

    /// <summary>Currently utilized capacity</summary>
    public int UsedCapacity { get; set; }

    /// <summary>Available capacity</summary>
    public int AvailableCapacity { get; set; }

    /// <summary>Capacity utilization rate (%)</summary>
    public decimal CapacityUtilizationRate { get; set; }

    /// <summary>Localization metadata for chart labels and UI display (e.g., Spanish, English)</summary>
    public Dictionary<string, string>? Labels { get; set; }
}
