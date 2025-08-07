using Acontplus.Core.Domain.Common.Entities;
using System.Linq.Expressions;

namespace Acontplus.Core.Abstractions.Persistence;

public interface IRepository<TEntity>
    where TEntity : BaseEntity
{
    #region Query Methods

    Task<TEntity> GetByIdAsync(int id, CancellationToken cancellationToken = default);

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

    IAsyncEnumerable<TEntity> FindAsyncEnumerable(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<PagedResult<TEntity>> GetPagedAsync(
        PaginationDto pagination,
        CancellationToken cancellationToken = default,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool orderByDescending = false);

    Task<PagedResult<TEntity>> GetPagedAsync(
        PaginationDto pagination,
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool orderByDescending = false,
        params Expression<Func<TEntity, object>>[] includeProperties);

    Task<PagedResult<TProjection>> GetPagedProjectionAsync<TProjection>(
        PaginationDto pagination,
        Expression<Func<TEntity, TProjection>> projection,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool orderByDescending = false);

    Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task<long> LongCountAsync(
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

    #endregion

    #region Audit Methods
    Task SoftDeleteAsync(TEntity entity, int? deletedByUserId = default, CancellationToken cancellationToken = default);

    Task RestoreAsync(TEntity entity, int? restoredByUserId = default, CancellationToken cancellationToken = default);
    #endregion
}
