# LookupService Implementation in TestApi

This document describes the complete implementation of the LookupService in the TestApi project.

## ✅ What Was Implemented

### 1. Minimal API Endpoint
**File:** `apps/src/Acontplus.TestApi/Endpoints/Core/LookupEndpoints.cs`

Two endpoints:
- `GET /api/lookups` - Get cached lookups
- `POST /api/lookups/refresh` - Refresh cache

### 2. Stored Procedure
**File:** `apps/database/StoredProcedures/dbo.GetLookups.sql`

Returns sample lookup data:
- User Statuses (ACTIVE, INACTIVE, SUSPENDED)
- User Roles (ADMIN, USER, GUEST) with metadata
- Countries (US, Ecuador) with metadata
- States (California, New York, Pichincha) - hierarchical

### 3. DI Registration
**File:** `apps/src/Acontplus.TestApi/Extensions/ProgramExtensions.cs`

Added:
```csharp
services.AddLookupService();
```

### 4. Endpoint Mapping
**File:** `apps/src/Acontplus.TestApi/Extensions/ProgramExtensions.cs`

Added:
```csharp
app.MapLookupEndpoints();
```

### 5. Global Usings
**File:** `apps/src/Acontplus.TestApi/GlobalUsings.cs`

Added:
```csharp
global using Acontplus.Services.Services.Abstractions;
```

### 6. HTTP Test File
**File:** `apps/src/Acontplus.TestApi/Lookups.http`

Test scenarios:
- Get lookups (cached)
- Get with parameters
- Refresh cache
- Test cache behavior

### 7. Documentation
**File:** `apps/src/Acontplus.TestApi/Endpoints/Core/LookupEndpoints.README.md`

Complete endpoint documentation with examples.

## 🚀 How to Test

### 1. Run the SQL Script

Execute the stored procedure script:
```sql
-- Run this in your SQL Server database
-- File: apps/database/StoredProcedures/dbo.GetLookups.sql
```

**Important:** The stored procedure uses JSON `@Filters` parameter:
- Query: `filters[module]=test&filters[context]=general`
- ADO repository serializes to: `@Filters = '{"module":"test","context":"general"}'`
- SP parses with: `JSON_VALUE(@Filters, '$.module')`

This is the default behavior (`UseJsonFilters = true` in `CommandOptionsDto`).

### 2. Start the API

```bash
cd apps/src/Acontplus.TestApi
dotnet run
```

### 3. Test with HTTP File

Open `Lookups.http` and run the requests:

**First Request (Cache Miss):**
```http
GET https://localhost:7001/api/lookups?filters[module]=test&filters[context]=general
```

**Second Request (Cache Hit - Faster):**
```http
GET https://localhost:7001/api/lookups?filters[module]=test&filters[context]=general
```

**Refresh Cache:**
```http
POST https://localhost:7001/api/lookups/refresh?filters[module]=test&filters[context]=general
```

### 4. Test with cURL

```bash
# Get lookups with filters
curl -X GET "https://localhost:7001/api/lookups?filters[module]=test&filters[context]=general"

# Get lookups with search
curl -X GET "https://localhost:7001/api/lookups?searchTerm=admin&filters[module]=test"

# Refresh cache
curl -X POST "https://localhost:7001/api/lookups/refresh?filters[module]=test&filters[context]=general"
```

### 5. Test with Swagger

Navigate to: `https://localhost:7001/swagger`

Look for the "Lookups" tag and test the endpoints.

## 📊 Expected Response

```json
{
  "status": "Success",
  "code": "200",
  "data": {
    "userStatuses": [
      {
        "id": 1,
        "code": "ACTIVE",
        "value": "Active",
        "displayOrder": 1,
        "isDefault": true,
        "isActive": true,
        "description": "User is active and can access the system"
      }
    ],
    "userRoles": [
      {
        "id": 1,
        "code": "ADMIN",
        "value": "Administrator",
        "displayOrder": 1,
        "isDefault": false,
        "isActive": true,
        "description": "Full system access",
        "metadata": "{\"icon\":\"👑\",\"color\":\"#FF5722\"}"
      }
    ],
    "countries": [
      {
        "id": 1,
        "code": "US",
        "value": "United States",
        "displayOrder": 1,
        "isDefault": true,
        "isActive": true,
        "description": "United States of America",
        "metadata": "{\"flag\":\"🇺🇸\",\"code\":\"USA\"}"
      }
    ],
    "states": [
      {
        "id": 1,
        "code": "CA",
        "value": "California",
        "displayOrder": 1,
        "parentId": 1,
        "description": "State of California"
      }
    ]
  }
}
```

## 🔍 Verify Cache is Working

### Method 1: Check Response Time
1. First request: ~10-50ms (database query)
2. Second request: < 1ms (from cache)

