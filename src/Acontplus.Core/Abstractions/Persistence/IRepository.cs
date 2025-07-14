using Acontplus.Core.Domain.Common.Entities;
using System.Linq.Expressions;

namespace Acontplus.Core.Abstractions.Persistence;

/// <summary>
/// Defines generic data access operations for entities.
/// </summary>
/// TEntity: The type of the entity.
/// TId: The type of the entity's primary key must be not null.
public interface IRepository<TEntity, TId>
    where TEntity : AuditableEntity<TId>
    where TId : notnull
{
    #region Query Methods

    /// <summary>
    /// Retrieves an entity by its primary key.
    /// </summary>
    Task<TEntity> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the first entity matching a predicate, with optional includes.
    /// </summary>
    Task<TEntity> GetFirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includeProperties);

    /// <summary>
    /// Retrieves all entities, with optional includes.
    /// </summary>
    Task<IReadOnlyList<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includeProperties);

    /// <summary>
    /// Finds entities matching a predicate, with optional includes.
    /// </summary>
    Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includeProperties);

    /// <summary>
    /// Gets entities as an IAsyncEnumerable for memory-efficient streaming.
    /// </summary>
    IAsyncEnumerable<TEntity> FindAsyncEnumerable(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paged result of all entities with optional sorting.
    /// </summary>
    Task<PagedResult<TEntity>> GetPagedAsync(
        PaginationDto pagination,
        CancellationToken cancellationToken = default,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool orderByDescending = false);

    /// <summary>
    /// Gets a paged result of filtered entities, with optional includes and sorting.
    /// </summary>
    Task<PagedResult<TEntity>> GetPagedAsync(
        PaginationDto pagination,
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool orderByDescending = false,
        params Expression<Func<TEntity, object>>[] includeProperties);

    /// <summary>
    /// Gets a projected paged result for efficient data transfer.
    /// </summary>
    Task<PagedResult<TProjection>> GetPagedProjectionAsync<TProjection>(
        PaginationDto pagination,
        Expression<Func<TEntity, TProjection>> projection,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool orderByDescending = false);

    /// <summary>
    /// Checks if any entity matches the predicate.
    /// </summary>
    Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts entities matching the predicate.
    /// </summary>
    Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts entities matching the predicate as a long for large datasets.
    /// </summary>
    Task<long> LongCountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Persistence Methods

    /// <summary>
    /// Adds a new entity. Persisted by UnitOfWork.SaveChangesAsync.
    /// </summary>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple entities efficiently. Persisted by UnitOfWork.SaveChangesAsync.
    /// </summary>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an entity for update. Persisted by UnitOfWork.SaveChangesAsync.
    /// </summary>
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks multiple entities for update. Persisted by UnitOfWork.SaveChangesAsync.
    /// </summary>
    Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates specific properties of an entity. Persisted by UnitOfWork.SaveChangesAsync.
    /// </summary>
    Task<TEntity> UpdatePropertiesAsync(
        TEntity entity,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] propertiesToUpdate);

    /// <summary>
    /// Marks an entity for deletion. Persisted by UnitOfWork.SaveChangesAsync.
    /// </summary>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an entity for deletion by ID. Persisted by UnitOfWork.SaveChangesAsync.
    /// </summary>
    Task<bool> DeleteByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks multiple entities for deletion. Persisted by UnitOfWork.SaveChangesAsync.
    /// </summary>
    Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks entities matching a predicate for deletion. Persisted by UnitOfWork.SaveChangesAsync.
    /// </summary>
    Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    #endregion

    #region Bulk Operations

    /// <summary>
    /// Performs a bulk delete directly in the database using ExecuteDeleteAsync.
    /// </summary>
    Task<int> BulkDeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a bulk update directly in the database using ExecuteUpdateAsync.
    /// </summary>
    Task<int> BulkUpdateAsync<TProperty>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TProperty>> propertyExpression,
        TProperty newValue,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a bulk insert operation for better performance with large datasets.
    /// </summary>
    Task<int> BulkInsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    #endregion

    #region Specification Pattern

    /// <summary>
    /// Finds entities using a specification.
    /// </summary>
    Task<IReadOnlyList<TEntity>> FindWithSpecificationAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the first entity using a specification.
    /// </summary>
    Task<TEntity> GetFirstOrDefaultWithSpecificationAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paged result of entities using a specification.
    /// </summary>
    Task<PagedResult<TEntity>> GetPagedWithSpecificationAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a projected result using a specification for efficient data transfer.
    /// </summary>
    Task<IReadOnlyList<TProjection>> FindProjectionWithSpecificationAsync<TProjection>(
        ISpecification<TEntity> specification,
        Expression<Func<TEntity, TProjection>> projection,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts entities using a specification.
    /// </summary>
    Task<int> CountWithSpecificationAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default);

    #endregion

    #region Advanced Query Operations

    /// <summary>
    /// Gets entities with complex ordering options.
    /// </summary>
    Task<IReadOnlyList<TEntity>> GetOrderedAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default,
        params (Expression<Func<TEntity, object>> KeySelector, bool Descending)[] orderExpressions);

    /// <summary>
    /// Performs aggregation operations on entities.
    /// </summary>
    Task<TResult> AggregateAsync<TResult>(
        Expression<Func<IQueryable<TEntity>, TResult>> aggregateExpression,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets distinct values for a specific property.
    /// </summary>
    Task<IReadOnlyList<TProperty>> GetDistinctAsync<TProperty>(
        Expression<Func<TEntity, TProperty>> propertySelector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    #endregion
}