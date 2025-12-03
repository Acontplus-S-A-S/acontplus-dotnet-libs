# FilterQuery Pattern for Lookup Endpoints

This document explains the FilterQuery pattern used in the Lookup endpoints, following the Restaurant API conventions.

## 🎯 Pattern Overview

The endpoints use `FilterQuery` from `Acontplus.Utilities` which provides automatic query string binding for minimal APIs.

## 📝 Endpoint Signature

```csharp
group.MapGet("/", async (
    FilterQuery filterQuery,              // ← Automatically bound from query string
    ILookupService lookupService,
    CancellationToken cancellationToken) =>
{
    var filterRequest = filterQuery.Adapt<FilterRequest>();  // ← Convert to FilterRequest

    return await lookupService
        .GetLookupsAsync("dbo.GetLookups", filterRequest, cancellationToken)
        .ToGetMinimalApiResultAsync();  // ← Convert Result<T,E> to IResult
})
```

## 🔗 Query String Binding

### Basic Filters

```http
GET /api/lookups?filters[module]=test&filters[context]=general
```

Binds to:
```csharp
FilterQuery(
    Filters: { ["module"] = "test", ["context"] = "general" }
)
```

### With Search Term

```http
GET /api/lookups?searchTerm=admin&filters[module]=test
```

Binds to:
```csharp
FilterQuery(
    SearchTerm: "admin",
    Filters: { ["module"] = "test" }
)
```

### With Sorting

```http
GET /api/lookups?sortBy=displayOrder&sortDirection=asc
```

Binds to:
```csharp
FilterQuery(
    SortBy: "displayOrder",
    SortDirection: SortDirection.Asc
)
```

### Complete Example

```http
GET /api/lookups?searchTerm=user&sortBy=value&sortDirection=desc&filters[module]=test&filters[context]=general
```

Binds to:
```csharp
FilterQuery(
    SearchTerm: "user",
    SortBy: "value",
    SortDirection: SortDirection.Desc,
    Filters: { ["module"] = "test", ["context"] = "general" }
)
```

## 🔄 Adaptation Flow

```
Query String
    ↓
FilterQuery (automatic binding via BindAsync)
    ↓
FilterRequest (via Mapster .Adapt<>())
    ↓
ILookupService.GetLookupsAsync()
    ↓
Result<IDictionary<string, IEnumerable<LookupItem>>, DomainError>
    ↓
IResult (via .ToGetMinimalApiResultAsync())
```

## 📦 Required Packages

1. **Acontplus.Utilities** - Contains `FilterQuery` record
2. **Mapster** - Provides `.Adapt<T>()` extension method
3. **Acontplus.Core** - Contains `FilterRequest` and `Result<T,E>`

## 🎨 FilterQuery Structure

```csharp
public sealed record FilterQuery(
    string? SortBy = null,
    SortDirection? SortDirection = null,
    string? SearchTerm = null,
    IReadOnlyDictionary<string, object>? Filters = null
)
{
    public bool IsEmpty => string.IsNullOrWhiteSpace(SearchTerm) &&
                          (Filters is null || !Filters.Any());

    public bool HasCriteria => !IsEmpty;

    // Custom BindAsync for automatic query string binding
    public static ValueTask<FilterQuery?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        // Parses query string parameters automatically
    }
}
```

## 🎯 FilterRequest Structure

```csharp
public record FilterRequest
{
    public string? SortBy { get; init; }
    public SortDirection SortDirection { get; init; } = SortDirection.Asc;
    public string? SearchTerm { get; init; }
    public IReadOnlyDictionary<string, object>? Filters { get; init; }

    public bool IsEmpty { get; }
    public bool HasCriteria { get; }
}
```

## 🔧 Mapster Configuration

Mapster automatically maps between `FilterQuery` and `FilterRequest` because they have the same property names and types. No explicit configuration needed!

```csharp
var filterRequest = filterQuery.Adapt<FilterRequest>();
```

## ✨ Benefits

### 1. Clean Query Strings
```http
# Instead of this (manual dictionary)
GET /api/lookups?module=test&context=general

# Use this (FilterQuery pattern)
GET /api/lookups?filters[module]=test&filters[context]=general
```

### 2. Type Safety
- `FilterQuery` is a record with strong typing
- Compile-time checking
- IntelliSense support

### 3. Automatic Binding
- No manual parameter parsing
- Handles complex scenarios (arrays, nested objects)
- Consistent across all endpoints

### 4. Extensibility
- Easy to add new filter parameters
- Supports search, sort, and custom filters
- No code changes needed for new filters

### 5. Consistency
- Same pattern across all APIs
- Follows Restaurant API conventions
- Familiar to team members

## 📚 Usage Examples

### Example 1: Simple Lookup

```http
GET /api/lookups?filters[module]=test&filters[context]=general
```

Returns all lookups for test module in general context.

### Example 2: Search Lookups

```http
GET /api/lookups?searchTerm=admin&filters[module]=test
```

Returns lookups containing "admin" in test module.

### Example 3: Sorted Lookups

```http
GET /api/lookups?sortBy=displayOrder&sortDirection=asc&filters[module]=test
```

Returns lookups sorted by display order ascending.

### Example 4: Complex Query

