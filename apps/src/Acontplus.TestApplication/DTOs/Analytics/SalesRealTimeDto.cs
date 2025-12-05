using Acontplus.Analytics.Dtos;

namespace Acontplus.TestApplication.Dtos.Analytics;

/// <summary>
/// Real-time sales statistics
/// </summary>
public class SalesRealTimeDto : BaseRealTimeStatsDto
{
    // Current activity
    public int ActiveSales { get; set; }
    public int PendingPayments { get; set; }
    public decimal CurrentDayRevenue { get; set; }

    // Performance
    public decimal AverageSaleTime { get; set; }
    public int SalesPerHour { get; set; }
    public decimal PeakHourRevenue { get; set; }
}
