# Acontplus.Analytics - Clean Architecture Implementation

Complete example of implementing analytics across all layers of a Clean Architecture application.

## Architecture Overview

```
├── API Layer (Acontplus.TestApi)
│   └── Endpoints/Business/Analytics/SalesAnalyticsEndpoints.cs
│       → Maps HTTP requests to service calls
│       → Applies localization at presentation layer
│
├── Application Layer (Acontplus.TestApplication)
│   ├── Interfaces/ISalesAnalyticsService.cs
│   │   → Domain-specific analytics contract
│   ├── Services/SalesAnalyticsService.cs
│   │   → Inherits from StatisticsService<T1,T2,T3,T4>
│   │   → Configures stored procedure names
│   ├── Dtos/Analytics/
│   │   ├── SalesDashboardDto.cs (extends BaseDashboardStatsDto)
│   │   └── SalesRealTimeDto.cs (extends BaseRealTimeStatsDto)
│   └── Helpers/SalesAnalyticsLocalization.cs
│       → Application-specific label provider (Spanish/English)
│
├── Domain Layer (Acontplus.TestDomain)
│   └── Entities/Sale.cs
│       → Business entity with analytics-relevant properties
│
└── Infrastructure Layer (Database)
    └── StoredProcedures/SalesAnalytics.sql
        → SQL procedures implementing analytics logic
```

## Implementation Files

### 1. Domain Layer - Sale Entity

**File**: `apps/src/Acontplus.TestDomain/Entities/Sale.cs`

```csharp
public class Sale : BaseEntity
{
    public int CustomerId { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public string Status { get; set; } // Pending, Completed, Cancelled
    public string PaymentMethod { get; set; }
    public int ItemCount { get; set; }
}
```

### 2. Application Layer - DTOs

**File**: `apps/src/Acontplus.TestApplication/Dtos/Analytics/SalesDashboardDto.cs`

```csharp
public class SalesDashboardDto : BaseDashboardStatsDto
{
    // Extends base DTO with sales-specific properties
    public decimal AverageOrderValue { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal CancellationRate { get; set; }
    public decimal CashSales { get; set; }
    public decimal CreditCardSales { get; set; }
    public int NewCustomers { get; set; }
    public decimal CustomerRetentionRate { get; set; }
}
```

**File**: `apps/src/Acontplus.TestApplication/Dtos/Analytics/SalesRealTimeDto.cs`

```csharp
public class SalesRealTimeDto : BaseRealTimeStatsDto
{
    public int ActiveSales { get; set; }
    public int PendingPayments { get; set; }
    public decimal CurrentDayRevenue { get; set; }
    public int SalesPerHour { get; set; }
}
```

### 3. Application Layer - Service Interface

**File**: `apps/src/Acontplus.TestApplication/Interfaces/ISalesAnalyticsService.cs`

```csharp
public interface ISalesAnalyticsService : IStatisticsService<
    SalesDashboardDto,
    SalesRealTimeDto,
    BaseAggregatedStatsDto,
    BaseTrendDto>
{
    // Inherits all methods from IStatisticsService
    // Can add domain-specific methods if needed
}
```

### 4. Application Layer - Service Implementation

**File**: `apps/src/Acontplus.TestApplication/Services/SalesAnalyticsService.cs`

```csharp
public class SalesAnalyticsService : StatisticsService<
    SalesDashboardDto,
    SalesRealTimeDto,
    BaseAggregatedStatsDto,
    BaseTrendDto>,
    ISalesAnalyticsService
{
    public SalesAnalyticsService(IAdoRepository adoRepository)
        : base(
            adoRepository,
            dashboardSpName: "Sales.GetDashboardStats",
            realTimeSpName: "Sales.GetRealTimeStats",
            aggregatedSpName: "Sales.GetAggregatedStats",
            trendsSpName: "Sales.GetTrendStats")
    {
    }
}
```

### 5. Application Layer - Localization

**File**: `apps/src/Acontplus.TestApplication/Helpers/SalesAnalyticsLocalization.cs`

```csharp
public static class SalesAnalyticsLocalization
{
    public static Dictionary<string, string> GetLabels(string language)
        => language.ToLowerInvariant() switch
        {
            "es" => GetSpanishLabels(),
            "en" => GetEnglishLabels(),
            _ => GetEnglishLabels()
        };

    private static Dictionary<string, string> GetSpanishLabels() => new()
    {
        { "TotalRevenue", "Ingresos Totales" },
        { "AverageOrderValue", "Valor Promedio de Orden" },
        { "CustomerRetentionRate", "Tasa de Retención" },
        // ... more labels
    };

    private static Dictionary<string, string> GetEnglishLabels() => new()
    {
        { "TotalRevenue", "Total Revenue" },
        { "AverageOrderValue", "Average Order Value" },
        { "CustomerRetentionRate", "Customer Retention Rate" },
        // ... more labels
    };
}
```

### 6. API Layer - Endpoints

**File**: `apps/src/Acontplus.TestApi/Endpoints/Business/Analytics/SalesAnalyticsEndpoints.cs`

```csharp
public static class SalesAnalyticsEndpoints
{
    public static IEndpointRouteBuilder MapSalesAnalyticsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/analytics/sales")
            .WithTags("Sales Analytics");

        // Dashboard endpoint
        group.MapGet("/dashboard", async (
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? language,
            [FromServices] ISalesAnalyticsService analyticsService,
            CancellationToken cancellationToken) =>
        {
            var filter = new FilterRequest
            {
                Filters = new Dictionary<string, object>
                {
                    { "StartDate", startDate ?? DateTime.UtcNow.AddDays(-30) },
                    { "EndDate", endDate ?? DateTime.UtcNow }
                }
            };

            var result = await analyticsService.GetDashboardStatsAsync(filter, cancellationToken);

            return result.Match(
                success: data =>
                {
                    // Apply localization at presentation layer
                    data.Labels = SalesAnalyticsLocalization.GetLabels(language ?? "en");
                    return Results.Ok(data);
                },
                failure: error => Results.BadRequest(new { error = error.Message, code = error.Code }));
        });

        // More endpoints: realtime, aggregated, trends...

        return app;
    }
}
```

