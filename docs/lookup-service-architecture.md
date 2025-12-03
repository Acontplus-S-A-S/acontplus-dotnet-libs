# LookupService Architecture & Design Decisions

## Overview

The `LookupService` is a reusable, cached service for managing lookup/reference data across all Acontplus APIs.

## Architecture Decisions

### 1. Location: `Acontplus.Services` Package

**Decision:** Place the service in `src/Acontplus.Services/Services/`

**Rationale:**
- ✅ **Database Agnostic**: Works with both PostgreSQL and SQL Server through `IUnitOfWork` abstraction
- ✅ **Reusable**: Available to all APIs via NuGet package
- ✅ **Proper Layer**: Application-level service, not infrastructure or persistence specific
- ✅ **Dependencies**: Already has access to Core and Infrastructure packages
- ✅ **Not Persistence-Specific**: Doesn't belong in `Acontplus.Persistence.SqlServer` or `Acontplus.Persistence.PostgreSQL`

**Alternatives Considered:**
- ❌ `Acontplus.Persistence.SqlServer` - Too database-specific, not reusable for PostgreSQL
- ❌ `Acontplus.Persistence.PostgreSQL` - Too database-specific, not reusable for SQL Server
- ❌ `Acontplus.Core` - Too low-level, should not contain business logic
- ❌ `Acontplus.Infrastructure` - Infrastructure is for cross-cutting concerns (caching, HTTP, resilience)

### 2. Caching Strategy: Use Existing `ICacheService`

**Decision:** Replace `ConcurrentDictionary` with `ICacheService` from `Acontplus.Infrastructure`

**Benefits:**
- ✅ Supports both in-memory and distributed (Redis) caching
- ✅ Consistent caching across all services
- ✅ Built-in logging and error handling
- ✅ Thread-safe operations
- ✅ Cache statistics and monitoring
- ✅ Configurable expiration

**Original Implementation Issues:**
- ❌ Static `ConcurrentDictionary` - memory leaks in long-running apps
- ❌ No distributed caching support
- ❌ Manual cache management
- ❌ No monitoring/statistics

### 3. Data Access: Use `IUnitOfWork` Abstraction

**Decision:** Use `IUnitOfWork.AdoRepository` instead of direct `IAdoRepository` injection

**Benefits:**
- ✅ Works with both EF Core and ADO.NET
- ✅ Transaction support
- ✅ Consistent with other Acontplus services
- ✅ Database-agnostic (SQL Server, PostgreSQL)
- ✅ Supports multiple database contexts (keyed services)

### 4. DTO Design: All Nullable Properties

**Decision:** Make all `LookupItem` properties nullable

**Rationale:**
- ✅ Flexible SQL query mapping
- ✅ Handles missing columns gracefully
- ✅ Works with dynamic stored procedures
- ✅ No mapping exceptions when columns are optional

**Trade-off:**
- Consumers must handle null values
- Validation should be done at business logic layer if needed

### 5. Stored Procedure Flexibility

**Decision:** Pass stored procedure name as parameter instead of hardcoding

**Benefits:**
- ✅ Reusable across different APIs
- ✅ Each API can have its own lookup procedures
- ✅ Supports multiple lookup contexts (module, context filters)
- ✅ Easy to test with different procedures

## Component Structure

```
Acontplus.Services/
├── Services/
│   ├── Abstractions/
│   │   └── ILookupService.cs          # Interface
│   ├── Implementations/
│   │   └── LookupService.cs           # Implementation
│   └── README.md                       # Documentation
├── Extensions/
│   └── ServiceExtensions.cs           # DI registration
└── GlobalUsings.cs                     # Namespace imports

Acontplus.Core/
└── Dtos/
    └── Responses/
        └── LookupItem.cs               # Shared DTO

Acontplus.Infrastructure/
└── Caching/
    ├── ICacheService.cs                # Cache abstraction
    ├── MemoryCacheService.cs           # In-memory implementation
    └── DistributedCacheService.cs      # Redis implementation
```

## Data Flow

```
Controller
    ↓
ILookupService.GetLookupsAsync()
    ↓
Check Cache (ICacheService)
    ↓ (cache miss)
IUnitOfWork.AdoRepository.GetFilteredDataSetAsync()
    ↓
Stored Procedure Execution
    ↓
Map DataSet → Dictionary<string, IEnumerable<LookupItem>>
    ↓
Store in Cache
    ↓
Return Result<T, DomainError>
```

## Cache Key Strategy

**Format:** `lookups:{storedProcedure}:{module}:{context}`

**Examples:**
- `lookups:restaurant.getlookups:restaurant:general`
- `lookups:inventory.getlookups:warehouse:default`
- `lookups:hr.getlookups:employees:active`

