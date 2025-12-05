# Event Systems Architecture Guide

## Overview

Acontplus provides **TWO distinct event systems** for different purposes:

### 1. **Domain Event Dispatcher** (DDD Pattern)
- **Interface**: `IDomainEventDispatcher` + `IDomainEventHandler<T>`
- **Events**: Generic entity events (`EntityCreatedEvent`, `EntityModifiedEvent`, etc.)
- **Purpose**: Domain-Driven Design events **within bounded context**
- **Execution**: **Synchronous** within same transaction/unit of work
- **Use For**:
  - Domain invariant enforcement
  - Updating related aggregates
  - Audit logging (transactional)
  - Domain business rules

### 2. **Application Event Bus** (Microservices Pattern)
- **Interface**: `IEventPublisher` + `IEventSubscriber`
- **Events**: Custom application events (`OrderCreatedEvent`, `PaymentProcessedEvent`, etc.)
- **Purpose**: **Cross-service** communication and async workflows
- **Execution**: **Asynchronous** via background handlers (System.Threading.Channels)
- **Use For**:
  - Microservices communication
  - Notifications (email, SMS, push)
  - Analytics and reporting
  - Integration with external systems
  - Background processing

---

## When to Use Which?

| Scenario | Use Domain Event Dispatcher | Use Application Event Bus |
|----------|----------------------------|---------------------------|
| **Update related aggregate in same transaction** | ✅ Yes | ❌ No |
| **Send email notification** | ❌ No | ✅ Yes |
| **Enforce domain invariant** | ✅ Yes | ❌ No |
| **Publish to external system** | ❌ No | ✅ Yes |
| **Audit trail (transactional)** | ✅ Yes | ❌ No |
| **Analytics/metrics** | ❌ No | ✅ Yes |
| **Workflow automation** | ❌ No | ✅ Yes |
| **Cross-bounded-context communication** | ❌ No | ✅ Yes |

---

## Example Usage

### Domain Event (DDD)

```csharp
// Domain Layer - Entity
public class Order : BaseEntity
{
    public void MarkAsProcessed()
    {
        Status = OrderStatus.Processed;
        ProcessedAt = DateTime.UtcNow;

        // Domain event will be dispatched by infrastructure
        // via IDomainEventDispatcher (synchronously)
    }
}

// Infrastructure Layer - Domain Event Handler
public class EntityAuditHandler : IDomainEventHandler<EntityCreatedEvent>
{
    public Task HandleAsync(EntityCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        // Runs SYNCHRONOUSLY in same transaction
        // Write to audit table
        await _auditRepo.AddAsync(new AuditEntry
        {
            EntityType = domainEvent.EntityType,
            EntityId = domainEvent.EntityId,
            Action = "Created",
            Timestamp = domainEvent.OccurredOn
        }, cancellationToken);

        return Task.CompletedTask;
    }
}
```

### Application Event (Event Bus)

```csharp
// Application Layer - Service
public class OrderService
{
    public async Task<Result<OrderCreatedResult>> CreateOrderAsync(CreateOrderCommand command)
    {
        // 1. Persist entity
        var order = await _repository.AddAsync(new Order { ... });

        // 2. Dispatch domain event (synchronous, same transaction)
        await _domainEventDispatcher.Dispatch(
            new EntityCreatedEvent(order.Id, nameof(Order), null));

        // 3. Publish application event (asynchronous, background)
        await _eventPublisher.PublishAsync(
            new OrderCreatedEvent(order.Id, order.CustomerName, order.TotalAmount));

        return Result.Success(new OrderCreatedResult(order.Id));
    }
}

// Infrastructure Layer - Application Event Handler
public class OrderNotificationHandler : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Runs ASYNCHRONOUSLY in background
        await _eventSubscriber.SubscribeAsync<OrderCreatedEvent>(async (evt, ct) =>
        {
            // Send email notification (non-transactional)
            await _emailService.SendAsync(
                evt.CustomerName,
                "Order Confirmation",
                $"Your order #{evt.OrderId} has been created!");
        }, stoppingToken);
    }
}
```

