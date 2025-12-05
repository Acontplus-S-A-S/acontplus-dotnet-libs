namespace Acontplus.Analytics.Dtos;

/// <summary>
/// Base dashboard statistics DTO - Common metrics applicable across all business domains
/// </summary>
public class BaseDashboardStatsDto
{
    // === TRANSACTION METRICS (Universal) ===
    /// <summary>Total number of transactions/orders/operations in period</summary>
    public int TotalTransactions { get; set; }

    /// <summary>Successfully completed transactions</summary>
    public int CompletedTransactions { get; set; }

    /// <summary>Cancelled/rejected transactions</summary>
    public int CancelledTransactions { get; set; }

    /// <summary>Active/pending transactions</summary>
    public int ActiveTransactions { get; set; }

    /// <summary>Completion rate as percentage (0-100)</summary>
    public decimal CompletionRate { get; set; }

    /// <summary>Average value per transaction</summary>
    public decimal AverageTransactionValue { get; set; }

    // === REVENUE METRICS (Universal) ===
    /// <summary>Total gross revenue/sales</summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>Net revenue after deductions</summary>
    public decimal NetRevenue { get; set; }

    /// <summary>Total tax amount</summary>
    public decimal TotalTax { get; set; }

    /// <summary>Total discounts given</summary>
    public decimal TotalDiscounts { get; set; }

    /// <summary>Revenue growth rate compared to previous period (%)</summary>
    public decimal GrowthRate { get; set; }

    /// <summary>Revenue per customer/client</summary>
    public decimal RevenuePerEntity { get; set; }

    // === VOLUME METRICS (Universal) ===
    /// <summary>Total items/products/services processed</summary>
    public int TotalItemsProcessed { get; set; }

    /// <summary>Average items per transaction</summary>
    public decimal AverageItemsPerTransaction { get; set; }

    /// <summary>Top performing item/product/service name</summary>
    public string? TopItem { get; set; }

    /// <summary>Quantity of top performing item</summary>
    public int TopItemQuantity { get; set; }

    /// <summary>Top revenue generating item</summary>
    public string? TopRevenueItem { get; set; }

    /// <summary>Revenue from top item</summary>
    public decimal TopRevenueItemTotal { get; set; }

    // === ENTITY METRICS (Customers/Clients/Suppliers) ===
    /// <summary>Unique entities served/processed</summary>
    public int UniqueEntities { get; set; }

    /// <summary>New entities in this period</summary>
    public int NewEntities { get; set; }

    /// <summary>Returning/repeat entities</summary>
    public int ReturningEntities { get; set; }

    /// <summary>Entity retention rate (%)</summary>
    public decimal EntityRetentionRate { get; set; }

    // === PERIOD COMPARISON METRICS ===
    /// <summary>Revenue change vs previous period (%)</summary>
    public decimal RevenueChangePercent { get; set; }

    /// <summary>Transaction count change vs previous period (%)</summary>
    public decimal TransactionChangePercent { get; set; }

    /// <summary>Entity count change vs previous period (%)</summary>
    public decimal EntityChangePercent { get; set; }

    // === FINANCIAL BREAKDOWN ===
    /// <summary>Cash payments total</summary>
    public decimal CashPayments { get; set; }

    /// <summary>Card/electronic payments total</summary>
    public decimal CardPayments { get; set; }

    /// <summary>Other payment methods total</summary>
    public decimal OtherPayments { get; set; }

    /// <summary>Average discount percentage applied</summary>
    public decimal AverageDiscountPercent { get; set; }

    /// <summary>Localization metadata for chart labels and UI display (e.g., Spanish, English)</summary>
    public Dictionary<string, string>? Labels { get; set; }
}