**Benefits:**
- Unique per API and context
- Easy to invalidate specific lookups
- Supports multi-tenant scenarios

## Error Handling

**Strategy:** Return `Result<T, DomainError>` pattern

**Benefits:**
- ✅ Type-safe error handling
- ✅ No exceptions for business logic errors
- ✅ Consistent with Acontplus patterns
- ✅ Easy to map to HTTP responses

**Error Codes:**
- `LOOKUPS_GET_ERROR` - Error retrieving lookups
- `LOOKUPS_REFRESH_ERROR` - Error refreshing cache
- `LOOKUPS_EMPTY` - No data returned from query

## Performance Considerations

### Caching
- **Default TTL:** 30 minutes
- **Cache Type:** Configurable (in-memory or distributed)
- **Cache Invalidation:** Manual via `RefreshLookupsAsync()` (removes specific cache key)

### Database
- **Query Type:** Stored procedures (optimized)
- **Connection:** Reuses existing `IUnitOfWork` connection
- **Result Mapping:** Efficient DataTable → LINQ projection

### Scalability
- **In-Memory Cache:** Good for single-server deployments
- **Distributed Cache:** Required for multi-server/load-balanced scenarios
- **Cache Warming:** First request per key hits database

## Testing Strategy

### Unit Tests
```csharp
// Mock dependencies
var mockUnitOfWork = new Mock<IUnitOfWork>();
var mockCacheService = new Mock<ICacheService>();
var mockLogger = new Mock<ILogger<LookupService>>();

var service = new LookupService(
    mockUnitOfWork.Object,
    mockCacheService.Object,
    mockLogger.Object);

// Test cache hit
mockCacheService
    .Setup(x => x.GetOrCreateAsync(...))
    .ReturnsAsync(expectedResult);

var result = await service.GetLookupsAsync(...);
```

### Integration Tests
```csharp
// Use real database and cache
var services = new ServiceCollection();
services.AddMemoryCacheService();
services.AddSqlServerPersistence<TestDbContext>(connectionString);
services.AddLookupService();

var provider = services.BuildServiceProvider();
var lookupService = provider.GetRequiredService<ILookupService>();

var result = await lookupService.GetLookupsAsync("Test.GetLookups", filterRequest);
```

## Migration Path

### From Existing Code
1. ✅ Add `LookupItem` to `Acontplus.Core.Dtos.Responses`
2. ✅ Create `ILookupService` interface
3. ✅ Implement `LookupService` using `ICacheService`
4. ✅ Add DI extension method
5. ✅ Update controllers to use `ILookupService`
6. ✅ Remove old caching logic

### Breaking Changes
- None - this is a new service

### Backward Compatibility
- Existing APIs can continue using their own lookup logic
- Gradual migration recommended

## Future Enhancements

### Potential Improvements
1. **Cache Warming** - Pre-load common lookups on startup
2. **Cache Tags** - Group related lookups for bulk invalidation
3. **Change Tracking** - Auto-refresh when database changes detected
4. **Compression** - Compress large lookup datasets in cache
5. **Versioning** - Support multiple versions of lookup data
6. **Localization** - Multi-language lookup values

### Configuration Options
```csharp
services.AddLookupService(options =>
{
    options.DefaultCacheDuration = TimeSpan.FromMinutes(60);
    options.EnableCacheWarming = true;
    options.EnableCompression = true;
    options.MaxCacheSize = 1000;
});
```

## Security Considerations

### SQL Injection
- ✅ Uses parameterized stored procedures
- ✅ Filter values are passed as parameters
- ✅ No dynamic SQL construction

### Data Access
- ✅ Respects existing `IUnitOfWork` security
- ✅ No elevation of privileges
- ✅ Uses application's database context

### Cache Poisoning
- ✅ Cache keys are deterministic
- ✅ No user input in cache keys (normalized)
- ✅ Cache expiration prevents stale data

## Monitoring & Observability

### Logging
- Cache hits/misses
- Database query execution
- Error conditions
- Cache refresh operations

### Metrics (Future)
- Cache hit rate
- Average response time
- Database query duration
- Cache size

### Health Checks (Future)
```csharp
services.AddHealthChecks()
    .AddCheck<LookupServiceHealthCheck>("lookup-service");
```

## Conclusion

The `LookupService` provides a robust, reusable solution for managing lookup data across all Acontplus APIs. By leveraging existing infrastructure (`ICacheService`, `IUnitOfWork`) and following established patterns (`Result<T, E>`, DI extensions), it integrates seamlessly with the Acontplus ecosystem while providing flexibility for different database backends and caching strategies.
