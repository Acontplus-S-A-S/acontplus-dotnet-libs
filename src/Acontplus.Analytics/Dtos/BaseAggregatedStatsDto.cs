namespace Acontplus.Analytics.Dtos;

/// <summary>
/// Base aggregated statistics DTO - Time-series data for trend analysis
/// </summary>
public class BaseAggregatedStatsDto
{
    // === TIME DIMENSION ===
    /// <summary>Time period for this data point</summary>
    public DateTime Period { get; set; }

    /// <summary>Human-readable period label</summary>
    public string PeriodLabel { get; set; } = string.Empty;

    /// <summary>Grouping interval (hour/day/week/month/quarter/year)</summary>
    public string GroupBy { get; set; } = string.Empty;

    // === METRIC IDENTIFICATION ===
    /// <summary>Metric being measured</summary>
    public string Metric { get; set; } = string.Empty;

    /// <summary>Primary category/dimension</summary>
    public string? Category { get; set; }

    /// <summary>Secondary category/sub-dimension</summary>
    public string? SubCategory { get; set; }

    // === STATISTICAL AGGREGATES ===
    /// <summary>Primary aggregated value</summary>
    public decimal Value { get; set; }

    /// <summary>Minimum value in period</summary>
    public decimal MinValue { get; set; }

    /// <summary>Maximum value in period</summary>
    public decimal MaxValue { get; set; }

    /// <summary>Average value in period</summary>
    public decimal AvgValue { get; set; }

    /// <summary>Count of data points</summary>
    public int Count { get; set; }

    /// <summary>Sum of all values</summary>
    public decimal Sum { get; set; }

    // === COMPARATIVE ANALYSIS ===
    /// <summary>Value from equivalent previous period</summary>
    public decimal? PreviousPeriodValue { get; set; }

    /// <summary>Percentage change from previous period</summary>
    public decimal? ChangePercent { get; set; }

    /// <summary>Absolute change from previous period</summary>
    public decimal? ChangeAbsolute { get; set; }

    /// <summary>Trend direction (up/down/stable)</summary>
    public string? Trend { get; set; }

    // === DISTRIBUTION METRICS ===
    /// <summary>25th percentile value</summary>
    public decimal? Percentile25 { get; set; }

    /// <summary>50th percentile/median value</summary>
    public decimal? Percentile50 { get; set; }

    /// <summary>75th percentile value</summary>
    public decimal? Percentile75 { get; set; }

    /// <summary>Standard deviation</summary>
    public decimal? StandardDeviation { get; set; }

    /// <summary>Variance</summary>
    public decimal? Variance { get; set; }

    /// <summary>Localization metadata for chart labels and UI display (e.g., Spanish, English)</summary>
    public Dictionary<string, string>? Labels { get; set; }
}
