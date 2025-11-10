using System.Data.Common;

namespace Acontplus.Core.Abstractions.Persistence;

/// <summary>
/// ADO.NET repository interface for high-performance, raw SQL operations.
/// Optimized for scalable, high-workload scenarios with SQL Server and PostgreSQL.
/// </summary>
public interface IAdoRepository
{
    #region Basic Query Methods

    /// <summary>
    /// Executes a SQL query and maps results to a list of objects.
    /// </summary>
    Task<List<T>> QueryAsync<T>(
        string sql,
        Dictionary<string, object>? parameters = null,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a SQL query and returns a DataSet with multiple tables.
    /// </summary>
    Task<DataSet> GetDataSetAsync(
        string sql,
        Dictionary<string, object>? parameters = null,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a non-query SQL command (INSERT, UPDATE, DELETE).
    /// Returns the number of affected rows.
    /// </summary>
    Task<int> ExecuteNonQueryAsync(
        string sql,
        Dictionary<string, object>? parameters = null,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a SQL query designed to return a single row or null.
    /// </summary>
    Task<T?> QuerySingleOrDefaultAsync<T>(
        string sql,
        Dictionary<string, object>? parameters = null,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default)
        where T : class;

    /// <summary>
    /// Executes a SQL query and returns the first row or null.
    /// </summary>
    Task<T?> QueryFirstOrDefaultAsync<T>(
        string sql,
        Dictionary<string, object>? parameters = null,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default)
        where T : class;

    #endregion

    #region Scalar Query Methods

    /// <summary>
    /// Executes a query and returns a single scalar value (e.g., COUNT, MAX, SUM).
    /// </summary>
    Task<TScalar?> ExecuteScalarAsync<TScalar>(
        string sql,
        Dictionary<string, object>? parameters = null,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if any rows exist for the given query.
    /// </summary>
    Task<bool> ExistsAsync(
        string sql,
        Dictionary<string, object>? parameters = null,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of rows for the given query.
    /// </summary>
    Task<int> CountAsync(
        string sql,
        Dictionary<string, object>? parameters = null,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the long count of rows for the given query (for tables with billions of rows).
    /// </summary>
    Task<long> LongCountAsync(
        string sql,
        Dictionary<string, object>? parameters = null,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Paged Query Methods

    /// <summary>
    /// Executes a paginated SQL query with automatic count query.
    /// For SQL Server: Uses OFFSET-FETCH with optimized query plans.
    /// For PostgreSQL: Uses LIMIT-OFFSET with parallel query support.
    /// Filters and parameters are extracted from the PaginationRequest.Filters property.
    /// </summary>
    /// <param name="sql">Main SELECT query (without OFFSET/LIMIT)</param>
    /// <param name="pagination">Pagination parameters including sort, filters, and search</param>
    /// <param name="options">Command options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<PagedResult<T>> GetPagedAsync<T>(
        string sql,
        PaginationRequest pagination,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a paginated SQL query with a custom count query for complex scenarios.
    /// Use this when the count query needs to be different from the main query.
    /// Filters and parameters are extracted from the PaginationRequest.Filters property.
    /// </summary>
    Task<PagedResult<T>> GetPagedAsync<T>(
        string sql,
        string countSql,
        PaginationRequest pagination,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a paginated stored procedure that returns results and total count.
    /// Expects the stored procedure to have @PageIndex, @PageSize parameters
    /// and return both results and total count (via output parameter or second result set).
    /// Filters and parameters are extracted from the PaginationRequest.Filters property.
    /// </summary>
    Task<PagedResult<T>> GetPagedFromStoredProcedureAsync<T>(
        string storedProcedureName,
        PaginationRequest pagination,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Batch Operations (High-Performance)

    /// <summary>
    /// Executes multiple queries in a single batch for better performance.
    /// Returns multiple result sets.
    /// </summary>
    Task<List<List<T>>> QueryMultipleAsync<T>(
        string sql,
        Dictionary<string, object>? parameters = null,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes multiple non-query commands in a batch.
    /// Returns total affected rows.
    /// </summary>
    Task<int> ExecuteBatchNonQueryAsync(
        IEnumerable<(string Sql, Dictionary<string, object>? Parameters)> commands,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Bulk insert data using optimized bulk copy operations.
    /// For SQL Server: Uses SqlBulkCopy.
    /// For PostgreSQL: Uses COPY command.
    /// </summary>
    Task<int> BulkInsertAsync<T>(
        IEnumerable<T> data,
        string tableName,
        Dictionary<string, string>? columnMappings = null,
        int batchSize = 10000,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Bulk insert data from a DataTable using optimized bulk copy operations.
    /// </summary>
    Task<int> BulkInsertAsync(
        DataTable dataTable,
        string tableName,
        Dictionary<string, string>? columnMappings = null,
        int batchSize = 10000,
        CancellationToken cancellationToken = default);

    #endregion

    #region Streaming and Async Enumerable (Memory-Efficient)

    /// <summary>
    /// Streams query results as an async enumerable for memory-efficient processing.
    /// Ideal for processing large datasets without loading all into memory.
    /// </summary>
    IAsyncEnumerable<T> QueryAsyncEnumerable<T>(
        string sql,
        Dictionary<string, object>? parameters = null,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Transaction Support

    /// <summary>
    /// Sets the current database transaction from the Unit of Work.
    /// </summary>
    void SetTransaction(DbTransaction transaction);

    /// <summary>
    /// Sets the current database connection from the Unit of Work.
    /// </summary>
    void SetConnection(DbConnection connection);

    /// <summary>
    /// Clears the current transaction and connection.
    /// </summary>
    void ClearTransaction();

    #endregion
}
