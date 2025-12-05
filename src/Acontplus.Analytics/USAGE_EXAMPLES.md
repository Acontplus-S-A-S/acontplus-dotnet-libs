# Acontplus.Analytics - Usage Examples

## Dynamic Statistics Service

### 1. Register the Service with Custom Stored Procedure Names

```csharp
using Acontplus.Analytics.Extensions;

// In your Startup.cs or Program.cs
builder.Services.AddStatisticsService<
    DashboardStatsDto,
    RealTimeStatsDto,
    AggregatedStatsDto,
    TrendDto>(
    dashboardSpName: "Restaurant.StatisticsGetDashboard",
    realTimeSpName: "Restaurant.StatisticsGetRealTime",
    aggregatedSpName: "Restaurant.StatisticsGetAggregated",
    trendsSpName: "Restaurant.StatisticsGetTrends");
```

### 2. Register Using Module-Based Naming Convention

```csharp
// Automatically creates SP names: Restaurant.StatisticsGetDashboard, etc.
builder.Services.AddStatisticsService<
    DashboardStatsDto,
    RealTimeStatsDto,
    AggregatedStatsDto,
    TrendDto>(
    moduleName: "Restaurant.Statistics");
```

### 3. Define Your Domain-Specific DTOs

```csharp
using Acontplus.Analytics.Dtos;

// Extend base DTOs with domain-specific properties
public class DashboardStatsDto : BaseDashboardStatsDto
{
    // Restaurant-specific properties
    public decimal AveragePreparationTime { get; set; }
    public decimal AverageServiceTime { get; set; }
    public decimal TableTurnoverRate { get; set; }
    public int TotalTips { get; set; }
}

public class RealTimeStatsDto : BaseRealTimeStatsDto
{
    // Restaurant-specific properties
    public int OccupiedTables { get; set; }
    public int AvailableTables { get; set; }
    public int ReservedTables { get; set; }
    public int ActiveWaiters { get; set; }
}

// Use base classes directly if no customization needed
public class AggregatedStatsDto : BaseAggregatedStatsDto { }
public class TrendDto : BaseTrendDto { }
```

### 4. Consume in Your Controller

```csharp
using Acontplus.Analytics.Interfaces;
using Acontplus.Core.Dtos.Requests;

[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService<DashboardStatsDto, RealTimeStatsDto,
        AggregatedStatsDto, TrendDto> _statisticsService;

    public StatisticsController(
        IStatisticsService<DashboardStatsDto, RealTimeStatsDto, AggregatedStatsDto, TrendDto> statisticsService)
    {
        _statisticsService = statisticsService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var filter = new FilterRequest
        {
            Filters = new Dictionary<string, object>
            {
                { "StartDate", startDate ?? DateTime.UtcNow.AddDays(-30) },
                { "EndDate", endDate ?? DateTime.UtcNow }
            }
        };

        var result = await _statisticsService.GetDashboardStatsAsync(filter);

        return result.Match(
            success => Ok(success),
            error => BadRequest(error)
        );
    }

    [HttpGet("realtime")]
    public async Task<IActionResult> GetRealTime()
    {
        var result = await _statisticsService.GetRealTimeStatsAsync();

        return result.Match(
            success => Ok(success),
            error => BadRequest(error)
        );
    }

    [HttpGet("aggregated")]
    public async Task<IActionResult> GetAggregated(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] string groupBy = "day")
    {
        var filter = new FilterRequest
        {
            Filters = new Dictionary<string, object>
            {
                { "StartDate", startDate },
                { "EndDate", endDate },
                { "GroupBy", groupBy }
            }
        };

        var result = await _statisticsService.GetAggregatedStatsAsync(filter);

        return result.Match(
            success => Ok(success),
            error => BadRequest(error)
        );
    }

    [HttpGet("trends")]
    public async Task<IActionResult> GetTrends(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] string metric = "revenue")
    {
        var filter = new FilterRequest
        {
            Filters = new Dictionary<string, object>
            {
                { "StartDate", startDate },
                { "EndDate", endDate },
                { "Metric", metric }
            }
        };

        var result = await _statisticsService.GetTrendsAsync(filter);

        return result.Match(
            success => Ok(success),
            error => BadRequest(error)
        );
    }
}
```

### 5. Implement Application-Level Localization