### 7. API Layer - DI Registration

**File**: `apps/src/Acontplus.TestApi/Extensions/ProgramExtensions.cs`

```csharp
public static IServiceCollection AddBusinessServices(this IServiceCollection services)
{
    // ... other services

    // Analytics Services
    services.AddScoped<ISalesAnalyticsService, SalesAnalyticsService>();

    return services;
}

public static void MapTestApiEndpoints(this WebApplication app)
{
    // ... other endpoints

    // Map Analytics demo endpoints
    app.MapSalesAnalyticsEndpoints();
}
```

### 8. Infrastructure Layer - Stored Procedures

**File**: `apps/database/StoredProcedures/SalesAnalytics.sql`

```sql
CREATE OR ALTER PROCEDURE [Sales].[GetDashboardStats]
    @StartDate DATETIME2 = NULL,
    @EndDate DATETIME2 = NULL
AS
BEGIN
    SET @StartDate = ISNULL(@StartDate, DATEADD(DAY, -30, GETUTCDATE()));
    SET @EndDate = ISNULL(@EndDate, GETUTCDATE());

    SELECT
        -- Base properties
        COUNT(*) AS TotalTransactions,
        SUM(TotalAmount) AS TotalRevenue,
        AVG(TotalAmount) AS AverageTransactionValue,

        -- Sales-specific properties
        AVG(TotalAmount) AS AverageOrderValue,
        SUM(DiscountAmount) AS TotalDiscounts,
        SUM(CASE WHEN PaymentMethod = 'Cash' THEN TotalAmount ELSE 0 END) AS CashSales,
        COUNT(DISTINCT CustomerId) AS NewCustomers

    FROM Sales
    WHERE SaleDate BETWEEN @StartDate AND @EndDate;
END
```

## Usage Example (.http file)

**File**: `apps/src/Acontplus.TestApi/SalesAnalytics.http`

```http
@baseUrl = https://localhost:7042

### Get Dashboard (English)
GET {{baseUrl}}/api/analytics/sales/dashboard?language=en

### Get Dashboard (Spanish, Custom Range)
GET {{baseUrl}}/api/analytics/sales/dashboard
    ?startDate=2025-01-01
    &endDate=2025-12-31
    &language=es

### Get Real-Time Stats
GET {{baseUrl}}/api/analytics/sales/realtime?language=en

### Get Daily Aggregates (Last 30 Days)
GET {{baseUrl}}/api/analytics/sales/aggregated
    ?startDate={{$datetime iso8601 -30 d}}
    &endDate={{$datetime iso8601}}
    &groupBy=day
    &language=en

### Get Revenue Trends
GET {{baseUrl}}/api/analytics/sales/trends
    ?startDate={{$datetime iso8601 -90 d}}
    &endDate={{$datetime iso8601}}
    &metric=revenue
    &language=en
```

## Key Benefits

### ✅ Clean Separation of Concerns
- **Domain**: Defines business entities (Sale)
- **Application**: Orchestrates business logic, DTOs, and localization
- **Infrastructure**: Data access via stored procedures
- **API**: HTTP presentation layer with minimal logic

### ✅ Reusability
- `StatisticsService<T1,T2,T3,T4>` can be reused for any domain
- Just change SP names and DTO types
- Same pattern works for Products, Orders, Customers, etc.

### ✅ Type Safety
- Generic constraints ensure compile-time safety
- DTOs extend base classes for consistency
- Interface contracts prevent implementation drift

### ✅ Localization Flexibility
- Application-level concern, not library concern
- Supports any number of languages
- Can integrate with existing i18n systems (RESX, DB, JSON)

### ✅ Testability
- Services can be mocked via interfaces
- DTOs are simple POCOs
- Stored procedures can be tested independently

### ✅ Maintainability
- Changes to analytics logic stay in application layer
- Library updates don't break consuming code
- Clear responsibility boundaries

## Response Examples

### Dashboard Response
```json
{
  "totalTransactions": 1543,
  "totalRevenue": 245678.90,
  "averageOrderValue": 159.23,
  "discountPercentage": 9.04,
  "cashSales": 98765.43,
  "newCustomers": 234,
  "customerRetentionRate": 69.86,
  "labels": {
    "TotalRevenue": "Ingresos Totales",
    "AverageOrderValue": "Valor Promedio de Orden",
    "CustomerRetentionRate": "Tasa de Retención de Clientes"
  }
}
```

### Real-Time Response
```json
{
  "activeSales": 12,
  "pendingPayments": 8,
  "currentDayRevenue": 23456.78,
  "salesPerHour": 45,
  "labels": { "ActiveSales": "Ventas Activas", ... }
}
```

## Summary

This example demonstrates **complete clean architecture integration** of the Acontplus.Analytics package:

1. **Domain entities** define business data
2. **Application DTOs** extend base analytics classes
3. **Application services** configure dynamic stored procedures
4. **Application helpers** provide domain-specific localization
5. **API endpoints** expose analytics with minimal logic
6. **Stored procedures** implement efficient data aggregation

The pattern is **reusable, testable, maintainable, and scalable** across any business domain.
