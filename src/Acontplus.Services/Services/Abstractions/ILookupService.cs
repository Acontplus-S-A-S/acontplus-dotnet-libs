namespace Acontplus.Services.Services.Abstractions;

/// <summary>
/// Service for managing and caching lookup data from database queries.
/// Supports flexible filtering and automatic cache management.
/// </summary>
public interface ILookupService
{
    /// <summary>
    /// Gets lookup data with caching support.
    /// </summary>
    /// <param name="storedProcedureName">Name of the stored procedure to execute.</param>
    /// <param name="filterRequest">Filter parameters for the query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Dictionary of lookup items grouped by table name.</returns>
    Task<Result<IDictionary<string, IEnumerable<LookupItem>>, DomainError>> GetLookupsAsync(
        string storedProcedureName,
        FilterRequest filterRequest,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes lookup data by clearing cache and fetching fresh data.
    /// </summary>
    /// <param name="storedProcedureName">Name of the stored procedure to execute.</param>
    /// <param name="filterRequest">Filter parameters for the query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Dictionary of lookup items grouped by table name.</returns>
    Task<Result<IDictionary<string, IEnumerable<LookupItem>>, DomainError>> RefreshLookupsAsync(
        string storedProcedureName,
        FilterRequest filterRequest,
        CancellationToken cancellationToken = default);
}