---

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                        API Layer (Presentation)                  │
│                         OrderEndpoints.cs                        │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                    Application Layer                             │
│                      OrderService.cs                             │
│                                                                  │
│  ┌──────────────────────┐      ┌──────────────────────────┐   │
│  │ IDomainEventDispatcher│      │    IEventPublisher       │   │
│  │   (Synchronous)       │      │    (Asynchronous)        │   │
│  └──────────┬───────────┘      └──────────┬───────────────┘   │
└─────────────┼──────────────────────────────┼───────────────────┘
              │                              │
              │                              │
    ┌─────────▼─────────┐          ┌─────────▼──────────┐
    │  Domain Events    │          │  Application Events │
    │                   │          │                     │
    │ EntityCreatedEvent│          │ OrderCreatedEvent   │
    │ EntityModifiedEvent          │ OrderProcessedEvent │
    │ EntityDeletedEvent│          │ OrderShippedEvent   │
    └─────────┬─────────┘          └─────────┬───────────┘
              │                              │
              │                              │
┌─────────────▼──────────────┐  ┌───────────▼────────────────────┐
│ Domain Event Handlers      │  │ Background Event Handlers       │
│ (Same Transaction)         │  │ (Async BackgroundService)       │
│                            │  │                                 │
│ EntityAuditHandler         │  │ OrderNotificationHandler        │
│ - Audit logging            │  │ - Send emails                   │
│ - Enforce invariants       │  │ - Log analytics                 │
│ - Update related aggregates│  │ - Call external APIs            │
└────────────────────────────┘  └─────────────────────────────────┘
```

---

## Key Differences

### Domain Event Dispatcher
- ✅ **Transaction safety**: Runs in same DB transaction
- ✅ **Consistency**: Strong consistency guarantees
- ✅ **Failure handling**: Rolls back with main operation
- ❌ **Performance**: Can slow down primary operation
- ❌ **Scalability**: Limited to single process

### Application Event Bus
- ✅ **Performance**: Non-blocking, async execution
- ✅ **Scalability**: Distributable across processes
- ✅ **Decoupling**: Loose coupling between services
- ❌ **Consistency**: Eventually consistent
- ❌ **Failure handling**: Requires retry/compensation logic

---

## Best Practices

1. **Use Domain Events for**:
   - Maintaining domain invariants
   - Ensuring data consistency
   - Transactional operations

2. **Use Application Events for**:
   - I/O operations (emails, HTTP calls)
   - Cross-service communication
   - Non-critical workflows

3. **Publish Both**:
   ```csharp
   // Domain event first (synchronous, transactional)
   await _domainEventDispatcher.Dispatch(new EntityCreatedEvent(...));

   // Application event second (asynchronous, fire-and-forget)
   await _eventPublisher.PublishAsync(new OrderCreatedEvent(...));
   ```

4. **Error Handling**:
   - Domain events: Let exceptions roll back transaction
   - Application events: Implement retry logic in handlers

---

## Configuration

### Register Domain Event Dispatcher
```csharp
services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
services.AddScoped<IDomainEventHandler<EntityCreatedEvent>, EntityAuditHandler>();
```

### Register Application Event Bus
```csharp
services.AddEventBus(options =>
{
    options.ChannelCapacity = 1000;
    options.MaxConcurrentHandlers = 10;
});

services.AddHostedService<OrderNotificationHandler>();
services.AddHostedService<OrderAnalyticsHandler>();
```

---

## References

- **Domain Events**: [Microsoft DDD Patterns](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-events-design-implementation)
- **Event Bus**: See `docs/EVENT_BUS_GUIDE.md`
- **CQRS**: See `docs/EVENT_BUS_CLEAN_ARCHITECTURE.md`
