namespace Acontplus.Analytics.Models;

/// <summary>
/// SQL template fragments for building reusable statistics stored procedures
/// </summary>
public static class StatsSqlTemplates
{
    /// <summary>
    /// Common date range parameters with defaults
    /// </summary>
    public const string DateRangeParams = @"
    @StartDate DATETIME2 = NULL,
    @EndDate DATETIME2 = NULL
";

    /// <summary>
    /// Default date range initialization (last 30 days if not provided)
    /// </summary>
    public const string DateRangeDefaults = @"
    SET @StartDate = ISNULL(@StartDate, DATEADD(DAY, -30, GETUTCDATE()));
    SET @EndDate = ISNULL(@EndDate, GETUTCDATE());
";

    /// <summary>
    /// Calculate previous period dates for comparison
    /// </summary>
    public const string PreviousPeriodCalc = @"
    DECLARE @PeriodDays INT = DATEDIFF(DAY, @StartDate, @EndDate);
    DECLARE @PrevStartDate DATETIME2 = DATEADD(DAY, -@PeriodDays, @StartDate);
    DECLARE @PrevEndDate DATETIME2 = @StartDate;
";

    /// <summary>
    /// Common filter params for aggregated stats
    /// </summary>
    public const string AggregationParams = @"
    @StartDate DATETIME2 = NULL,
    @EndDate DATETIME2 = NULL,
    @GroupBy NVARCHAR(50) = 'day',
    @MetricType NVARCHAR(50) = 'revenue',
    @Category NVARCHAR(100) = NULL
";

    /// <summary>
    /// Grouping CASE statement for time periods
    /// </summary>
    public const string GroupByCase = @"
    CASE @GroupBy
        WHEN 'hour' THEN DATEADD(HOUR, DATEDIFF(HOUR, 0, {0}), 0)
        WHEN 'day' THEN CAST({0} AS DATE)
        WHEN 'week' THEN DATEADD(DAY, -(DATEPART(WEEKDAY, {0}) - 1), CAST({0} AS DATE))
        WHEN 'month' THEN DATEADD(MONTH, DATEDIFF(MONTH, 0, {0}), 0)
        WHEN 'quarter' THEN DATEADD(QUARTER, DATEDIFF(QUARTER, 0, {0}), 0)
        WHEN 'year' THEN DATEADD(YEAR, DATEDIFF(YEAR, 0, {0}), 0)
        ELSE CAST({0} AS DATE)
    END
";

    /// <summary>
    /// Period label formatting
    /// </summary>
    public const string PeriodLabelFormat = @"
    FORMAT({0},
        CASE @GroupBy
            WHEN 'hour' THEN 'yyyy-MM-dd HH:00'
            WHEN 'day' THEN 'yyyy-MM-dd'
            WHEN 'week' THEN 'yyyy-MM-dd'
            WHEN 'month' THEN 'yyyy-MM'
            WHEN 'quarter' THEN 'yyyy-Qq'
            WHEN 'year' THEN 'yyyy'
            ELSE 'yyyy-MM-dd'
        END
    )
";

    /// <summary>
    /// Moving average calculation (7-period)
    /// </summary>
    public const string MovingAverage7 = @"
    AVG({0}) OVER (ORDER BY {1} ROWS BETWEEN 6 PRECEDING AND CURRENT ROW)
";

    /// <summary>
    /// Moving average calculation (30-period)
    /// </summary>
    public const string MovingAverage30 = @"
    AVG({0}) OVER (ORDER BY {1} ROWS BETWEEN 29 PRECEDING AND CURRENT ROW)
";

    /// <summary>
    /// Lag function for previous period comparison
    /// </summary>
    public const string LagPreviousPeriod = @"
    LAG({0}, 1) OVER (ORDER BY {1})
";

    /// <summary>
    /// Lag function for same period last week
    /// </summary>
    public const string LagLastWeek = @"
    LAG({0}, 7) OVER (ORDER BY {1})
";

    /// <summary>
    /// Lag function for same period last month
    /// </summary>
    public const string LagLastMonth = @"
    LAG({0}, 30) OVER (ORDER BY {1})
";

    /// <summary>
    /// Percentage change calculation
    /// </summary>
    public const string PercentChange = @"
    CASE WHEN {1} > 0
        THEN CAST(({0} - {1}) * 100.0 / {1} AS DECIMAL(10,2))
        ELSE NULL
    END
";

    /// <summary>
    /// Trend direction classification
    /// </summary>
    public const string TrendDirection = @"
    CASE
        WHEN {0} > ISNULL({1}, 0) * 1.05 THEN 'up'
        WHEN {0} < ISNULL({1}, 0) * 0.95 THEN 'down'
        ELSE 'stable'
    END
";

    /// <summary>
    /// Anomaly detection (simple threshold-based)
    /// </summary>
    public const string AnomalyDetection = @"
    CASE
        WHEN {0} > {1} * 2 OR {0} < {1} * 0.5 THEN 1
        ELSE 0
    END
";

    /// <summary>
    /// Weekend/weekday classification
    /// </summary>
    public const string IsWeekend = @"
    CASE WHEN DATEPART(WEEKDAY, {0}) IN (1, 7) THEN 1 ELSE 0 END
";

    /// <summary>
    /// Real-time time range calculations
    /// </summary>
    public const string RealTimeRanges = @"
    DECLARE @Now DATETIME2 = GETUTCDATE();
    DECLARE @FiveMinAgo DATETIME2 = DATEADD(MINUTE, -5, @Now);
    DECLARE @FifteenMinAgo DATETIME2 = DATEADD(MINUTE, -15, @Now);
    DECLARE @CurrentHourStart DATETIME2 = DATEADD(HOUR, DATEDIFF(HOUR, 0, @Now), 0);
    DECLARE @TodayStart DATETIME2 = CAST(CAST(@Now AS DATE) AS DATETIME2);
";

    /// <summary>
    /// Common field mappings guide
    /// </summary>
    public const string CommonFieldMappings = @"
-- Field Mapping Guide for Cross-Domain Consistency:
-- Transactions: Orders, Sales, Invoices, Tickets, Jobs, Appointments, etc.
-- Entities: Customers, Clients, Suppliers, Patients, Members, etc.
-- Items: Products, Services, Articles, SKUs, etc.
-- Status: Active/Pending/Completed/Cancelled
-- Revenue: Total, Subtotal, Tax, Discount, Net
-- Timestamps: CreatedAt, UpdatedAt, CompletedAt, CancelledAt
";
}
