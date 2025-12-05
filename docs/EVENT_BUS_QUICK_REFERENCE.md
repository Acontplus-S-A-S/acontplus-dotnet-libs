# Event Bus - Quick Reference

## 🚀 30-Second Setup

```csharp
// 1. Register in Program.cs
builder.Services.AddInMemoryEventBus();

// 2. Define an event
public record UserRegisteredEvent(int UserId, string Email);

// 3. Publish
await _eventPublisher.PublishAsync(new UserRegisteredEvent(123, "user@example.com"));

// 4. Subscribe (BackgroundService)
await foreach (var evt in _eventSubscriber.SubscribeAsync<UserRegisteredEvent>(stoppingToken))
{
    Console.WriteLine($"User {evt.UserId} registered!");
}
```

## 📦 Packages Required

- `Acontplus.Core` (v2.0.3+) - Abstractions
- `Acontplus.Infrastructure` (v1.2.1+) - Implementation

**No additional NuGet packages needed!** Uses built-in `System.Threading.Channels`.

## 🔧 Dependency Injection

```csharp
// Choose one:
public class MyService
{
    // For publishing only
    public MyService(IEventPublisher publisher) { }

    // For subscribing only (usually BackgroundServices)
    public MyService(IEventSubscriber subscriber) { }

    // For both
    public MyService(IEventBus eventBus) { }
}
```

## 📝 Common Patterns

### Pattern 1: Command → Event → Multiple Handlers

```csharp
// Command Handler
public async Task CreateOrderAsync(CreateOrderCommand cmd)
{
    var order = new Order(...);
    await _repository.SaveAsync(order);

    // Publish event
    await _eventPublisher.PublishAsync(new OrderCreatedEvent(order.Id));
}

// Event Handlers (all execute concurrently)
services.AddHostedService<SendEmailHandler>();        // Send confirmation
services.AddHostedService<UpdateAnalyticsHandler>();  // Track metrics
services.AddHostedService<NotifyWarehouseHandler>();  // Prepare shipping
```

### Pattern 2: Event Chain (Saga/Workflow)

```csharp
// Handler publishes next event
protected override async Task ExecuteAsync(CancellationToken ct)
{
    await foreach (var evt in _subscriber.SubscribeAsync<OrderCreatedEvent>(ct))
    {
        // Process order
        await ProcessOrderAsync(evt.OrderId);

        // Publish next stage event
        await _publisher.PublishAsync(new OrderProcessedEvent(evt.OrderId));
    }
}
```

### Pattern 3: Multiple Event Types in One Handler

```csharp
protected override async Task ExecuteAsync(CancellationToken ct)
{
    var task1 = ListenToOrderCreated(ct);
    var task2 = ListenToOrderCancelled(ct);

    await Task.WhenAll(task1, task2);
}

private async Task ListenToOrderCreated(CancellationToken ct)
{
    await foreach (var evt in _subscriber.SubscribeAsync<OrderCreatedEvent>(ct))
        await HandleAsync(evt);
}
```

## ⚡ Performance Tips

1. **Use unbounded channels** (default) - no blocking
2. **Avoid `Task.Run`** in handlers - already async
3. **Keep events small** - < 1 KB for best performance
4. **Use record types** - immutable and efficient
5. **Enable logging selectively** - disable in production for max speed

## 🐛 Troubleshooting

| Problem | Solution |
|---------|----------|
| Events not received | Ensure handler registered: `AddHostedService<T>()` |
| Memory leak | Handlers must honor `CancellationToken` |
| Slow processing | Check handler logic, not event bus |
| Multiple events | Expected! Each subscriber gets all events |
| Events lost on restart | In-memory only, use distributed for persistence |

## 📊 When to Use

### ✅ Use Event Bus For

- Cross-cutting concerns (notifications, logging, analytics)
- Async workflows (order processing, data pipelines)
- Decoupling components (microservices communication)
- CQRS event sourcing
- Background task orchestration

### ❌ Use MediatR Instead For

- Request-response patterns
- Single handler per message
- Pipeline behaviors (validation, logging)
- In-process command/query handling with return values

## 🔄 Upgrade Path

```csharp
// Current: In-Memory (single instance)
services.AddInMemoryEventBus();

// Future: Distributed (multi-instance)
services.AddAzureServiceBusEventBus();  // Same interface!
services.AddRabbitMqEventBus();         // Same interface!
```

## 📚 Full Documentation

- **Complete Guide**: `docs/EVENT_BUS_GUIDE.md`
- **Implementation Details**: `docs/EVENT_BUS_IMPLEMENTATION_SUMMARY.md`
- **Live Example**: `apps/src/Acontplus.TestApi/Features/Orders/`
- **HTTP Tests**: `apps/src/Acontplus.TestApi/Orders.http`

---

**Questions?** Check the full guide or TestApi example!
