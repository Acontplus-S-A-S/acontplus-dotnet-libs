# LookupService Quick Reference

## 🚀 Quick Start

### 1. Register Service (Program.cs)
```csharp
builder.Services.AddMemoryCacheService(); // or AddDistributedCacheService()
builder.Services.AddSqlServerPersistence<YourDbContext>(connectionString);
builder.Services.AddLookupService();
```

### 2. Inject & Use
```csharp
public class MyController : ControllerBase
{
    private readonly ILookupService _lookupService;

    public MyController(ILookupService lookupService)
    {
        _lookupService = lookupService;
    }

    [HttpGet("lookups")]
    public async Task<IActionResult> GetLookups()
    {
        var filterRequest = new FilterRequest
        {
            Filters = new Dictionary<string, object>
            {
                ["module"] = "myModule",
                ["context"] = "general"
            }
        };

        var result = await _lookupService.GetLookupsAsync(
            "MySchema.GetLookups",
            filterRequest);

        return result.Match(
            success => Ok(ApiResponse.Success(success)),
            error => BadRequest(ApiResponse.Failure(error)));
    }
}
```

## 📋 Required SQL Columns

Your stored procedure MUST return these columns:

| Column | Type | Required | Description |
|--------|------|----------|-------------|
| `TableName` | string | ✅ Yes | Groups results (e.g., "Countries", "States") |
| `Id` | int? | No | Unique identifier |
| `Code` | string? | No | Short code (e.g., "US", "CA") |
| `Value` | string? | No | Display text |
| `DisplayOrder` | int? | No | Sort order |
| `ParentId` | int? | No | For hierarchical data |
| `IsDefault` | bool? | No | Default selection |
| `IsActive` | bool? | No | Active/inactive flag |
| `Description` | string? | No | Tooltip or help text |
| `Metadata` | string? | No | JSON string for custom data |

## 🗄️ SQL Server Template

```sql
CREATE PROCEDURE [YourSchema].[GetLookups]
    @Module NVARCHAR(100) = NULL,
    @Context NVARCHAR(100) = NULL
AS
BEGIN
    SELECT
        'YourTableName' AS TableName,
        Id,
        Code,
        [Name] AS [Value],
        DisplayOrder,
        ParentId,
        IsDefault,
        IsActive,
        Description,
        Metadata
    FROM YourSchema.YourTable
    WHERE IsActive = 1
    ORDER BY DisplayOrder;
END
```

## 🐘 PostgreSQL Template

```sql
CREATE OR REPLACE FUNCTION your_schema.get_lookups(
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
        'yourTableName'::VARCHAR AS table_name,
        t.id,
        t.code,
        t.name AS value,
        t.display_order,
        t.parent_id,
        t.is_default,
        t.is_active,
        t.description,
        t.metadata::JSONB
    FROM your_schema.your_table t
    WHERE t.is_active = TRUE
    ORDER BY t.display_order;
END;
$$ LANGUAGE plpgsql;
```

## 🎯 API Methods

### GetLookupsAsync (Cached)
```csharp
var result = await _lookupService.GetLookupsAsync(
    storedProcedureName: "Schema.GetLookups",
    filterRequest: filterRequest,
    cancellationToken: cancellationToken);
```
- ✅ Uses cache (30 min TTL)
- ✅ Fast response
- ✅ Use for normal operations

### RefreshLookupsAsync (Force Refresh)
```csharp
var result = await _lookupService.RefreshLookupsAsync(
    storedProcedureName: "Schema.GetLookups",
    filterRequest: filterRequest,
    cancellationToken: cancellationToken);
```
- ✅ Removes cache for specific lookup
- ✅ Fetches fresh data
- ✅ Use after data updates

## 📦 Response Format

```json
{
  "status": "Success",
  "code": "200",
  "data": {
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
        "metadata": "{\"flag\":\"🇺🇸\"}"
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
        "description": null,
        "metadata": null
      }
    ]
  }
}
```

## 🔑 Cache Keys

Format: `lookups:{storedProcedure}:{module}:{context}`

Examples:
- `lookups:restaurant.getlookups:restaurant:general`
- `lookups:inventory.getlookups:warehouse:default`

## ⚙️ Configuration

### In-Memory Cache (Single Server)
```csharp
builder.Services.AddMemoryCache();
builder.Services.AddMemoryCacheService();
```

### Distributed Cache (Multi-Server)
```csharp
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});
builder.Services.AddDistributedCacheService();
```

## 🎨 Frontend TypeScript

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

// Usage
const response = await fetch('/api/lookups?module=restaurant&context=general');
const data: ApiResponse<LookupsResponse> = await response.json();

const countries = data.data.countries;
const states = data.data.states;
```

## 🐛 Common Issues

### "The lookup query returned no data sets"
- ✅ Check stored procedure returns data
- ✅ Verify connection string
- ✅ Check filter parameters match SP expectations

### Cache not working
- ✅ Verify `ICacheService` is registered
- ✅ Check `AddMemoryCacheService()` or `AddDistributedCacheService()` is called
- ✅ Review logs for cache errors

### Missing columns in response
- ✅ All properties are nullable - missing columns = null
- ✅ Verify SP column names match exactly (case-sensitive)
- ✅ Ensure `TableName` column exists

### Hierarchical data not working
- ✅ Use `ParentId` to link child items
- ✅ Ensure parent records are returned first
- ✅ Frontend must build tree structure

## 📚 Related Documentation

- [Full Usage Guide](./lookup-service-usage.md)
- [Architecture & Design](./lookup-service-architecture.md)
- [Service README](../src/Acontplus.Services/Services/README.md)

## 💡 Best Practices

1. ✅ Always include `TableName` for grouping
2. ✅ Use `DisplayOrder` for consistent sorting
3. ✅ Return only active records by default
4. ✅ Use `Metadata` for JSON custom properties
5. ✅ Call `RefreshLookupsAsync()` after data changes
6. ✅ Use distributed cache for multi-server deployments
7. ✅ Keep stored procedures simple and fast
8. ✅ Add indexes on lookup tables for performance

## 🔒 Security

- ✅ Uses parameterized stored procedures (no SQL injection)
- ✅ Respects existing database permissions
- ✅ No elevation of privileges
- ✅ Cache keys are normalized (no user input)

## 📊 Performance

- **Cache Hit:** < 1ms
- **Cache Miss:** Depends on SP execution time
- **Cache TTL:** 30 minutes (default)
- **Recommended:** Use distributed cache for production

## 🎯 When to Use

✅ **Use LookupService for:**
- Dropdown lists
- Select boxes
- Reference data
- Static/semi-static data
- Multi-table lookups

❌ **Don't use for:**
- Frequently changing data
- User-specific data
- Large datasets (> 10,000 records)
- Real-time data
