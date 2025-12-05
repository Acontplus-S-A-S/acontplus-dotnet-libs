# CQRS + Event Bus - Clean Architecture Implementation

## ✅ Proper Layer Organization

The Order feature has been correctly organized following **Clean Architecture** principles across all layers:

```
📁 Clean Architecture Layers
├── 🎯 Domain Layer (Acontplus.TestDomain)
│   ├── Entities/
│   │   └── Order.cs                 - Aggregate root
│   └── Events/
│       └── OrderEvents.cs           - Domain events (OrderCreatedEvent, OrderProcessedEvent, OrderShippedEvent)
│
├── 📋 Application Layer (Acontplus.TestApplication)
│   ├── Dtos/
│   │   └── OrderDtos.cs             - Commands, Queries, Results, DTOs
│   ├── Interfaces/
│   │   └── IOrderService.cs         - Application service contract
│   └── Services/
│       └── OrderService.cs          - CQRS Command/Query handlers
│
├── 🏗️ Infrastructure Layer (Acontplus.TestInfrastructure)
│   └── EventHandlers/
│       ├── OrderNotificationHandler.cs   - Email notifications (BackgroundService)
│       ├── OrderAnalyticsHandler.cs      - Analytics tracking (BackgroundService)
│       └── OrderWorkflowHandler.cs       - Workflow automation (BackgroundService)
│
└── 🌐 Presentation Layer (Acontplus.TestApi)
    └── Endpoints/Business/
        └── OrderEndpoints.cs        - Minimal API endpoints
```

## 🎯 Layer Responsibilities

### Domain Layer (TestDomain)
**Pure business logic, no dependencies**

- **Entities** (`Order.cs`)
  - Aggregate roots and value objects
  - Business rules and invariants
  - Domain model

- **Events** (`OrderEvents.cs`)
  - Domain events (immutable records)
  - Represent business facts
  - Published when state changes occur

### Application Layer (TestApplication)
**Use cases and orchestration**

- **DTOs** (`OrderDtos.cs`)
  - `CreateOrderCommand` - Write model (CQRS Command)
  - `GetOrderQuery` - Read model (CQRS Query)
  - `OrderCreatedResult` - Command response
  - `OrderDto` - Query response

- **Interfaces** (`IOrderService.cs`)
  - Application service contracts
  - Decouples presentation from implementation

- **Services** (`OrderService.cs`)
  - CQRS Command handlers (CreateOrderAsync)
  - CQRS Query handlers (GetOrderByIdAsync, GetAllOrdersAsync)
  - Coordinates domain entities, repositories, and event publishing
  - Returns `Result<T>` for functional error handling

### Infrastructure Layer (TestInfrastructure)
**Cross-cutting concerns and external integrations**

- **Event Handlers** (BackgroundService implementations)
  - `OrderNotificationHandler` - Sends email notifications
  - `OrderAnalyticsHandler` - Records analytics/metrics
  - `OrderWorkflowHandler` - Orchestrates multi-step workflows
  - All run as background services
  - Subscribe to domain events via `IEventSubscriber`
  - React to events asynchronously

### Presentation Layer (TestApi)
**HTTP endpoints - thin layer**

- **Endpoints** (`OrderEndpoints.cs`)
  - Minimal API route definitions
  - Request/response mapping
  - Delegates to `IOrderService`
  - Returns appropriate HTTP status codes
  - Uses `Result<T>.Match()` for error handling

## 🔄 Request Flow

### Create Order (Command)

```
1. HTTP POST /api/orders
   ↓
2. OrderEndpoints.MapPost (Presentation)
   ↓
3. IOrderService.CreateOrderAsync (Application)
   ├── Create Order entity (Domain)
   ├── Save to repository (Infrastructure)
   └── Publish OrderCreatedEvent (via IEventPublisher)
   ↓
4. Event Bus distributes to subscribers:
   ├── OrderNotificationHandler → Send email
   ├── OrderAnalyticsHandler → Record analytics
   └── OrderWorkflowHandler → Auto-process order
       ├── Publish OrderProcessedEvent
       └── Publish OrderShippedEvent
   ↓
5. Return OrderCreatedResult
```

### Get Order (Query)

```
1. HTTP GET /api/orders/{id}
   ↓
2. OrderEndpoints.MapGet (Presentation)
   ↓
3. IOrderService.GetOrderByIdAsync (Application)
   ├── Load from repository
   └── Map to OrderDto
   ↓
4. Return OrderDto
```

## 📦 Dependency Direction

```
Presentation → Application → Domain
                ↓
           Infrastructure

✅ Domain has NO dependencies
✅ Application depends on Domain only
✅ Infrastructure depends on Domain & Application
✅ Presentation depends on Application (via interfaces)
```

## 🔧 Dependency Injection

### Application Layer Registration
```csharp
// TestApi/Extensions/ProgramExtensions.cs
services.AddScoped<IOrderService, OrderService>();
```

### Infrastructure Layer Registration
```csharp
// TestApi/Extensions/ProgramExtensions.cs

// Event Bus
services.AddInMemoryEventBus(options =>
{
    options.EnableDiagnosticLogging = true;
});

// Background Event Handlers
services.AddHostedService<OrderNotificationHandler>();
services.AddHostedService<OrderAnalyticsHandler>();
services.AddHostedService<OrderWorkflowHandler>();
```

## 🧪 Testing the Implementation

### Run the API
```bash
cd apps/src/Acontplus.TestApi
dotnet run
```

### Test with HTTP file
Use `Orders.http` to send requests and watch the console for event processing logs.

## 🎯 Key Benefits

### ✅ Separation of Concerns
- Each layer has a single responsibility
- Easy to test each layer independently
- Changes in one layer don't affect others

### ✅ Dependency Inversion
- High-level modules (Application) don't depend on low-level modules (Infrastructure)
- Both depend on abstractions (interfaces)
- Easy to swap implementations

### ✅ Event-Driven Architecture
- Loose coupling between components
- Multiple handlers can react to same event
- Easy to add new event handlers without modifying existing code

### ✅ CQRS Pattern
- Clear separation between reads and writes
- Optimized query models
- Command handlers focus on business rules

### ✅ Clean Architecture
- Business logic in Domain (framework-independent)
- Use cases in Application (technology-agnostic)
- Infrastructure details isolated
- Testable and maintainable

## 📚 Files Created

### Domain Layer
- `apps/src/Acontplus.TestDomain/Entities/Order.cs`
- `apps/src/Acontplus.TestDomain/Events/OrderEvents.cs`

### Application Layer
- `apps/src/Acontplus.TestApplication/Dtos/OrderDtos.cs`
- `apps/src/Acontplus.TestApplication/Interfaces/IOrderService.cs`
- `apps/src/Acontplus.TestApplication/Services/OrderService.cs`

### Infrastructure Layer
- `apps/src/Acontplus.TestInfrastructure/EventHandlers/OrderNotificationHandler.cs`
- `apps/src/Acontplus.TestInfrastructure/EventHandlers/OrderAnalyticsHandler.cs`
- `apps/src/Acontplus.TestInfrastructure/EventHandlers/OrderWorkflowHandler.cs`

### Presentation Layer
- `apps/src/Acontplus.TestApi/Endpoints/Business/OrderEndpoints.cs`

---

**This is the proper way to implement Clean Architecture + DDD + CQRS + Event-Driven Architecture!** 🎉
