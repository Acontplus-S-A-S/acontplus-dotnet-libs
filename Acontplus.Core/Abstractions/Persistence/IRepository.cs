using Acontplus.Core.Base;
using System.Linq.Expressions;

namespace Acontplus.Core.Abstractions.Persistence;

/// <summary>
/// Defines generic data access operations for entities.
/// </summary>
/// <typeparam name="T">The entity type, must derive from BaseEntity.</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    #region Query Methods

    /// <summary>
    /// Retrieves an entity by its primary key.
    /// </summary>
    Task<T> GetByIdAsync(object id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the first entity matching a predicate, with optional includes.
    /// </summary>
    Task<T> GetFirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includeProperties);

    /// <summary>
    /// Retrieves all entities, with optional includes.
    /// </summary>
    Task<IReadOnlyList<T>> GetAllAsync(
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includeProperties);

    /// <summary>
    /// Finds entities matching a predicate, with optional includes.
    /// </summary>
    Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includeProperties);

    /// <summary>
    /// Gets entities as an IAsyncEnumerable for memory-efficient streaming.
    /// </summary>
    IAsyncEnumerable<T> FindAsyncEnumerable(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paged result of all entities with optional sorting.
    /// </summary>
    Task<PagedResult<T>> GetPagedAsync(
        PaginationDto pagination,
        CancellationToken cancellationToken = default,
        Expression<Func<T, object>> orderBy = null,
        bool orderByDescending = false);

    /// <summary>
    /// Gets a paged result of filtered entities, with optional includes and sorting.
    /// </summary>
    Task<PagedResult<T>> GetPagedAsync(
        PaginationDto pagination,
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default,
        Expression<Func<T, object>> orderBy = null,
        bool orderByDescending = false,
        params Expression<Func<T, object>>[] includeProperties);

    /// <summary>
    /// Gets a projected paged result for efficient data transfer.
    /// </summary>
    Task<PagedResult<TProjection>> GetPagedProjectionAsync<TProjection>(
        PaginationDto pagination,
        Expression<Func<T, TProjection>> projection,
        Expression<Func<T, bool>> predicate = null,
        CancellationToken cancellationToken = default,
        Expression<Func<T, object>> orderBy = null,
        bool orderByDescending = false);

    /// <summary>
    /// Checks if any entity matches the predicate.
    /// </summary>
    Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts entities matching the predicate.
    /// </summary>
    Task<int> CountAsync(
        Expression<Func<T, bool>> predicate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts entities matching the predicate as a long for large datasets.
    /// </summary>
    Task<long> LongCountAsync(
        Expression<Func<T, bool>> predicate = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Persistence Methods

    /// <summary>
    /// Adds a new entity. Persisted by UnitOfWork.SaveChangesAsync.
    /// </summary>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple entities efficiently. Persisted by UnitOfWork.SaveChangesAsync.
    /// </summary>
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an entity for update. Persisted by UnitOfWork.SaveChangesAsync.
    /// </summary>
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks multiple entities for update. Persisted by UnitOfWork.SaveChangesAsync.
    /// </summary>
    Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates specific properties of an entity. Persisted by UnitOfWork.SaveChangesAsync.
    /// </summary>
    Task<T> UpdatePropertiesAsync(
        T entity,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] propertiesToUpdate);

    /// <summary>
    /// Marks an entity for deletion. Persisted by UnitOfWork.SaveChangesAsync.
    /// </summary>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an entity for deletion by ID. Persisted by UnitOfWork.SaveChangesAsync.
    /// </summary>
    Task<bool> DeleteByIdAsync(object id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks multiple entities for deletion. Persisted by UnitOfWork.SaveChangesAsync.
    /// </summary>
    Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks entities matching a predicate for deletion. Persisted by UnitOfWork.SaveChangesAsync.
    /// </summary>
    Task<int> DeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    #endregion

    #region Bulk Operations

    /// <summary>
    /// Performs a bulk delete directly in the database using ExecuteDeleteAsync.
    /// </summary>
    Task<int> BulkDeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a bulk update directly in the database using ExecuteUpdateAsync.
    /// </summary>
    Task<int> BulkUpdateAsync<TProperty>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, TProperty>> propertyExpression,
            TProperty newValue,
            CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a bulk insert operation for better performance with large datasets.
    /// </summary>
    Task<int> BulkInsertAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    #endregion

    #region Specification Pattern

    /// <summary>
    /// Finds entities using a specification.
    /// </summary>
    Task<IReadOnlyList<T>> FindWithSpecificationAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the first entity using a specification.
    /// </summary>
    Task<T> GetFirstOrDefaultWithSpecificationAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paged result of entities using a specification.
    /// </summary>
    Task<PagedResult<T>> GetPagedWithSpecificationAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a projected result using a specification for efficient data transfer.
    /// </summary>
    Task<IReadOnlyList<TProjection>> FindProjectionWithSpecificationAsync<TProjection>(
        ISpecification<T> specification,
        Expression<Func<T, TProjection>> projection,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts entities using a specification.
    /// </summary>
    Task<int> CountWithSpecificationAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken = default);

    #endregion

    #region Advanced Query Operations

    /// <summary>
    /// Gets entities with complex ordering options.
    /// </summary>
    Task<IReadOnlyList<T>> GetOrderedAsync(
        Expression<Func<T, bool>> predicate = null,
        CancellationToken cancellationToken = default,
        params (Expression<Func<T, object>> KeySelector, bool Descending)[] orderExpressions);

    /// <summary>
    /// Performs aggregation operations on entities.
    /// </summary>
    Task<TResult> AggregateAsync<TResult>(
        Expression<Func<IQueryable<T>, TResult>> aggregateExpression,
        Expression<Func<T, bool>> predicate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets distinct values for a specific property.
    /// </summary>
    Task<IReadOnlyList<TProperty>> GetDistinctAsync<TProperty>(
        Expression<Func<T, TProperty>> propertySelector,
        Expression<Func<T, bool>> predicate = null,
        CancellationToken cancellationToken = default);

    #endregion
}