### Method 2: Check Logs
Look for these log messages:
```
[INF] Fetched 4 lookup groups from dbo.GetLookups
[INF] Cache removed for key: lookups:dbo.getlookups:test:general
```

### Method 3: Test Refresh
1. Call GET endpoint - note the response
2. Call POST refresh endpoint
3. Call GET endpoint again - should hit database

## 🎯 Features Demonstrated

### 1. FilterQuery Pattern
- Automatic parameter binding from query string
- `filters[key]=value` syntax for dynamic filters
- Support for `searchTerm`, `sortBy`, `sortDirection`
- Adapts to `FilterRequest` using Mapster

### 2. Automatic Caching
- First request hits database
- Subsequent requests use cache (30-min TTL)
- Cache key: `lookups:dbo.getlookups:{module}:{context}`

### 3. Cache Refresh
- POST endpoint removes cache
- Next GET request fetches fresh data

### 4. Flexible Filtering
- Dynamic filters via query string
- Different cache keys for different contexts
- Search and sort capabilities

### 5. Hierarchical Data
- States linked to Countries via `ParentId`
- Frontend can build tree structures

### 6. Metadata Support
- JSON strings in `Metadata` column
- Useful for icons, colors, custom properties

### 7. Result Pattern with Extensions
- Type-safe error handling with `Result<T, E>`
- `.ToGetMinimalApiResultAsync()` extension
- Automatic HTTP status code mapping

## 📁 File Structure

```
apps/
├── database/
│   └── StoredProcedures/
│       └── dbo.GetLookups.sql          ← SQL stored procedure
└── src/
    └── Acontplus.TestApi/
        ├── Endpoints/
        │   └── Core/
        │       ├── LookupEndpoints.cs           ← Minimal API endpoints
        │       └── LookupEndpoints.README.md    ← Endpoint docs
        ├── Extensions/
        │   └── ProgramExtensions.cs             ← DI registration
        ├── GlobalUsings.cs                      ← Namespace imports
        ├── Lookups.http                         ← HTTP test file
        └── LOOKUP_IMPLEMENTATION.md             ← This file
```

## 📋 FilterQuery Pattern

The endpoints use the `FilterQuery` pattern from `Acontplus.Utilities`:

### Query String Binding

```http
GET /api/lookups?filters[module]=test&filters[context]=general&searchTerm=admin&sortBy=value&sortDirection=asc
```

This automatically binds to:
```csharp
FilterQuery(
    SortBy: "value",
    SortDirection: SortDirection.Asc,
    SearchTerm: "admin",
    Filters: { ["module"] = "test", ["context"] = "general" }
)
```

### Adaptation to FilterRequest

```csharp
var filterRequest = filterQuery.Adapt<FilterRequest>();
```

Uses Mapster to convert `FilterQuery` → `FilterRequest` for the service layer.

### Benefits

- ✅ Clean query string syntax
- ✅ Automatic parameter binding
- ✅ Type-safe with records
- ✅ Supports dynamic filters
- ✅ Consistent across all endpoints

## 🔧 Configuration

The LookupService uses the cache configuration from `appsettings.json`:

```json
{
  "Cache": {
    "UseDistributedCache": false,
    "ExpirationScanFrequencyMinutes": 30
  }
}
```

For production with multiple servers, use distributed cache:

```json
{
  "Cache": {
    "UseDistributedCache": true
  },
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  }
}
```

## 🐛 Troubleshooting

### Issue: "Stored procedure not found"
**Solution:** Run the SQL script in `apps/database/StoredProcedures/dbo.GetLookups.sql`

### Issue: "Cache not working"
**Solution:** Verify `AddInfrastructureServices()` is called before `AddLookupService()`

### Issue: "Namespace not found"
**Solution:** Check `GlobalUsings.cs` includes `Acontplus.Services.Services.Abstractions`

### Issue: "Endpoint not found"
**Solution:** Verify `app.MapLookupEndpoints()` is called in `MapTestApiEndpoints()`

## 📚 Related Documentation

- [LookupService Quick Reference](../../../docs/lookup-service-quick-reference.md)
- [LookupService Usage Guide](../../../docs/lookup-service-usage.md)
- [LookupService Architecture](../../../docs/lookup-service-architecture.md)
- [Services Package README](../../../src/Acontplus.Services/README.md)

## ✨ Next Steps

1. ✅ Run the stored procedure script
2. ✅ Start the API
3. ✅ Test with HTTP file
4. ✅ Verify cache is working
5. ✅ Check logs for cache hits/misses
6. ✅ Test refresh endpoint
7. ✅ Integrate into your own API

## 🎉 Success Criteria

- [x] Endpoints return 200 OK
- [x] Response contains lookup data
- [x] Second request is faster (cached)
- [x] Refresh endpoint clears cache
- [x] Logs show cache operations
- [x] No errors in console

Congratulations! You've successfully implemented the LookupService in your API! 🚀