Since localization is business-specific, implement it in your application:

```csharp
// In your application layer
public class RestaurantStatisticsLocalization
{
    public static Dictionary<string, string> GetSpanishLabels() => new()
    {
        // Base properties
        { "TotalRevenue", "Ingresos Totales" },
        { "NetRevenue", "Ingresos Netos" },
        { "CompletedTransactions", "Transacciones Completadas" },
        { "GrowthRate", "Tasa de Crecimiento" },

        // Restaurant-specific
        { "AveragePreparationTime", "Tiempo Promedio de Preparación" },
        { "AverageServiceTime", "Tiempo Promedio de Servicio" },
        { "TableTurnoverRate", "Tasa de Rotación de Mesas" },
        { "TotalTips", "Propinas Totales" },

        // Chart labels
        { "Chart.Title.Dashboard", "Panel de Control del Restaurante" },
        { "Chart.Axis.Revenue", "Ingresos ($)" },
        { "Chart.Axis.Time", "Tiempo (min)" }
    };

    public static Dictionary<string, string> GetEnglishLabels() => new()
    {
        { "TotalRevenue", "Total Revenue" },
        { "NetRevenue", "Net Revenue" },
        { "CompletedTransactions", "Completed Transactions" },
        { "GrowthRate", "Growth Rate" },
        { "AveragePreparationTime", "Average Preparation Time" },
        { "AverageServiceTime", "Average Service Time" },
        { "TableTurnoverRate", "Table Turnover Rate" },
        { "TotalTips", "Total Tips" },
        { "Chart.Title.Dashboard", "Restaurant Dashboard" },
        { "Chart.Axis.Revenue", "Revenue ($)" },
        { "Chart.Axis.Time", "Time (min)" }
    };
}

// Apply localization in your service or controller
public async Task<Result<DashboardStatsDto, DomainError>> GetLocalizedDashboard(
    FilterRequest filter,
    string language = "es")
{
    var result = await _statisticsService.GetDashboardStatsAsync(filter);

    if (result.IsSuccess)
    {
        result.Value.Labels = language == "es"
            ? RestaurantStatisticsLocalization.GetSpanishLabels()
            : RestaurantStatisticsLocalization.GetEnglishLabels();
    }

    return result;
}
```

### 6. SQL Stored Procedure Example

```sql
CREATE PROCEDURE [Restaurant.StatisticsGetDashboard]
    @StartDate DATETIME2 = NULL,
    @EndDate DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Use date range defaults if not provided
    SET @StartDate = ISNULL(@StartDate, DATEADD(DAY, -30, GETUTCDATE()));
    SET @EndDate = ISNULL(@EndDate, GETUTCDATE());

    SELECT
        -- Transaction Metrics
        COUNT(*) AS TotalTransactions,
        SUM(CASE WHEN Status = 'Completed' THEN 1 ELSE 0 END) AS CompletedTransactions,
        SUM(CASE WHEN Status = 'Cancelled' THEN 1 ELSE 0 END) AS CancelledTransactions,
        SUM(CASE WHEN Status = 'Pending' THEN 1 ELSE 0 END) AS ActiveTransactions,

        -- Revenue Metrics
        SUM(TotalAmount) AS TotalRevenue,
        SUM(TotalAmount - TaxAmount) AS NetRevenue,
        SUM(TaxAmount) AS TotalTax,
        SUM(DiscountAmount) AS TotalDiscounts,
        AVG(TotalAmount) AS AverageTransactionValue,

        -- Restaurant-specific
        AVG(PreparationTimeMinutes) AS AveragePreparationTime,
        AVG(ServiceTimeMinutes) AS AverageServiceTime,
        SUM(Tips) AS TotalTips

    FROM Orders
    WHERE CreatedAt BETWEEN @StartDate AND @EndDate;
END
```

### 7. Benefits

✅ **Dynamic Stored Procedures** - Configure SP names per module
✅ **Flexible Localization** - Integrate with your i18n system
✅ **Type-Safe** - Generic constraints ensure type safety
✅ **Domain-Specific** - Extend base DTOs with custom properties
✅ **Reusable** - Same service for multiple business domains
✅ **Clean Architecture** - Separation of concerns maintained
✅ **No Lock-In** - Library doesn't impose localization strategy
