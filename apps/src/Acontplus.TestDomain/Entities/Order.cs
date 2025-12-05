namespace Acontplus.TestDomain.Entities;

/// <summary>
/// Order aggregate root - Domain entity following DDD patterns
/// </summary>
public class Order : BaseEntity
{
    public required string CustomerName { get; set; }
    public required string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Created;
    public new DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime? ShippedAt { get; set; }
    public string? TrackingNumber { get; set; }
}

/// <summary>
/// Order status enumeration
/// </summary>
public enum OrderStatus
{
    Created = 0,
    Processing = 1,
    Processed = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5
}
