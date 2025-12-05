namespace Acontplus.Analytics.Dtos;

/// <summary>
/// Base trend analysis DTO - Comprehensive time-series with forecasting support
/// </summary>
public class BaseTrendDto
{
    // === TIME SERIES ===
    /// <summary>Date/time of this data point</summary>
    public DateTime Date { get; set; }

    /// <summary>Formatted date label for display</summary>
    public string DateLabel { get; set; } = string.Empty;

    /// <summary>Day of week (1=Sunday, 7=Saturday)</summary>
    public int DayOfWeek { get; set; }

    /// <summary>Hour of day (0-23)</summary>
    public int Hour { get; set; }

    /// <summary>Is weekend day</summary>
    public bool IsWeekend { get; set; }

    /// <summary>Is holiday/special day</summary>
    public bool IsHoliday { get; set; }

    // === METRIC DATA ===
    /// <summary>Metric being tracked</summary>
    public string Metric { get; set; } = string.Empty;

    /// <summary>Actual observed value</summary>
    public decimal Value { get; set; }

    /// <summary>Target/goal value for this period</summary>
    public decimal? Target { get; set; }

    /// <summary>Forecasted/predicted value</summary>
    public decimal? Forecast { get; set; }

    // === MOVING AVERAGES ===
    /// <summary>7-period moving average</summary>
    public decimal? MovingAverage7 { get; set; }

    /// <summary>30-period moving average</summary>
    public decimal? MovingAverage30 { get; set; }

    /// <summary>90-period moving average</summary>
    public decimal? MovingAverage90 { get; set; }

    // === TREND ANALYSIS ===
    /// <summary>Change percentage from previous period</summary>
    public decimal? ChangePercent { get; set; }

    /// <summary>Absolute change from previous period</summary>
    public decimal? ChangeAbsolute { get; set; }

    /// <summary>Trend direction (up/down/stable)</summary>
    public string? TrendDirection { get; set; }

    /// <summary>Volatility/variance metric</summary>
    public decimal? Volatility { get; set; }

    // === COMPARATIVE METRICS ===
    /// <summary>Value from same period last week</summary>
    public decimal? SamePeriodLastWeek { get; set; }

    /// <summary>Value from same period last month</summary>
    public decimal? SamePeriodLastMonth { get; set; }

    /// <summary>Value from same period last year</summary>
    public decimal? SamePeriodLastYear { get; set; }

    /// <summary>Average for this period across history</summary>
    public decimal? PeriodAverage { get; set; }

    /// <summary>Percentage of period average</summary>
    public decimal? PercentOfPeriodAvg { get; set; }

    // === CLASSIFICATION ===
    /// <summary>Custom label/tag for this data point</summary>
    public string? Label { get; set; }

    /// <summary>Category/segment</summary>
    public string? Category { get; set; }

    /// <summary>Is this an anomaly/outlier</summary>
    public bool IsAnomaly { get; set; }

    /// <summary>Anomaly score (0-1, higher = more anomalous)</summary>
    public decimal? AnomalyScore { get; set; }

    // === METADATA ===
    /// <summary>Additional notes or context</summary>
    public string? Notes { get; set; }

    /// <summary>Additional metadata as key-value pairs</summary>
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>Localization metadata for chart labels and UI display (e.g., Spanish, English)</summary>
    public Dictionary<string, string>? Labels { get; set; }
}
