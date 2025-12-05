# Event Bus - In-Memory Channel-Based Architecture

## Overview

The **Acontplus Event Bus** provides a high-performance, scalable in-memory event-driven architecture using `System.Threading.Channels`. It's designed for **Clean Architecture + DDD + CQRS** patterns with support for horizontal and vertical scaling under high workloads.

## Architecture

### 📦 Package Structure

```
Acontplus.Core (Abstractions)
├── IEventPublisher          - Publish events
├── IEventSubscriber         - Subscribe to events
└── IEventBus                - Combined interface

Acontplus.Infrastructure (Implementation)
├── InMemoryEventBus         - Channel-based implementation
├── ChannelExtensions        - Type-safe channel transformations
├── EventBusOptions          - Configuration options
└── EventBusExtensions       - DI registration
```

### 🏗️ Design Principles

- **Clean Architecture**: Abstractions in Core, implementations in Infrastructure
- **Domain-Driven Design**: Event-driven communication between bounded contexts
- **CQRS**: Separation of commands (write) and queries (read)
- **Event Sourcing**: Publish domain events for async processing
- **High Performance**: Unbounded channels with multi-producer/multi-consumer support
- **Thread-Safe**: Concurrent dictionary with lock-free event publishing
- **Scalable**: Designed for horizontal and vertical scaling

## Installation

The event bus is included in:
- `Acontplus.Core` (v2.0.3+) - Abstractions
- `Acontplus.Infrastructure` (v1.2.1+) - Implementation

```bash
dotnet add package Acontplus.Core
dotnet add package Acontplus.Infrastructure
```

## Quick Start

### 1. Register Event Bus

```csharp
// Program.cs or ServiceCollectionExtensions.cs
services.AddInMemoryEventBus(options =>
{
    options.EnableDiagnosticLogging = true;
});
```

### 2. Define Events

```csharp
// Events are simple POCOs (record types recommended)
public record OrderCreatedEvent(
    Guid OrderId,
    string CustomerName,
    decimal TotalAmount,
    DateTime CreatedAt);
```

### 3. Publish Events

```csharp
public class OrderService
{
    private readonly IEventPublisher _eventPublisher;

    public OrderService(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task CreateOrderAsync(CreateOrderCommand command)
    {
        // ... create order logic ...

        // Publish event
        await _eventPublisher.PublishAsync(new OrderCreatedEvent(
            orderId,
            command.CustomerName,
            totalAmount,
            DateTime.UtcNow));
    }
}
```

### 4. Subscribe to Events

```csharp
public class OrderNotificationHandler : BackgroundService
{
    private readonly IEventSubscriber _eventSubscriber;
    private readonly ILogger<OrderNotificationHandler> _logger;

    public OrderNotificationHandler(
        IEventSubscriber eventSubscriber,
        ILogger<OrderNotificationHandler> logger)
    {
        _eventSubscriber = eventSubscriber;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var orderEvent in _eventSubscriber
            .SubscribeAsync<OrderCreatedEvent>(stoppingToken))
        {
            _logger.LogInformation(
                "Sending email for Order {OrderId}",
                orderEvent.OrderId);

            // Process event...
            await SendEmailAsync(orderEvent);
        }
    }
}
```

### 5. Register Event Handlers

```csharp
// Register as hosted services
services.AddHostedService<OrderNotificationHandler>();
services.AddHostedService<OrderAnalyticsHandler>();
```

## Complete CQRS Example

See `Acontplus.TestApi/Features/Orders/` for a comprehensive example:

### Structure

```
Features/Orders/
├── Commands/
│   └── CreateOrderCommand.cs       - Write operations
├── Queries/
│   └── GetOrderQuery.cs            - Read operations
├── Events/
│   └── OrderEvents.cs              - Domain events
├── Handlers/
│   ├── OrderNotificationHandler.cs - Email notifications
│   ├── OrderAnalyticsHandler.cs    - Analytics tracking
│   └── OrderWorkflowHandler.cs     - Workflow automation
├── Services/
│   └── OrderService.cs             - Command/Query handlers
└── Endpoints/
    └── OrderEndpoints.cs           - API endpoints
```

### Event Flow

```
1. Client → POST /api/orders (CreateOrderCommand)
2. OrderService.CreateOrderAsync()
   ├── Create order (write model)
   └── Publish OrderCreatedEvent
3. Event Bus distributes to multiple subscribers:
   ├── OrderNotificationHandler → Send email
   ├── OrderAnalyticsHandler → Record analytics
   └── OrderWorkflowHandler → Auto-process order
       ├── Publish OrderProcessedEvent
       └── Publish OrderShippedEvent
4. Return OrderCreatedResult to client
```

## Configuration Options

