namespace Demo.Domain.Events;

/// <summary>
/// APPLICATION-LEVEL EVENTS for cross-service communication via Event Bus
/// These events are published to the application event bus for:
/// - Microservices communication
/// - Background processing
/// - Cross-cutting concerns (notifications, analytics, auditing)
/// - Integration with external systems
/// </summary>

/// <summary>
/// Event published to application event bus when an order is created.
/// Used for notifications, analytics, and workflow automation.
/// </summary>
public record OrderCreatedEvent(
    int OrderId,
    string CustomerName,
    string ProductName,
    decimal TotalAmount,
    DateTime CreatedAt);

/// <summary>
/// Event published to application event bus when an order is processed.
/// Triggers shipping workflow and customer notifications.
/// </summary>
public record OrderProcessedEvent(
    int OrderId,
    DateTime ProcessedAt,
    string ProcessedBy);

/// <summary>
/// Event published to application event bus when an order is shipped.
/// Sends tracking notifications to customers.
/// </summary>
public record OrderShippedEvent(
    int OrderId,
    DateTime ShippedAt,
    string TrackingNumber);

// NOTE: Domain events (EntityCreatedEvent, EntityModifiedEvent, etc.)
// are handled via IDomainEventDispatcher within the domain layer.
// These application events are published via IEventPublisher for cross-cutting concerns.
