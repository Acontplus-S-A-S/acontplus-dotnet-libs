using Acontplus.Analytics.Dtos;

namespace Demo.Application.Dtos.Analytics;

/// <summary>
/// Sales-specific dashboard statistics extending base analytics DTO
/// </summary>
public class SalesDashboardDto : BaseDashboardStatsDto
{
    // Sales-specific metrics
    public decimal AverageOrderValue { get; set; }
    public decimal DiscountPercentage { get; set; }
    public int CancelledOrders { get; set; }
    public decimal CancellationRate { get; set; }

    // Payment metrics
    public decimal CashSales { get; set; }
    public decimal CreditCardSales { get; set; }
    public decimal TransferSales { get; set; }

    // Customer metrics
    public int NewCustomers { get; set; }
    public int ReturningCustomers { get; set; }
    public decimal CustomerRetentionRate { get; set; }
}
