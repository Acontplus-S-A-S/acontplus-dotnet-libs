namespace Demo.Domain.Entities;

/// <summary>
/// Order line item entity - child of Order aggregate.
/// Demonstrates domain event dispatcher usage where line items are created
/// automatically when an Order is created (using Order.Id from parent).
/// </summary>
public class OrderLineItem : BaseEntity
{
    public int OrderId { get; set; }
    public required string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}
