using Acontplus.Analytics.Dtos;
using Acontplus.Analytics.Services;
using Acontplus.Core.Abstractions.Persistence;
using Acontplus.TestApplication.Dtos.Analytics;
using Acontplus.TestApplication.Interfaces;

namespace Acontplus.TestApplication.Services;

/// <summary>
/// Sales analytics service implementation using the generic StatisticsService
/// Demonstrates clean architecture integration with Acontplus.Analytics
/// </summary>
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
