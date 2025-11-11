namespace Acontplus.Core.Abstractions.Persistence;

public interface IRepository<TEntity>
    where TEntity : class
{
    #region Query Methods

    Task<TEntity> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdOrDefaultAsync(int id, CancellationToken cancellationToken = default);

    Task<TEntity> GetFirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includeProperties);

    Task<IReadOnlyList<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includeProperties);

    Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includeProperties);

    Task<TEntity?> FindSingleOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includeProperties);

    Task<TEntity> FindSingleAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includeProperties);

    IAsyncEnumerable<TEntity> FindAsyncEnumerable(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<PagedResult<TEntity>> GetPagedAsync(
        PaginationRequest pagination,
        CancellationToken cancellationToken = default,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool orderByDescending = false);

    Task<PagedResult<TEntity>> GetPagedAsync(
        PaginationRequest pagination,
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool orderByDescending = false,
        params Expression<Func<TEntity, object>>[] includeProperties);

    Task<PagedResult<TProjection>> GetPagedProjectionAsync<TProjection>(
        PaginationRequest pagination,
        Expression<Func<TEntity, TProjection>> projection,
        Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool orderByDescending = false,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task<long> LongCountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TEntity>> GetByIdsAsync(
        IEnumerable<int> ids,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includeProperties);

    Task<TProperty?> GetMaxAsync<TProperty>(
        Expression<Func<TEntity, TProperty>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task<TProperty?> GetMinAsync<TProperty>(
        Expression<Func<TEntity, TProperty>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task<decimal> GetSumAsync(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task<double> GetAverageAsync(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Persistence Methods

    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default);

    Task<TEntity> UpdatePropertiesAsync(
        TEntity entity,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] propertiesToUpdate);

    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default);

    Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    #endregion

    #region Bulk Operations

    Task<int> BulkDeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    Task<int> BulkUpdateAsync<TProperty>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TProperty>> propertyExpression,
        TProperty newValue,
        CancellationToken cancellationToken = default);

    Task<int> BulkInsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    Task<int> BulkUpdateAsync(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TEntity>> updateExpression,
        CancellationToken cancellationToken = default);

    #endregion

    #region Specification Pattern

    Task<IReadOnlyList<TEntity>> FindWithSpecificationAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default);

    Task<TEntity> GetFirstOrDefaultWithSpecificationAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default);

    Task<PagedResult<TEntity>> GetPagedWithSpecificationAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TProjection>> FindProjectionWithSpecificationAsync<TProjection>(
        ISpecification<TEntity> specification,
        Expression<Func<TEntity, TProjection>> projection,
        CancellationToken cancellationToken = default);

    Task<int> CountWithSpecificationAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default);

    #endregion

    #region Advanced Query Operations

    /// <summary>
    /// Gets a queryable for building complex queries with joins, custom projections, and advanced filtering.
    /// Use this for scenarios that can't be handled by the standard repository methods.
    /// </summary>
    /// <param name="tracking">Whether to enable change tracking</param>
    /// <param name="includeProperties">Navigation properties to include</param>
    /// <returns>A queryable for building complex queries</returns>
    IQueryable<TEntity> GetQueryable(
        bool tracking = false,
        params Expression<Func<TEntity, object>>[] includeProperties);

    /// <summary>
    /// Executes a custom query expression and returns the results.
    /// Use this for complex queries that require custom logic.
    /// </summary>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <param name="queryExpression">The custom query expression</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The query results</returns>
    Task<TResult> ExecuteQueryAsync<TResult>(
        Expression<Func<IQueryable<TEntity>, TResult>> queryExpression,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a custom query expression and returns the results as a list.
    /// Use this for complex queries that return collections.
    /// </summary>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <param name="queryExpression">The custom query expression</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The query results as a list</returns>
    Task<IReadOnlyList<TResult>> ExecuteQueryToListAsync<TResult>(
        Expression<Func<IQueryable<TEntity>, IQueryable<TResult>>> queryExpression,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a custom query with pagination and returns paged results.
    /// Use this for complex queries that need pagination.
    /// </summary>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <param name="queryExpression">The custom query expression</param>
    /// <param name="pagination">Pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paged query results</returns>
    Task<PagedResult<TResult>> ExecutePagedQueryAsync<TResult>(
        Expression<Func<IQueryable<TEntity>, IQueryable<TResult>>> queryExpression,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TEntity>> GetOrderedAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default,
        params (Expression<Func<TEntity, object>> KeySelector, bool Descending)[] orderExpressions);

    Task<TResult> AggregateAsync<TResult>(
        Expression<Func<IQueryable<TEntity>, TResult>> aggregateExpression,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TProperty>> GetDistinctAsync<TProperty>(
        Expression<Func<TEntity, TProperty>> propertySelector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TProjection>> GetProjectionAsync<TProjection>(
        Expression<Func<TEntity, TProjection>> projection,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includeProperties);

    Task<TProjection?> GetFirstProjectionOrDefaultAsync<TProjection>(
        Expression<Func<TEntity, TProjection>> projection,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includeProperties);

    #endregion



    #region Transaction Support

    Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<Task<TResult>> operation,
        CancellationToken cancellationToken = default);

    Task ExecuteInTransactionAsync(
        Func<Task> operation,
        CancellationToken cancellationToken = default);
    #endregion
}
