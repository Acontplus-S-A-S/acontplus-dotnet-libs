# LookupService Usage Guide

## Overview

The `LookupService` is a reusable, cached service for managing lookup data across all Acontplus APIs. It's located in the `Acontplus.Services` NuGet package and works seamlessly with both PostgreSQL and SQL Server.

## Architecture Decision

**Location:** `src/Acontplus.Services/Services/`

**Why here?**
- ✅ Database-agnostic (works with both PostgreSQL and SQL Server)
- ✅ Uses abstracted `IUnitOfWork` and `ICacheService` interfaces
- ✅ Reusable across all APIs via NuGet package
- ✅ Application-level service (not infrastructure or persistence specific)
- ✅ Already has proper dependencies on Core and Infrastructure

## Setup in Your API

### 1. Install Required Packages

Your API project needs these Acontplus packages:

```xml
<PackageReference Include="Acontplus.Core" Version="x.x.x" />
<PackageReference Include="Acontplus.Infrastructure" Version="x.x.x" />
<PackageReference Include="Acontplus.Services" Version="x.x.x" />
<PackageReference Include="Acontplus.Persistence.SqlServer" Version="x.x.x" />
<!-- OR -->
<PackageReference Include="Acontplus.Persistence.PostgreSQL" Version="x.x.x" />
```

### 2. Register Services in Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);

// 1. Register cache service (choose one)
builder.Services.AddMemoryCache(); // For in-memory caching
builder.Services.AddMemoryCacheService(); // Acontplus wrapper
// OR
builder.Services.AddDistributedCacheService(); // For Redis/distributed cache

// 2. Register persistence (choose your database)
builder.Services.AddSqlServerPersistence<YourDbContext>(
    builder.Configuration.GetConnectionString("DefaultConnection"));
// OR
builder.Services.AddPostgresPersistence<YourDbContext>(
    builder.Configuration.GetConnectionString("DefaultConnection"));

// 3. Register lookup service
builder.Services.AddLookupService();

var app = builder.Build();
```

### 3. Create Your Stored Procedure

**SQL Server Example:**

```sql
CREATE PROCEDURE [Restaurant].[GetLookups]
    @Module NVARCHAR(100) = NULL,
    @Context NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Return multiple lookup tables
    SELECT
        'OrderStatuses' AS TableName,
        Id,
        Code,
        Value AS [Value],
        DisplayOrder,
        NULL AS ParentId,
        CAST(0 AS BIT) AS IsDefault,
        CAST(1 AS BIT) AS IsActive,
        Description,
        NULL AS Metadata
    FROM Restaurant.OrderStatuses
    WHERE IsActive = 1

    UNION ALL

    SELECT
        'PaymentMethods' AS TableName,
        Id,
        Code,
        [Name] AS [Value],
        SortOrder AS DisplayOrder,
        NULL AS ParentId,
        IsDefault,
        IsActive,
        NULL AS Description,
        JSON_QUERY((SELECT Icon, Color FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)) AS Metadata
    FROM Restaurant.PaymentMethods
    WHERE IsActive = 1

    UNION ALL

    SELECT
        'TableLocations' AS TableName,
        Id,
        Code,
        LocationName AS [Value],
        DisplayOrder,
        ParentLocationId AS ParentId,
        CAST(0 AS BIT) AS IsDefault,
        CAST(1 AS BIT) AS IsActive,
        NULL AS Description,
        NULL AS Metadata
    FROM Restaurant.TableLocations
    WHERE IsActive = 1

    ORDER BY TableName, DisplayOrder;
END
```

**PostgreSQL Example:**

```sql
CREATE OR REPLACE FUNCTION restaurant.get_lookups(
    p_module VARCHAR DEFAULT NULL,
    p_context VARCHAR DEFAULT NULL
)
RETURNS TABLE (
    table_name VARCHAR,
    id INTEGER,
    code VARCHAR,
    value VARCHAR,
    display_order INTEGER,
    parent_id INTEGER,
    is_default BOOLEAN,
    is_active BOOLEAN,
    description TEXT,
    metadata JSONB
) AS $$
BEGIN
    RETURN QUERY

    SELECT
        'orderStatuses'::VARCHAR AS table_name,
        os.id,
        os.code,
        os.value,
        os.display_order,
        NULL::INTEGER AS parent_id,
        FALSE AS is_default,
        TRUE AS is_active,
        os.description,
        NULL::JSONB AS metadata
    FROM restaurant.order_statuses os
    WHERE os.is_active = TRUE

    UNION ALL

    SELECT
        'paymentMethods'::VARCHAR,
        pm.id,
        pm.code,
        pm.name AS value,
        pm.sort_order AS display_order,
        NULL::INTEGER,
        pm.is_default,
        pm.is_active,
        NULL::TEXT,
        jsonb_build_object('icon', pm.icon, 'color', pm.color) AS metadata
    FROM restaurant.payment_methods pm
    WHERE pm.is_active = TRUE

    ORDER BY table_name, display_order;
END;
$$ LANGUAGE plpgsql;
```

### 4. Use in Your Controller

```csharp
using Microsoft.AspNetCore.Mvc;

namespace Restaurant.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LookupsController : ControllerBase
{
    private readonly ILookupService _lookupService;

    public LookupsController(ILookupService lookupService)
    {
        _lookupService = lookupService;
    }

