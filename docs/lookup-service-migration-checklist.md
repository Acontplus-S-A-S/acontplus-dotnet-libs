# LookupService Migration Checklist

Use this checklist to migrate your existing Restaurant API (or any API) to use the new `LookupService`.

## ✅ Pre-Migration Checklist

- [x] `LookupItem` DTO created in `Acontplus.Core.Dtos.Responses`
- [x] `ILookupService` interface created in `Acontplus.Services`
- [x] `LookupService` implementation created
- [x] DI extension method `AddLookupService()` added
- [x] Documentation created

## 🔄 Migration Steps for Your API

### Step 1: Update Dependencies
- [ ] Ensure your API references `Acontplus.Services` NuGet package
- [ ] Ensure your API references `Acontplus.Infrastructure` NuGet package
- [ ] Ensure your API references `Acontplus.Core` NuGet package
- [ ] Verify package versions are compatible

### Step 2: Register Services (Program.cs)
- [ ] Add cache service registration:
  ```csharp
  // For development/single server
  builder.Services.AddMemoryCache();
  builder.Services.AddMemoryCacheService();

  // OR for production/multi-server
  builder.Services.AddStackExchangeRedisCache(options => {
      options.Configuration = builder.Configuration.GetConnectionString("Redis");
  });
  builder.Services.AddDistributedCacheService();
  ```
- [ ] Verify `IUnitOfWork` is registered (should already be from persistence package)
- [ ] Add lookup service registration:
  ```csharp
  builder.Services.AddLookupService();
  ```

### Step 3: Update/Create Stored Procedure
- [ ] Review your existing stored procedure (if any)
- [ ] Ensure it returns these columns:
  - [ ] `TableName` (string) - Required for grouping
  - [ ] `Id` (int?)
  - [ ] `Code` (string?)
  - [ ] `Value` (string?)
  - [ ] `DisplayOrder` (int?)
  - [ ] `ParentId` (int?)
  - [ ] `IsDefault` (bool?)
  - [ ] `IsActive` (bool?) - Added in new version
  - [ ] `Description` (string?) - Added in new version
  - [ ] `Metadata` (string?)
- [ ] Test stored procedure returns data correctly
- [ ] Add indexes if needed for performance

**SQL Server Example:**
```sql
CREATE PROCEDURE [Restaurant].[GetLookups]
    @Module NVARCHAR(100) = NULL,
    @Context NVARCHAR(100) = NULL
AS
BEGIN
    SELECT
        'OrderStatuses' AS TableName,
        Id, Code, [Name] AS [Value], DisplayOrder,
        NULL AS ParentId, IsDefault, IsActive,
        Description, NULL AS Metadata
    FROM Restaurant.OrderStatuses
    WHERE IsActive = 1
    ORDER BY DisplayOrder;
END
```

### Step 4: Update Controller
- [ ] Remove old service interface (if exists)
- [ ] Remove old service implementation (if exists)
- [ ] Update controller constructor to inject `ILookupService`:
  ```csharp
  private readonly ILookupService _lookupService;

  public LookupsController(ILookupService lookupService)
  {
      _lookupService = lookupService;
  }
  ```
- [ ] Update GET endpoint:
  ```csharp
  [HttpGet]
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
          "Restaurant.GetLookups",
          filterRequest,
          cancellationToken);

      return result.Match(
          success => Ok(ApiResponse.Success(success)),
          error => BadRequest(ApiResponse.Failure(error)));
  }
  ```
- [ ] Add refresh endpoint:
  ```csharp
  [HttpPost("refresh")]
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
  ```


### Step 5: Remove Old Code
- [ ] Remove old `LookupService` class (if exists in your API)
- [ ] Remove old `ILookupService` interface (if exists in your API)
- [ ] Remove old `LookupItem` DTO (if exists in your API)
- [ ] Remove `ConcurrentDictionary` caching logic
- [ ] Remove old DI registrations
- [ ] Clean up unused using statements

### Step 6: Update Frontend (if needed)
- [ ] Update TypeScript interfaces to include new properties:
  ```typescript
  interface LookupItem {
    id?: number;
    code?: string;
    value?: string;
    displayOrder?: number;
    parentId?: number;
    isDefault?: boolean;
    isActive?: boolean;      // NEW
    description?: string;    // NEW
    metadata?: string;
  }
  ```
