# Lookup Endpoints

Minimal API endpoints demonstrating the `LookupService` with automatic caching.

## Endpoints

### GET /api/lookups

Get lookup/reference data with automatic caching (30-minute TTL) using `FilterQuery` pattern.

**Query Parameters (FilterQuery):**
- `searchTerm` (optional) - General search term
- `sortBy` (optional) - Property name to sort by
- `sortDirection` (optional) - Sort direction (asc/desc)
- `filters[key]` (optional) - Dynamic filters using array notation

**Examples:**
```http
# Simple filters
GET /api/lookups?filters[module]=test&filters[context]=general

# With search term
GET /api/lookups?searchTerm=admin&filters[module]=test

# With sorting
GET /api/lookups?sortBy=displayOrder&sortDirection=asc

# Complex query
GET /api/lookups?searchTerm=user&sortBy=value&sortDirection=desc&filters[module]=test&filters[context]=general
```

**Response:**
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
        "parentId": null,
        "isDefault": true,
        "isActive": true,
        "description": "User is active and can access the system",
        "metadata": null
      }
    ],
    "userRoles": [
      {
        "id": 1,
        "code": "ADMIN",
        "value": "Administrator",
        "displayOrder": 1,
        "parentId": null,
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
        "parentId": null,
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
        "isDefault": false,
        "isActive": true,
        "description": "State of California",
        "metadata": null
      }
    ]
  },
  "message": "Operation completed successfully.",
  "timestamp": "2024-12-03T10:30:00.000Z"
}
```

### POST /api/lookups/refresh

Refresh lookup cache by removing the cached entry and fetching fresh data using `FilterQuery` pattern.

**Query Parameters (FilterQuery):**
- Same as GET endpoint

**Examples:**
```http
# Refresh with filters
POST /api/lookups/refresh?filters[module]=test&filters[context]=general

# Refresh with complex filters
POST /api/lookups/refresh?filters[module]=test&filters[context]=admin&filters[includeInactive]=false
```

**Response:** Same as GET endpoint

## Testing

Use the `Lookups.http` file to test the endpoints:

**FilterQuery Pattern (Query String)**
```bash
# Simple GET - hits database
GET https://localhost:7001/api/lookups?filters[module]=test&filters[context]=general

# Second call - served from cache (faster)
GET https://localhost:7001/api/lookups?filters[module]=test&filters[context]=general

# With search term
GET https://localhost:7001/api/lookups?searchTerm=admin&filters[module]=test

# With sorting
GET https://localhost:7001/api/lookups?sortBy=displayOrder&sortDirection=asc&filters[module]=test

# Refresh cache
POST https://localhost:7001/api/lookups/refresh?filters[module]=test&filters[context]=general
```

**FilterQuery Binding:**
The `FilterQuery` record automatically binds query parameters:
- `filters[key]=value` → Filters dictionary
- `searchTerm=text` → SearchTerm property
- `sortBy=field` → SortBy property
- `sortDirection=asc` → SortDirection enum

## Cache Behavior

- **Cache Key Format:** `lookups:dbo.getlookups:{module}:{context}`
- **Cache TTL:** 30 minutes
- **Cache Type:** Configured in `appsettings.json` (in-memory or distributed)

## Stored Procedure

The endpoints use `dbo.GetLookups` stored procedure located at:
`apps/database/StoredProcedures/dbo.GetLookups.sql`

**Important:** The stored procedure uses JSON `@Filters` parameter (default ADO repository behavior):
- Query: `filters[module]=test&filters[context]=general`
- SP receives: `@Filters = '{"module":"test","context":"general"}'`
- Parse with: `JSON_VALUE(@Filters, '$.module')`

The stored procedure returns sample data for:
- User Statuses
- User Roles
- Countries
- States (hierarchical - linked to countries via ParentId)

## Features Demonstrated

1. **Automatic Caching** - First request hits database, subsequent requests use cache
2. **Cache Refresh** - Manual cache invalidation via refresh endpoint
3. **Flexible Filtering** - Module and context parameters for different lookup sets
4. **Hierarchical Data** - States linked to Countries via ParentId
5. **Metadata Support** - JSON metadata for icons, colors, etc.
6. **Result Pattern** - Type-safe error handling with `Result<T, E>`

## Performance

- **Cache Hit:** < 1ms
- **Cache Miss:** ~10-50ms (depends on database)
- **Cache Refresh:** Same as cache miss + cache removal overhead

## Integration

This demonstrates how to integrate `LookupService` from `Acontplus.Services` package:

1. ✅ Service registered in `ProgramExtensions.AddAllTestServices()`
2. ✅ Endpoint mapped in `ProgramExtensions.MapTestApiEndpoints()`
3. ✅ Stored procedure created in `apps/database/StoredProcedures/`
4. ✅ HTTP test file for manual testing

## Next Steps

To use in your own API:

1. Copy the endpoint pattern
2. Create your own stored procedure
3. Register `AddLookupService()` in DI
4. Map endpoints in your Program.cs
5. Test with the HTTP file

See full documentation: `docs/lookup-service-quick-reference.md`
