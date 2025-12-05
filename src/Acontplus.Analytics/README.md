# Acontplus.Analytics

[![NuGet](https://img.shields.io/nuget/v/Acontplus.Analytics.svg)](https://www.nuget.org/packages/Acontplus.Analytics)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

A comprehensive analytics and statistics library for .NET applications, providing domain-agnostic metrics, trends, and business intelligence capabilities. Built with modern .NET 10 features and designed for cross-domain reusability.

## 🚀 Features

### 📊 Comprehensive Analytics DTOs

- **Dashboard Statistics**: Complete business metrics (transactions, revenue, entities, growth rates)
- **Real-Time Metrics**: Live operational data (current activity, processing metrics, capacity utilization)
- **Aggregated Analytics**: Time-series data with statistical aggregates (min, max, avg, percentiles)
- **Trend Analysis**: Advanced trend tracking with moving averages, forecasting, and anomaly detection

### 🎯 Domain-Agnostic Design

Works across **any business domain**:
- 🛒 E-commerce (orders, products, customers)
- 🏥 Healthcare (patients, appointments, treatments)
- 💰 Finance (transactions, accounts, investments)
- 🏭 Manufacturing (production, inventory, quality)
- 📚 Education (students, courses, assessments)
- 🍽️ Hospitality (reservations, orders, guests)

### 💡 Key Capabilities

- **Generic interface** for consistent analytics implementation
- **SQL template library** for building reusable stored procedures
- **Flexible DTO structure** with optional label dictionaries for localization
- **Comparison metrics** (period-over-period, year-over-year)
- **Statistical functions** (percentiles, standard deviation, variance)
- **Performance tracking** (moving averages, trend detection)

## 📦 Installation

```bash
dotnet add package Acontplus.Analytics
```

### NuGet Package Manager

```bash
Install-Package Acontplus.Analytics
```

### PackageReference

```xml
<PackageReference Include="Acontplus.Analytics" Version="1.0.0" />
```

## 🎯 Quick Start

### 1. Define Your Domain-Specific DTOs

Extend the base DTOs with your domain-specific properties:

```csharp
using Acontplus.Analytics.Dtos;

// Your custom dashboard stats
public class SalesDashboardDto : BaseDashboardStatsDto
{
    public decimal AverageOrderSize { get; set; }
    public int TopSellingProductId { get; set; }
    public string TopSellingProductName { get; set; } = string.Empty;
}

// Your custom real-time stats
public class SalesRealTimeDto : BaseRealTimeStatsDto
{
    public int ActiveCheckouts { get; set; }
    public decimal PendingPaymentsTotal { get; set; }
}

// Use base classes directly or extend them
public class SalesAggregatedDto : BaseAggregatedStatsDto { }
public class SalesTrendDto : BaseTrendDto { }
```

### 2. Implement the Statistics Service

```csharp
using Acontplus.Analytics.Interfaces;
using Acontplus.Core.Dtos.Requests;

public class SalesStatisticsService : IStatisticsService<
    SalesDashboardDto,
    SalesRealTimeDto,
    BaseAggregatedStatsDto,
    BaseTrendDto>
{
    private readonly IRepository<Order> _orderRepository;

    public async Task<Result<SalesDashboardDto, DomainError>> GetDashboardStatsAsync(
        FilterRequest filterRequest,
        CancellationToken cancellationToken = default)
    {
        // Your implementation using repository, stored procedures, etc.
        var stats = await CalculateDashboardMetrics(filterRequest);
        return Result<SalesDashboardDto, DomainError>.Success(stats);
    }

    public async Task<Result<SalesRealTimeDto, DomainError>> GetRealTimeStatsAsync(
        FilterRequest? filterRequest = null,
        CancellationToken cancellationToken = default)
    {
        var stats = await CalculateRealTimeMetrics();
        return Result<SalesRealTimeDto, DomainError>.Success(stats);
    }

    public async Task<Result<List<BaseAggregatedStatsDto>, DomainError>> GetAggregatedStatsAsync(
        FilterRequest filterRequest,
        CancellationToken cancellationToken = default)
    {
        var stats = await CalculateAggregatedMetrics(filterRequest);
        return Result<List<BaseAggregatedStatsDto>, DomainError>.Success(stats);
    }

    public async Task<Result<List<BaseTrendDto>, DomainError>> GetTrendsAsync(
        FilterRequest filterRequest,
        CancellationToken cancellationToken = default)
    {
        var trends = await CalculateTrends(filterRequest);
        return Result<List<BaseTrendDto>, DomainError>.Success(trends);
    }
}
```

### 3. Use SQL Templates for Stored Procedures

```sql
-- Example stored procedure using the SQL templates
CREATE PROCEDURE [dbo].[sp_GetSalesDashboardStats]
    @StartDate DATETIME2 = NULL,
    @EndDate DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Use the date range defaults from StatsSqlTemplates
    SET @StartDate = ISNULL(@StartDate, DATEADD(DAY, -30, GETUTCDATE()));
    SET @EndDate = ISNULL(@EndDate, GETUTCDATE());

    -- Calculate previous period for comparison
    DECLARE @PeriodDays INT = DATEDIFF(DAY, @StartDate, @EndDate);
    DECLARE @PrevStartDate DATETIME2 = DATEADD(DAY, -@PeriodDays, @StartDate);
    DECLARE @PrevEndDate DATETIME2 = @StartDate;

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

        -- Entity Metrics
        COUNT(DISTINCT CustomerId) AS UniqueEntities,
        SUM(CASE WHEN CustomerCreatedDate >= @StartDate THEN 1 ELSE 0 END) AS NewEntities

    FROM Orders
    WHERE CreatedAt BETWEEN @StartDate AND @EndDate;
END
```

### 4. Consume in Your API

```csharp
[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IStatisticsService<SalesDashboardDto, SalesRealTimeDto,
        BaseAggregatedStatsDto, BaseTrendDto> _statsService;

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var filter = new FilterRequest
        {
            StartDate = startDate,
            EndDate = endDate
        };

        var result = await _statsService.GetDashboardStatsAsync(filter);

        return result.Match(
            success => Ok(success),
            error => BadRequest(error)
        );
    }

    [HttpGet("realtime")]
    public async Task<IActionResult> GetRealTime()
    {
        var result = await _statsService.GetRealTimeStatsAsync();

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
            StartDate = startDate,
            EndDate = endDate,
            Filters = $"{{\"Metric\": \"{metric}\"}}"
        };

        var result = await _statsService.GetTrendsAsync(filter);

        return result.Match(
            success => Ok(success),
            error => BadRequest(error)
        );
    }
}
```

## 📊 DTO Reference

### BaseDashboardStatsDto

Comprehensive business metrics for executive dashboards:

| Property | Type | Description |
|----------|------|-------------|
| `TotalTransactions` | int | Total count of operations |
| `CompletedTransactions` | int | Successfully completed |
| `TotalRevenue` | decimal | Gross revenue |
| `NetRevenue` | decimal | Revenue after deductions |
| `GrowthRate` | decimal | Growth percentage vs previous period |
| `UniqueEntities` | int | Unique customers/clients |
| `CompletionRate` | decimal | Success rate percentage |

**40+ properties** covering transactions, revenue, volumes, entities, and comparisons.

### BaseRealTimeStatsDto

Live operational metrics:

| Property | Type | Description |
|----------|------|-------------|
| `ActiveOperations` | int | Current active operations |
| `OperationsLast5Min` | int | Activity in last 5 minutes |
| `CurrentHourRevenue` | decimal | Revenue this hour |
| `ItemsInQueue` | int | Pending items |
| `AverageProcessingTime` | decimal | Processing time in minutes |
| `CapacityUtilizationRate` | decimal | Resource usage percentage |

**25+ properties** for real-time monitoring and capacity planning.

### BaseAggregatedStatsDto

Time-series aggregations with statistical analysis:

| Property | Type | Description |
|----------|------|-------------|
| `Period` | DateTime | Time period for this data point |
| `Value` | decimal | Primary metric value |
| `MinValue` / `MaxValue` / `AvgValue` | decimal | Statistical aggregates |
| `PreviousPeriodValue` | decimal? | Comparison value |
| `ChangePercent` | decimal? | Period-over-period change |
| `Percentile25` / `50` / `75` | decimal? | Distribution metrics |

**30+ properties** for comprehensive statistical analysis.

### BaseTrendDto

Advanced trend analysis with forecasting:

| Property | Type | Description |
|----------|------|-------------|
| `Date` | DateTime | Data point timestamp |
| `Value` | decimal | Actual observed value |
| `Forecast` | decimal? | Predicted value |
| `MovingAverage7` / `30` / `90` | decimal? | Trend smoothing |
| `SamePeriodLastYear` | decimal? | Year-over-year comparison |
| `IsAnomaly` | bool | Outlier detection |

**35+ properties** for deep trend analysis and forecasting.

## 🛠️ SQL Templates

Use `StatsSqlTemplates` for consistent SQL patterns:

```csharp
using Acontplus.Analytics.Models;

// Date range parameters
StatsSqlTemplates.DateRangeParams
StatsSqlTemplates.DateRangeDefaults
StatsSqlTemplates.PreviousPeriodCalc

// Aggregation helpers
StatsSqlTemplates.GroupByCase       // Dynamic time grouping
StatsSqlTemplates.PeriodLabelFormat // Human-readable labels

// Statistical functions
StatsSqlTemplates.MovingAverage7    // 7-period moving average
StatsSqlTemplates.MovingAverage30   // 30-period moving average
StatsSqlTemplates.PercentChange     // Percentage change calculation
StatsSqlTemplates.TrendDirection    // up/down/stable classification

// Special functions
StatsSqlTemplates.AnomalyDetection  // Outlier detection
StatsSqlTemplates.IsWeekend         // Weekend classification
```

## 🌍 Localization

All DTOs include an optional `Labels` dictionary property that your application can populate with localized strings:

```csharp
// In your application's localization service
public class SalesStatisticsLocalization
{
    public static Dictionary<string, string> GetSpanishLabels() => new()
    {
        { "TotalRevenue", "Ingresos Totales" },
        { "NetRevenue", "Ingresos Netos" },
        { "GrowthRate", "Tasa de Crecimiento" },
        { "AverageOrderValue", "Valor Promedio del Pedido" }
    };

    public static Dictionary<string, string> GetEnglishLabels() => new()
    {
        { "TotalRevenue", "Total Revenue" },
        { "NetRevenue", "Net Revenue" },
        { "GrowthRate", "Growth Rate" },
        { "AverageOrderValue", "Average Order Value" }
    };
}

// Populate labels in your service
var dashboard = await GetDashboardStatsAsync(filter);
if (dashboard.IsSuccess)
{
    dashboard.Value.Labels = language == "es"
        ? SalesStatisticsLocalization.GetSpanishLabels()
        : SalesStatisticsLocalization.GetEnglishLabels();
}
```

> **Note**: Localization is intentionally left to the consuming application, allowing you to integrate with your preferred localization system (RESX files, database, JSON, or any i18n framework).

## 🏗️ Architecture Patterns

### Repository Pattern

```csharp
public class SalesAnalyticsRepository : ISalesAnalyticsRepository
{
    public async Task<SalesDashboardDto> GetDashboardAsync(
        DateTime startDate,
        DateTime endDate)
    {
        // Use stored procedure with SQL templates
        var parameters = new[]
        {
            new SqlParameter("@StartDate", startDate),
            new SqlParameter("@EndDate", endDate)
        };

        return await _context.ExecuteStoredProcedureAsync<SalesDashboardDto>(
            "sp_GetSalesDashboardStats",
            parameters);
    }
}
```

### Clean Architecture Integration

```
Domain Layer:
  └── IStatisticsService<TDashboard, TRealTime, TAggregated, TTrend>

Application Layer:
  └── SalesStatisticsService : IStatisticsService<...>
      └── Uses domain repositories

Infrastructure Layer:
  └── SalesAnalyticsRepository
      └── Stored procedures with StatsSqlTemplates

API Layer:
  └── AnalyticsController
      └── Injects IStatisticsService
```

## 📚 Use Cases

### E-Commerce Analytics

```csharp
public class OrderDashboardDto : BaseDashboardStatsDto
{
    public decimal AverageOrderValue { get; set; }
    public int AbandonedCarts { get; set; }
    public decimal ConversionRate { get; set; }
}
```

### Healthcare Analytics

```csharp
public class PatientDashboardDto : BaseDashboardStatsDto
{
    public int TotalAppointments { get; set; }
    public int CompletedTreatments { get; set; }
    public decimal PatientSatisfactionScore { get; set; }
}
```

### Financial Analytics

```csharp
public class TransactionDashboardDto : BaseDashboardStatsDto
{
    public decimal ProcessingFees { get; set; }
    public int ChargebackCount { get; set; }
    public decimal ApprovalRate { get; set; }
}
```

## 🔗 Integration with Acontplus Libraries

Works seamlessly with other Acontplus packages:

- **Acontplus.Core**: Uses `Result<T, TError>` pattern and `FilterRequest`
- **Acontplus.Utilities**: Leverages API conversion extensions
- **Acontplus.Persistence**: Compatible with repository patterns
- **Acontplus.Services**: Can be exposed via API controllers

## 📖 Best Practices

1. **Extend base DTOs** for domain-specific properties
2. **Use SQL templates** for consistent stored procedures
3. **Implement caching** for frequently accessed dashboard data
4. **Return Result<T>** for consistent error handling
5. **Add localization labels** for international applications
6. **Use aggregations** for large datasets instead of real-time calculations

## 🤝 Contributing

We welcome contributions! Please see [Contributing Guidelines](../../CONTRIBUTING.md).

## 📧 Support

- **Email**: proyectos@acontplus.com
- **Issues**: [GitHub Issues](https://github.com/acontplus/acontplus-dotnet-libs/issues)
- **Documentation**: [Wiki](https://github.com/acontplus/acontplus-dotnet-libs/wiki)

## 👨‍💻 Author

**Ivan Paz** – [@iferpaz7](https://linktr.ee/iferpaz7)

## 🏢 Company

**[Acontplus](https://www.acontplus.com)** – Software Solutions, Ecuador

---

**Built with ❤️ for data-driven business applications**
