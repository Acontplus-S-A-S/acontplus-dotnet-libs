# Lookup Service Troubleshooting Guide

Common issues and solutions when using the Lookup Service.

## ❌ Error: "@Filters is not a parameter for procedure GetLookups"

### Problem
```json
{
  "status": "error",
  "code": "500",
  "message": "An error occurred while retrieving lookups: @Filters is not a parameter for procedure GetLookups."
}
```

### Cause
The ADO repository is trying to pass the entire `Filters` dictionary as a parameter, but the stored procedure doesn't have a `@Filters` parameter.

### Solution
The ADO repository uses JSON serialization by default (`UseJsonFilters = true`). Your stored procedure should accept `@Filters` as JSON:

**Query String:**
```http
GET /api/lookups?filters[module]=test&filters[context]=general
```

**ADO Repository Serializes To:**
```sql
EXEC dbo.GetLookups @Filters = '{"module":"test","context":"general"}'
```

**Stored Procedure (CORRECT):**
```sql
CREATE PROCEDURE [dbo].[GetLookups]
    @Filters NVARCHAR(MAX) = NULL,     -- ✅ JSON string
    @SearchTerm NVARCHAR(200) = NULL
AS
BEGIN
    -- Parse JSON
    DECLARE @module NVARCHAR(100);
    DECLARE @context NVARCHAR(100);

    IF @Filters IS NOT NULL
    BEGIN
        SELECT
            @module = JSON_VALUE(@Filters, '$.module'),
            @context = JSON_VALUE(@Filters, '$.context');
    END

    -- Your query
END
```

**Alternative: Individual Parameters**

If you prefer individual parameters, modify the service to use `UseJsonFilters = false`:

```csharp
var options = new CommandOptionsDto { UseJsonFilters = false };
await _unitOfWork.AdoRepository.GetFilteredDataSetAsync(
    storedProcedureName,
    filterRequest,
    options,
    cancellationToken);
```

Then your SP can use:
```sql
CREATE PROCEDURE [dbo].[GetLookups]
    @module NVARCHAR(100) = NULL,
    @context NVARCHAR(100) = NULL
AS
BEGIN
    -- Direct parameter usage
END
```

### Key Points
1. ✅ Default behavior: `@Filters` as JSON (UseJsonFilters = true)
2. ✅ Parse JSON with `JSON_VALUE(@Filters, '$.key')`
3. ✅ Alternative: Set `UseJsonFilters = false` for individual parameters
4. ✅ `@SearchTerm` is always a separate parameter

---

## ❌ Error: "Stored procedure not found"

### Problem
```json
{
  "status": "error",
  "message": "Could not find stored procedure 'dbo.GetLookups'."
}
```

### Solution
1. Run the SQL script: `apps/database/StoredProcedures/dbo.GetLookups.sql`
2. Verify the procedure exists:
   ```sql
   SELECT * FROM sys.procedures WHERE name = 'GetLookups'
   ```
3. Check the schema: `dbo.GetLookups` vs `GetLookups`

---

## ❌ Error: "Cache not working"

### Problem
Every request hits the database, cache doesn't seem to work.

### Symptoms
- Response times are always slow
- Logs show database queries on every request
- No cache hit messages in logs

### Solution

#### 1. Verify Cache Service is Registered
```csharp
// In Program.cs or ProgramExtensions.cs
builder.Services.AddInfrastructureServices(configuration); // ✅ Must be called
builder.Services.AddLookupService(); // ✅ After infrastructure
```

#### 2. Check Cache Configuration
```json
// appsettings.json
{
  "Cache": {
    "UseDistributedCache": false,  // Use in-memory for testing
    "ExpirationScanFrequencyMinutes": 30
  }
}
```

#### 3. Verify Cache Keys Match
Cache keys are built from filters. Same filters = same cache key.

```http
# These use the SAME cache key
GET /api/lookups?filters[module]=test&filters[context]=general
GET /api/lookups?filters[module]=test&filters[context]=general

# These use DIFFERENT cache keys
GET /api/lookups?filters[module]=test&filters[context]=general
GET /api/lookups?filters[module]=test&filters[context]=admin
```

#### 4. Check Logs
Look for these messages:
```
[INF] Fetched 4 lookup groups from dbo.GetLookups  ← Cache miss
[INF] Cache removed for key: lookups:dbo.getlookups:test:general  ← Refresh
```

---

## ❌ Error: "Namespace not found"

### Problem
```
The type or namespace name 'ILookupService' could not be found
```

### Solution

#### 1. Add Global Using
```csharp
// GlobalUsings.cs
global using Acontplus.Services.Services.Abstractions;
```

#### 2. Verify Project Reference
```xml
<!-- Acontplus.TestApi.csproj -->
<ProjectReference Include="..\..\..\src\Acontplus.Services\Acontplus.Services.csproj" />
```

#### 3. Check Package References
```xml
<PackageReference Include="Mapster" />
```

---

## ❌ Error: "Adapt method not found"

### Problem
```
'FilterQuery' does not contain a definition for 'Adapt'
```

