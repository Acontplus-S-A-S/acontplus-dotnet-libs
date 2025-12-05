namespace Acontplus.TestDomain.Entities;

/// <summary>
/// Order aggregate root - Domain entity following DDD patterns
/// </summary>
public class Order : BaseEntity
{
    public required string CustomerName { get; set; }
    public required string ProductName { get; set; }
    
    private int _quantity;
    private decimal _price;
    
    public int Quantity 
    { 
        get => _quantity;
        set => _quantity = value > 0 ? value : throw new ArgumentException("Quantity must be positive", nameof(value));
    }
    
    public decimal Price 
    { 
        get => _price;
        set => _price = value >= 0 ? value : throw new ArgumentException("Price cannot be negative", nameof(value));
    }

    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Created;
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
