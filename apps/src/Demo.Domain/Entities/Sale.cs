namespace Demo.Domain.Entities;

/// <summary>
/// Sale entity for analytics demonstration
/// </summary>
public class Sale : BaseEntity
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal NetAmount => TotalAmount - DiscountAmount;
    public string Status { get; set; } = "Pending"; // Pending, Completed, Cancelled
    public string PaymentMethod { get; set; } = string.Empty;
    public int ItemCount { get; set; }
}