```http
GET /api/lookups?searchTerm=user&sortBy=value&sortDirection=desc&filters[module]=test&filters[context]=general&filters[category]=roles
```

Returns lookups matching "user", sorted by value descending, with multiple filters.

### Example 5: Refresh Cache

```http
POST /api/lookups/refresh?filters[module]=test&filters[context]=general
```

Clears cache and fetches fresh data.

## ⚠️ Stored Procedure JSON Parameter

**CRITICAL:** The ADO repository serializes filters as JSON by default (`UseJsonFilters = true`):

### Query String → JSON Parameter

```
filters[module]=test&filters[context]=general

↓ ADO Repository serializes to:

@Filters = '{"module":"test","context":"general"}'
```

### Example Stored Procedure

```sql
CREATE PROCEDURE [dbo].[GetLookups]
    @Filters NVARCHAR(MAX) = NULL,     -- JSON string with all filters
    @SearchTerm NVARCHAR(200) = NULL   -- SearchTerm is separate
AS
BEGIN
    -- Parse JSON filters
    DECLARE @module NVARCHAR(100);
    DECLARE @context NVARCHAR(100);

    IF @Filters IS NOT NULL
    BEGIN
        SELECT
            @module = JSON_VALUE(@Filters, '$.module'),
            @context = JSON_VALUE(@Filters, '$.context');
    END

    -- Set defaults
    SET @module = ISNULL(@module, 'test');
    SET @context = ISNULL(@context, 'general');

    -- Your query here
END
```

### How It Works

1. `FilterQuery` binds from query string
2. Adapts to `FilterRequest`
3. `FilterRequest.Filters` dictionary passed to ADO repository
4. ADO repository serializes to JSON: `@Filters = '{"module":"test",...}'`
5. SP parses JSON with `JSON_VALUE(@Filters, '$.key')`

### Alternative: Individual Parameters

If you prefer individual parameters, set `UseJsonFilters = false`:

```csharp
var options = new CommandOptionsDto { UseJsonFilters = false };
await adoRepository.GetFilteredDataSetAsync("dbo.GetLookups", filterRequest, options);
```

Then your SP can use individual parameters:
```sql
CREATE PROCEDURE [dbo].[GetLookups]
    @module NVARCHAR(100) = NULL,
    @context NVARCHAR(100) = NULL
AS
BEGIN
    -- Direct parameter usage
END
```

## 🔍 Cache Key Generation

The cache key is built from the filters:

```csharp
private static string BuildCacheKey(string storedProcedureName, FilterRequest filterRequest)
{
    var module = NormalizeSegment(ExtractFilterValue(filterRequest, "module"));
    var context = NormalizeSegment(ExtractFilterValue(filterRequest, "context"));
    var spName = NormalizeSegment(storedProcedureName);

    return $"{CacheKeyPrefix}:{spName}:{module}:{context}";
}
```

**Examples:**
- `lookups:dbo.getlookups:test:general`
- `lookups:dbo.getlookups:test:admin`
- `lookups:dbo.getlookups:restaurant:menu`

## 🎭 Result Extension

The `.ToGetMinimalApiResultAsync()` extension converts `Result<T, DomainError>` to `IResult`:

```csharp
public static async Task<IResult> ToGetMinimalApiResultAsync<T>(
    this Task<Result<T, DomainError>> resultTask)
{
    var result = await resultTask;

    return result.Match(
        success => Results.Ok(success),
        error => Results.Problem(/* error details */)
    );
}
```

**Benefits:**
- Automatic HTTP status code mapping
- Consistent error responses
- No manual Result handling in endpoints

## 🚀 Best Practices

### 1. Always Use FilterQuery
```csharp
// ✅ Good
group.MapGet("/", async (FilterQuery filterQuery, ...) => { });

// ❌ Bad - manual parameters
group.MapGet("/", async ([FromQuery] string? module, ...) => { });
```

### 2. Adapt to FilterRequest
```csharp
// ✅ Good
var filterRequest = filterQuery.Adapt<FilterRequest>();

// ❌ Bad - manual mapping
var filterRequest = new FilterRequest { Filters = ... };
```

### 3. Use Extension Methods
```csharp
// ✅ Good
return await service.GetAsync(...).ToGetMinimalApiResultAsync();

// ❌ Bad - manual Result handling
var result = await service.GetAsync(...);
return result.Match(...);
```

### 4. Consistent Naming
```csharp
// ✅ Good - filters[key] syntax
?filters[module]=test&filters[context]=general

// ❌ Bad - inconsistent
?module=test&context=general
```

## 📖 Related Documentation

- [FilterQuery Source](../../../src/Acontplus.Utilities/Dtos/FilterQuery.cs)
- [FilterRequest Source](../../../src/Acontplus.Core/Dtos/Requests/FilterRequest.cs)
- [Result Extensions](../../../src/Acontplus.Utilities/Extensions/ResultApiExtensions.cs)
- [Lookup Service Guide](../../../docs/lookup-service-quick-reference.md)

## 🎉 Summary

The FilterQuery pattern provides:
- ✅ Clean, consistent query string syntax
- ✅ Automatic parameter binding
- ✅ Type safety with records
- ✅ Easy adaptation to service layer
- ✅ Consistent error handling
- ✅ Follows team conventions

This pattern is used across all Acontplus APIs for consistency and maintainability!