### Solution
Install Mapster NuGet package:
```bash
dotnet add package Mapster
```

Or add to `Directory.Packages.props`:
```xml
<PackageVersion Include="Mapster" Version="7.4.0" />
```

---

## ❌ Error: "ToGetMinimalApiResultAsync not found"

### Problem
```
'Task<Result<...>>' does not contain a definition for 'ToGetMinimalApiResultAsync'
```

### Solution

#### 1. Add Global Using
```csharp
// GlobalUsings.cs
global using Acontplus.Utilities.Extensions;
```

#### 2. Verify Project Reference
```xml
<ProjectReference Include="..\..\..\src\Acontplus.Utilities\Acontplus.Utilities.csproj" />
```

---

## ❌ Error: "Empty response / No data"

### Problem
```json
{
  "status": "error",
  "code": "404",
  "message": "The lookup query returned no data sets."
}
```

### Causes & Solutions

#### 1. Stored Procedure Returns No Rows
Check if your SP has data:
```sql
EXEC dbo.GetLookups @module = 'test', @context = 'general'
```

#### 2. Missing TableName Column
Ensure your SP returns `TableName` column:
```sql
SELECT
    'UserStatuses' AS TableName,  -- ✅ Required!
    Id, Code, Value, ...
```

#### 3. All Rows Filtered Out
Check your WHERE clauses in the SP.

---

## ❌ Error: "Invalid cast" or "Null reference"

### Problem
```
System.InvalidCastException: Unable to cast object of type 'System.DBNull' to type 'System.Int32'
```

### Solution
All `LookupItem` properties are nullable. Ensure your SP returns nullable columns:

```sql
SELECT
    Id,                    -- Can be NULL
    Code,                  -- Can be NULL
    [Value],              -- Can be NULL
    DisplayOrder,         -- Can be NULL
    ParentId,             -- Can be NULL
    IsDefault,            -- Can be NULL
    IsActive,             -- Can be NULL
    Description,          -- Can be NULL
    Metadata              -- Can be NULL
FROM YourTable
```

Use `NULL` instead of throwing errors for missing data.

---

## ❌ Error: "Parameter count mismatch"

### Problem
```
Procedure or function 'GetLookups' expects parameter '@module', which was not supplied.
```

### Solution
Make all SP parameters optional with defaults:

```sql
CREATE PROCEDURE [dbo].[GetLookups]
    @module NVARCHAR(100) = NULL,      -- ✅ Optional with default
    @context NVARCHAR(100) = NULL,     -- ✅ Optional with default
    @searchTerm NVARCHAR(200) = NULL   -- ✅ Optional with default
AS
BEGIN
    -- Set defaults if needed
    SET @module = ISNULL(@module, 'test');
    SET @context = ISNULL(@context, 'general');

    -- Your query
END
```

---

## 🔍 Debugging Tips

### 1. Enable Detailed Logging
```json
// appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "Acontplus.Services": "Debug",
      "Acontplus.Infrastructure": "Debug"
    }
  }
}
```

### 2. Check SQL Profiler
Use SQL Server Profiler to see actual SP calls:
```sql
EXEC dbo.GetLookups @module = 'test', @context = 'general'
```

### 3. Test SP Directly
Run the SP in SSMS to verify it works:
```sql
EXEC dbo.GetLookups @module = 'test', @context = 'general'
```

### 4. Check Cache Keys
Add logging to see cache keys:
```csharp
_logger.LogDebug("Cache key: {CacheKey}", cacheKey);
```

### 5. Test Without Cache
Temporarily disable caching to isolate issues:
```csharp
// Comment out cache temporarily
// return await _cacheService.GetOrCreateAsync(...);
return await FetchLookupsAsync(...);
```

---

## ✅ Verification Checklist

Before asking for help, verify:

- [ ] Stored procedure exists and runs in SSMS
- [ ] SP parameters match filter keys (lowercase)
- [ ] SP returns `TableName` column
- [ ] All SP parameters are optional (`= NULL`)
- [ ] `AddInfrastructureServices()` is called before `AddLookupService()`
- [ ] `AddLookupService()` is registered in DI
- [ ] `MapLookupEndpoints()` is called
- [ ] Mapster package is installed
- [ ] Global usings include necessary namespaces
- [ ] Cache configuration exists in appsettings.json

---

## 📚 Related Documentation

- [Implementation Guide](./LOOKUP_IMPLEMENTATION.md)
- [FilterQuery Pattern](./LOOKUP_FILTERQUERY_PATTERN.md)
- [Endpoint Documentation](./Endpoints/Core/LookupEndpoints.README.md)
- [Quick Reference](../../../docs/lookup-service-quick-reference.md)

---

## 🆘 Still Having Issues?

1. Check the logs for detailed error messages
2. Run the stored procedure directly in SSMS
3. Verify all dependencies are installed
4. Test with the provided HTTP file
5. Compare with working examples in the codebase

If the issue persists, provide:
- Full error message
- Stored procedure definition
- Query string used
- Relevant logs