    /// <summary>
    /// Get all lookups with caching
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IDictionary<string, IEnumerable<LookupItem>>>), 200)]
    public async Task<IActionResult> GetLookups(
        [FromQuery] string? module = null,
        [FromQuery] string? context = null,
        CancellationToken cancellationToken = default)
    {
        var filterRequest = new FilterRequest
        {
            Filters = new Dictionary<string, object>
            {
                ["module"] = module ?? "restaurant",
                ["context"] = context ?? "general"
            }
        };

        var result = await _lookupService.GetLookupsAsync(
            "Restaurant.GetLookups", // SQL Server
            // OR "restaurant.get_lookups" for PostgreSQL
            filterRequest,
            cancellationToken);

        return result.Match(
            success => Ok(ApiResponse.Success(success)),
            error => BadRequest(ApiResponse.Failure(error)));
    }

    /// <summary>
    /// Refresh lookups cache
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<IDictionary<string, IEnumerable<LookupItem>>>), 200)]
    public async Task<IActionResult> RefreshLookups(
        [FromQuery] string? module = null,
        [FromQuery] string? context = null,
        CancellationToken cancellationToken = default)
    {
        var filterRequest = new FilterRequest
        {
            Filters = new Dictionary<string, object>
            {
                ["module"] = module ?? "restaurant",
                ["context"] = context ?? "general"
            }
        };

        var result = await _lookupService.RefreshLookupsAsync(
            "Restaurant.GetLookups",
            filterRequest,
            cancellationToken);

        return result.Match(
            success => Ok(ApiResponse.Success(success)),
            error => BadRequest(ApiResponse.Failure(error)));
    }


}
```

## Response Format

```json
{
  "status": "Success",
  "code": "200",
  "data": {
    "orderStatuses": [
      {
        "id": 1,
        "code": "PENDING",
        "value": "Pending",
        "displayOrder": 1,
        "parentId": null,
        "isDefault": true,
        "isActive": true,
        "description": "Order is pending confirmation",
        "metadata": null
      },
      {
        "id": 2,
        "code": "CONFIRMED",
        "value": "Confirmed",
        "displayOrder": 2,
        "parentId": null,
        "isDefault": false,
        "isActive": true,
        "description": "Order has been confirmed",
        "metadata": null
      }
    ],
    "paymentMethods": [
      {
        "id": 1,
        "code": "CASH",
        "value": "Cash",
        "displayOrder": 1,
        "parentId": null,
        "isDefault": true,
        "isActive": true,
        "description": null,
        "metadata": "{\"icon\":\"💵\",\"color\":\"#4CAF50\"}"
      }
    ],
    "tableLocations": [
      {
        "id": 1,
        "code": "INDOOR",
        "value": "Indoor Dining",
        "displayOrder": 1,
        "parentId": null,
        "isDefault": false,
        "isActive": true,
        "description": null,
        "metadata": null
      },
      {
        "id": 2,
        "code": "PATIO",
        "value": "Patio",
        "displayOrder": 2,
        "parentId": 1,
        "isActive": true,
        "description": null,
        "metadata": null
      }
    ]
  },
  "message": "Operation completed successfully.",
  "timestamp": "2024-12-03T10:30:00.000Z"
}
```

## Features

### ✅ Automatic Caching
- Default cache duration: 30 minutes
- Cache key format: `lookups:{storedProcedure}:{module}:{context}`
- Supports both in-memory and distributed (Redis) caching

### ✅ Flexible SQL Mapping
- All properties are nullable for flexible SQL queries
- Missing columns in SQL result = null in DTO
- Works with dynamic stored procedures

### ✅ Hierarchical Support
- Use `ParentId` for nested lookups (e.g., Country → States → Cities)

### ✅ Metadata Support
- Store JSON strings in `Metadata` column
- Useful for icons, colors, custom properties

### ✅ Database Agnostic
- Works with SQL Server and PostgreSQL
- Uses `IUnitOfWork` abstraction

## Best Practices

1. **Cache Management**
   - Use `GetLookupsAsync()` for normal operations (cached)
   - Use `RefreshLookupsAsync()` when data changes (removes specific cache key)

2. **Stored Procedure Design**
   - Always include `TableName` column for grouping
   - Use consistent column names (Id, Code, Value, etc.)
   - Return only active records by default
   - Order by TableName and DisplayOrder

3. **Frontend Integration**
   ```typescript
   interface LookupItem {
     id?: number;
     code?: string;
     value?: string;
     displayOrder?: number;
     parentId?: number;
     isDefault?: boolean;
     isActive?: boolean;
     description?: string;
     metadata?: string;
   }

   interface LookupsResponse {
     [key: string]: LookupItem[];
   }
   ```

4. **Performance**
   - Lookups are cached for 30 minutes
   - First request hits database, subsequent requests use cache
   - Use refresh endpoint after data updates

## Migration from Existing Code

If you have existing lookup code like the Restaurant example:

1. ✅ Move `LookupItem` DTO to `Acontplus.Core.Dtos.Responses` (already done)
2. ✅ Replace `ConcurrentDictionary` with `ICacheService`
3. ✅ Replace `IAdoRepository` with `IUnitOfWork.AdoRepository`
4. ✅ Use `AddLookupService()` extension method
5. ✅ Update controller to inject `ILookupService`
6. ✅ Remove custom caching logic

## Troubleshooting

**Issue:** "The lookup query returned no data sets"
- Check stored procedure returns data
- Verify connection string
- Check filter parameters

**Issue:** Cache not working
- Verify `ICacheService` is registered
- Check cache configuration
- Review logs for cache errors

**Issue:** Missing columns in response
- All properties are nullable - missing SQL columns = null
- Verify stored procedure column names match exactly
- Check `TableName` column exists for grouping