- [ ] Update API calls if endpoint URLs changed
- [ ] Test frontend still works with new response format

### Step 7: Testing
- [ ] Unit test: Service registration
- [ ] Integration test: GET lookups endpoint
- [ ] Integration test: Refresh lookups endpoint
- [ ] Integration test: Cache is working (second call is faster)
- [ ] Integration test: Refresh removes cache for specific key
- [ ] Load test: Multiple concurrent requests
- [ ] Test with empty result set
- [ ] Test with missing filter parameters
- [ ] Test with invalid stored procedure name
- [ ] Test cache expiration (wait 30+ minutes)

### Step 8: Configuration
- [ ] Add cache configuration to appsettings.json (if needed):
  ```json
  {
    "Cache": {
      "ExpirationScanFrequencyMinutes": 30,
      "SizeLimit": 1024
    },
    "ConnectionStrings": {
      "Redis": "localhost:6379" // if using distributed cache
    }
  }
  ```
- [ ] Configure cache for different environments (dev, staging, prod)
- [ ] Document cache configuration in README

### Step 9: Deployment
- [ ] Update deployment scripts (if needed)
- [ ] Ensure Redis is available (if using distributed cache)
- [ ] Deploy to staging environment
- [ ] Smoke test in staging
- [ ] Monitor logs for errors
- [ ] Check cache hit rates
- [ ] Deploy to production
- [ ] Monitor production logs

### Step 10: Documentation
- [ ] Update API documentation (Swagger/OpenAPI)
- [ ] Update team wiki/confluence
- [ ] Update README with new endpoints
- [ ] Document cache management procedures
- [ ] Document troubleshooting steps

## 🧪 Testing Checklist

### Manual Testing
- [ ] Call GET /api/lookups - should return data
- [ ] Call GET /api/lookups again - should be faster (cached)
- [ ] Call POST /api/lookups/refresh - should remove cache for that key
- [ ] Call GET /api/lookups again - should hit database
- [ ] Test with different module/context parameters
- [ ] Test with missing parameters
- [ ] Verify response format matches expected

### Automated Testing
- [ ] Write unit tests for controller
- [ ] Write integration tests for service
- [ ] Write integration tests for stored procedure
- [ ] Add tests to CI/CD pipeline
- [ ] Verify code coverage

### Performance Testing
- [ ] Measure response time (cache hit)
- [ ] Measure response time (cache miss)
- [ ] Test with 100 concurrent requests
- [ ] Monitor memory usage
- [ ] Monitor database connections

## 📊 Verification

### Before Migration
- [ ] Document current response times
- [ ] Document current error rates
- [ ] Document current cache behavior
- [ ] Take screenshots of current responses

### After Migration
- [ ] Compare response times (should be similar or better)
- [ ] Compare error rates (should be same or lower)
- [ ] Verify cache is working (logs show cache hits)
- [ ] Compare response format (should be compatible)

## 🚨 Rollback Plan

If something goes wrong:

1. [ ] Keep old code in a separate branch
2. [ ] Document rollback procedure
3. [ ] Test rollback in staging first
4. [ ] Have database backup ready
5. [ ] Monitor logs during rollback

## ✅ Post-Migration

- [ ] Monitor application logs for errors
- [ ] Monitor cache hit rates
- [ ] Monitor database query performance
- [ ] Gather feedback from frontend team
- [ ] Update documentation with lessons learned
- [ ] Share migration experience with team

## 📝 Notes

### Common Issues
- **Cache not working**: Verify `ICacheService` is registered
- **Missing columns**: Check stored procedure returns all required columns
- **Slow performance**: Add indexes to lookup tables
- **Memory issues**: Use distributed cache instead of in-memory

### Tips
- Start with in-memory cache for simplicity
- Move to distributed cache when scaling
- Use refresh endpoint after data updates
- Monitor cache hit rates in production
- Keep stored procedures simple and fast

## 🎉 Success Criteria

Migration is successful when:
- [x] All tests pass
- [ ] Response times are acceptable
- [ ] Cache is working correctly
- [ ] No errors in logs
- [ ] Frontend works without changes (or minimal changes)
- [ ] Team is trained on new service
- [ ] Documentation is updated

---

**Estimated Time:** 2-4 hours for a single API
**Difficulty:** Medium
**Risk:** Low (backward compatible)

