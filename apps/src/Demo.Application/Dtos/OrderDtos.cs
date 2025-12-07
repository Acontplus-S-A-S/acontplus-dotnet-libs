namespace Demo.Application.Dtos;

/// <summary>
/// Command to create a new order (CQRS Write Model)
/// Demonstrates both Domain Events and Application Events:
/// - Domain Event: EntityCreatedEvent dispatched to create OrderLineItems (transactional)
/// - Application Event: OrderCreatedEvent published for notifications (async)
/// </summary>
public record CreateOrderCommand(
    string CustomerName,
    string ProductName,
    int Quantity,
    decimal Price);

/// <summary>
/// Line item for order creation
/// </summary>
public record OrderLineItemDto(
    string ProductName,
    int Quantity,
    decimal UnitPrice);

/// <summary>
/// Query to get order by ID (CQRS Read Model)
/// </summary>
public record GetOrderQuery(int OrderId);

/// <summary>
/// Result of order creation
/// </summary>
public record OrderCreatedResult(
    int OrderId,
    DateTime CreatedAt,
    decimal TotalAmount);

/// <summary>
/// Order data transfer object for API responses
/// </summary>
public record OrderDto(
    int OrderId,
    string CustomerName,
    string ProductName,
    int Quantity,
    decimal Price,
    decimal TotalAmount,
    DateTime CreatedAt,
    string Status);