```csharp
services.AddInMemoryEventBus(options =>
{
    // Enable detailed logging for diagnostics
    options.EnableDiagnosticLogging = true;

    // Limit concurrent handlers (0 = unlimited)
    options.MaxConcurrentHandlers = 10;

    // Dispose on application shutdown
    options.DisposeOnShutdown = true;
});
```

## Performance Characteristics

### Channel Configuration

```csharp
// Unbounded channels optimized for high throughput
Channel.CreateUnbounded<object>(new UnboundedChannelOptions
{
    SingleWriter = false,                     // Multiple publishers
    SingleReader = false,                     // Multiple subscribers
    AllowSynchronousContinuations = false     // Prevent deadlocks
});
```

### Benchmarks (Estimated)

| Operation | Throughput | Latency |
|-----------|-----------|---------|
| Publish Event | ~1M ops/sec | <1μs |
| Subscribe & Process | ~500K ops/sec | <10μs |
| Concurrent Publishers (8 threads) | ~5M ops/sec | <5μs |

## Scaling Strategies

### Horizontal Scaling

For distributed systems, replace `InMemoryEventBus` with:
- **Azure Service Bus**: `services.AddAzureServiceBusEventBus()`
- **RabbitMQ**: `services.AddRabbitMqEventBus()`
- **Kafka**: `services.AddKafkaEventBus()`

Interface (`IEventPublisher`, `IEventSubscriber`) remains the same!

### Vertical Scaling

- Event handlers run as `BackgroundService` instances
- Increase parallelism with multiple handler instances
- Use `MaxConcurrentHandlers` to throttle processing

## Best Practices

### ✅ Do

- Use **record types** for events (immutable, value equality)
- Keep events **small and focused** (single responsibility)
- Make events **JSON-serializable** (for future distributed support)
- Use **cancellation tokens** for graceful shutdown
- Register handlers as **scoped or transient** for DI injection
- Log important events for **observability**

### ❌ Don't

- Throw exceptions in event handlers (use try-catch)
- Perform long-running blocking operations (use async)
- Share mutable state between handlers
- Publish events from constructors or finalizers
- Use events for request-response patterns (use MediatR instead)

## Testing

### Unit Testing

```csharp
[Fact]
public async Task CreateOrder_PublishesOrderCreatedEvent()
{
    // Arrange
    var eventBus = new InMemoryEventBus(logger);
    var service = new OrderService(eventBus, logger);
    var events = new List<OrderCreatedEvent>();

    // Start subscriber
    var cts = new CancellationTokenSource();
    _ = Task.Run(async () =>
    {
        await foreach (var evt in eventBus.SubscribeAsync<OrderCreatedEvent>(cts.Token))
        {
            events.Add(evt);
            cts.Cancel(); // Stop after first event
        }
    });

    // Act
    await service.CreateOrderAsync(new CreateOrderCommand(...));
    await Task.Delay(100); // Allow event processing

    // Assert
    Assert.Single(events);
    Assert.Equal("John Doe", events[0].CustomerName);
}
```

## Live Demo

Run the TestApi and use `Orders.http` to test:

```bash
cd apps/src/Acontplus.TestApi
dotnet run
```

Open `Orders.http` in VS Code and execute requests. Watch console logs for real-time event processing!

## Troubleshooting

### Events not received

- Ensure handlers are registered as `HostedService`
- Check cancellation token is not cancelled
- Enable diagnostic logging

### Memory leaks

- Channels are disposed on application shutdown
- Call `eventBus.Dispose()` explicitly if needed
- Ensure subscribers complete when cancellation is requested

### Performance issues

- Use unbounded channels (default)
- Avoid blocking operations in handlers
- Consider batching events
- Profile with `dotnet-trace` or Application Insights

## Related Patterns

- **Domain Events**: Use `IDomainEvent` in `Acontplus.Core.Domain.Common.Events`
- **MediatR**: Use for request-response (CQRS queries)
- **Outbox Pattern**: Persist events before publishing (transactional safety)
- **Saga Pattern**: Orchestrate multi-step workflows

## Migration Path

### From Domain Events

```csharp
// Before (domain events)
public class Order : Entity
{
    public void Create()
    {
        AddDomainEvent(new OrderCreatedDomainEvent(Id));
    }
}

// After (application events)
public class OrderService
{
    public async Task CreateOrderAsync()
    {
        var order = new Order();
        await _eventPublisher.PublishAsync(new OrderCreatedEvent(order.Id));
    }
}
```

## Support & Contribution

- **Issues**: [GitHub Issues](https://github.com/acontplus/acontplus-dotnet-libs/issues)
- **Documentation**: [Wiki](https://github.com/acontplus/acontplus-dotnet-libs/wiki)
- **License**: MIT

## Version History

- **1.0.0** (2025-01-XX): Initial release with `InMemoryEventBus`
- Future: Azure Service Bus, RabbitMQ, Kafka implementations

---

**Built with ❤️ using .NET 10 and modern C# features**
