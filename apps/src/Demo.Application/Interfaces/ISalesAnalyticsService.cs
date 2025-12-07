using Acontplus.Analytics.Dtos;
using Acontplus.Analytics.Interfaces;
using Demo.Application.Dtos.Analytics;

namespace Demo.Application.Interfaces;

/// <summary>
/// Sales analytics service interface - demonstrates clean architecture with Analytics package
/// </summary>
public interface ISalesAnalyticsService : IStatisticsService<
    SalesDashboardDto,
    SalesRealTimeDto,
    BaseAggregatedStatsDto,
    BaseTrendDto>
{
    // Can add domain-specific methods here if needed
    // For now, inheriting all methods from IStatisticsService is sufficient
}
