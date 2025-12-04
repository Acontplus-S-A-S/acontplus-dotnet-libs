namespace Acontplus.TestApi.Endpoints.Core;

/// <summary>
/// Minimal API endpoints for lookup/reference data management.
/// Demonstrates the LookupService with caching capabilities.
/// </summary>
public static class LookupEndpoints
{
    public static void MapLookupEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/lookups")
            .WithDescription("Endpoints for managing test API lookups.")
            .WithTags("Lookups");

        group.MapGet("/", async (
            FilterQuery filterQuery,
            ILookupService lookupService,
            CancellationToken cancellationToken) =>
        {
            // ✅ Example: Use GetFilterValue to extract specific filter values with type safety
            var category = filterQuery.GetFilterValue<string>("category");
            var isActive = filterQuery.GetFilterValue<bool>("isActive", true); // with default
            var minPriority = filterQuery.GetFilterValue<int>("minPriority", 1);

            // ✅ Example: Use TryGetFilterValue for safe retrieval
            if (filterQuery.TryGetFilterValue<Guid>("entityId", out var entityId))
            {
                // entityId is available and successfully converted to Guid
                // You could add additional logic here if needed
            }

            var filterRequest = filterQuery.Adapt<FilterRequest>();

            return await lookupService
                .GetLookupsAsync("dbo.GetLookups", filterRequest, cancellationToken)
                .ToGetMinimalApiResultAsync();
        })
        .WithName("GetLookups")
        .WithSummary("Get test API lookups")
        .WithDescription("Retrieve test API lookups based on filters with automatic caching (30-minute TTL). Supports filters: category, isActive, minPriority, entityId.")
        .Produces<IDictionary<string, IEnumerable<LookupItem>>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/refresh", async (
            FilterQuery filterQuery,
            ILookupService lookupService,
            CancellationToken cancellationToken) =>
        {
            // ✅ Example: Extract filter values before mapping
            var forceRefresh = filterQuery.GetFilterValue<bool>("forceRefresh", false);
            var cacheKey = filterQuery.GetFilterValue<string>("cacheKey", "default");

            var filterRequest = filterQuery.Adapt<FilterRequest>();

            return await lookupService
                .RefreshLookupsAsync("dbo.GetLookups", filterRequest, cancellationToken)
                .ToGetMinimalApiResultAsync();
        })
        .WithName("RefreshLookups")
        .WithSummary("Refresh test API lookups cache")
        .WithDescription("Forces a cache refresh for test API lookups based on filters. Supports filters: forceRefresh, cacheKey.")
        .Produces<IDictionary<string, IEnumerable<LookupItem>>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
