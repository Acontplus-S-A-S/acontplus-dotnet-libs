using Acontplus.Core.DTOs.Responses;
using Acontplus.Persistence.SqlServer.Diagnostics;
using Acontplus.Persistence.SqlServer.Exceptions;
using Microsoft.EntityFrameworkCore.Query; // Assuming DiagnosticConfig is here
using System.Linq.Expressions;

namespace Acontplus.Persistence.SqlServer.Repositories;

/// <summary>
/// A modern, generic repository implementation for Entity Framework Core, targeting .NET 9+.
/// </summary>
/// <typeparam name="T">The entity type, which must derive from BaseEntity.</typeparam>
public class BaseRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly DbContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly ILogger<BaseRepository<T>> _logger;

    public BaseRepository(DbContext context, ILogger<BaseRepository<T>> logger = null)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<T>();
        _logger = logger;
    }

    #region Query Methods

    public virtual async Task<T> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(GetByIdAsync)}");
        try
        {
            ArgumentNullException.ThrowIfNull(id);
            return await _dbSet.FindAsync([id], cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error getting entity of type {EntityType} by ID: {Id}", typeof(T).Name, id);
            throw new RepositoryException($"Error getting entity by ID {id}", ex);
        }
    }

    public virtual async Task<T> GetFirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includeProperties)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(GetFirstOrDefaultAsync)}");
        try
        {
            ArgumentNullException.ThrowIfNull(predicate);
            var query = BuildQuery(includeProperties: includeProperties);
            return await query.FirstOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in GetFirstOrDefaultAsync for entity {EntityType}", typeof(T).Name);
            throw new RepositoryException("Error retrieving entity", ex);
        }
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includeProperties)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(GetAllAsync)}");
        try
        {
            var query = BuildQuery(includeProperties: includeProperties);
            return await query.ToListAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in GetAllAsync for entity {EntityType}", typeof(T).Name);
            throw new RepositoryException("Error retrieving all entities", ex);
        }
    }

    public virtual async Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includeProperties)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(FindAsync)}");
        try
        {
            ArgumentNullException.ThrowIfNull(predicate);
            var query = BuildQuery(includeProperties: includeProperties);
            return await query.Where(predicate).ToListAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in FindAsync for entity {EntityType}", typeof(T).Name);
            throw new RepositoryException("Error finding entities", ex);
        }
    }

    public virtual IAsyncEnumerable<T> FindAsyncEnumerable(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(FindAsyncEnumerable)}");
        try
        {
            ArgumentNullException.ThrowIfNull(predicate);
            return _dbSet.Where(predicate).AsNoTracking().AsAsyncEnumerable();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in FindAsyncEnumerable for entity {EntityType}", typeof(T).Name);
            throw new RepositoryException("Error finding entities as async enumerable", ex);
        }
    }

    public virtual Task<PagedResult<T>> GetPagedAsync(
        PaginationDto pagination,
        CancellationToken cancellationToken = default,
        Expression<Func<T, object>> orderBy = null,
        bool orderByDescending = false)
    {
        return GetPagedAsync(pagination, null!, cancellationToken, orderBy, orderByDescending);
    }

    public virtual async Task<PagedResult<T>> GetPagedAsync(
        PaginationDto pagination,
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default,
        Expression<Func<T, object>> orderBy = null,
        bool orderByDescending = false,
        params Expression<Func<T, object>>[] includeProperties)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(GetPagedAsync)}");
        try
        {
            ValidatePagination(pagination);
            var query = BuildQuery(tracking: false, includeProperties: includeProperties);

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

            if (orderBy != null)
            {
                query = orderByDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
            }
            else // Default sorting by ID if not specified
            {
                query = query.OrderBy(e => e.Id);
            }

            var items = await query
                .Skip((pagination.PageIndex - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new PagedResult<T>(items, pagination.PageIndex, pagination.PageSize, totalCount);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in GetPagedAsync for entity {EntityType}", typeof(T).Name);
            throw new RepositoryException("Error retrieving paged results", ex);
        }
    }

    public virtual async Task<PagedResult<TProjection>> GetPagedProjectionAsync<TProjection>(
        PaginationDto pagination,
        Expression<Func<T, TProjection>> projection,
        Expression<Func<T, bool>> predicate = null,
        CancellationToken cancellationToken = default,
        Expression<Func<T, object>> orderBy = null,
        bool orderByDescending = false)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(GetPagedProjectionAsync)}");
        try
        {
            ValidatePagination(pagination);
            ArgumentNullException.ThrowIfNull(projection);

            var query = _dbSet.AsNoTracking();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

            if (orderBy != null)
            {
                query = orderByDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
            }
            else // Default sorting by ID if not specified
            {
                query = query.OrderBy(e => e.Id);
            }

            var items = await query
                .Skip((pagination.PageIndex - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(projection)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new PagedResult<TProjection>(items, pagination.PageIndex, pagination.PageSize, totalCount);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in GetPagedProjectionAsync for entity {EntityType}", typeof(T).Name);
            throw new RepositoryException("Error retrieving paged projected results", ex);
        }
    }

    public virtual async Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(ExistsAsync)}");
        try
        {
            ArgumentNullException.ThrowIfNull(predicate);
            return await _dbSet.AnyAsync(predicate, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in ExistsAsync for entity {EntityType}", typeof(T).Name);
            throw new RepositoryException("Error checking entity existence", ex);
        }
    }

    public virtual async Task<int> CountAsync(
        Expression<Func<T, bool>> predicate = null,
        CancellationToken cancellationToken = default)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(CountAsync)}");
        try
        {
            return predicate == null
                ? await _dbSet.CountAsync(cancellationToken).ConfigureAwait(false)
                : await _dbSet.CountAsync(predicate, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in CountAsync for entity {EntityType}", typeof(T).Name);
            throw new RepositoryException("Error counting entities", ex);
        }
    }

    public virtual async Task<long> LongCountAsync(
        Expression<Func<T, bool>> predicate = null,
        CancellationToken cancellationToken = default)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(LongCountAsync)}");
        try
        {
            return predicate == null
                ? await _dbSet.LongCountAsync(cancellationToken).ConfigureAwait(false)
                : await _dbSet.LongCountAsync(predicate, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in LongCountAsync for entity {EntityType}", typeof(T).Name);
            throw new RepositoryException("Error counting entities", ex);
        }
    }

    #endregion

    #region Persistence Methods

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(AddAsync)}");
        try
        {
            ArgumentNullException.ThrowIfNull(entity);
            var entry = await _dbSet.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            return entry.Entity;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error adding entity of type {EntityType}", typeof(T).Name);
            throw new RepositoryException("Error adding entity", ex);
        }
    }

    public virtual Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(AddRangeAsync)}");
        try
        {
            ArgumentNullException.ThrowIfNull(entities);
            return _dbSet.AddRangeAsync(entities, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error adding entity range of type {EntityType}", typeof(T).Name);
            throw new RepositoryException("Error adding entity range", ex);
        }
    }

    public virtual Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(UpdateAsync)}");
        try
        {
            ArgumentNullException.ThrowIfNull(entity);
            var entry = _context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            entry.State = EntityState.Modified;
            return Task.FromResult(entity);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating entity of type {EntityType}", typeof(T).Name);
            throw new RepositoryException("Error updating entity", ex);
        }
    }

    public virtual Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(UpdateRangeAsync)}");
        try
        {
            ArgumentNullException.ThrowIfNull(entities);
            var entityList = entities.ToList();
            if (!entityList.Any()) return Task.FromResult(Enumerable.Empty<T>());

            _dbSet.UpdateRange(entityList);
            return Task.FromResult<IEnumerable<T>>(entityList);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating entity range of type {EntityType}", typeof(T).Name);
            throw new RepositoryException("Error updating entity range", ex);
        }
    }

    public virtual Task<T> UpdatePropertiesAsync(
        T entity,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] propertiesToUpdate)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(UpdatePropertiesAsync)}");
        try
        {
            ArgumentNullException.ThrowIfNull(entity);

            var entry = _context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }

            if (propertiesToUpdate is { Length: > 0 })
            {
                entry.State = EntityState.Unchanged;
                foreach (var property in propertiesToUpdate)
                {
                    entry.Property(property).IsModified = true;
                }
            }
            else
            {
                entry.State = EntityState.Modified;
            }

            return Task.FromResult(entity);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating specific properties for entity {EntityType}", typeof(T).Name);
            throw new RepositoryException("Error performing partial update on entity.", ex);
        }
    }

    public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(DeleteAsync)}");
        try
        {
            ArgumentNullException.ThrowIfNull(entity);
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error deleting entity of type {EntityType}", typeof(T).Name);
            throw new RepositoryException("Error deleting entity", ex);
        }
    }

    public virtual async Task<bool> DeleteByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(DeleteByIdAsync)}");
        try
        {
            ArgumentNullException.ThrowIfNull(id);
            var entity = await _dbSet.FindAsync([id], cancellationToken).ConfigureAwait(false);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error deleting entity of type {EntityType} by ID: {Id}", typeof(T).Name, id);
            throw new RepositoryException($"Error deleting entity by ID {id}", ex);
        }
    }

    public virtual Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(DeleteRangeAsync)}");
        try
        {
            ArgumentNullException.ThrowIfNull(entities);
            _dbSet.RemoveRange(entities);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error deleting entity range of type {EntityType}", typeof(T).Name);
            throw new RepositoryException("Error deleting entity range", ex);
        }
    }

    public virtual async Task<int> DeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(DeleteAsync)}_Predicate");
        try
        {
            ArgumentNullException.ThrowIfNull(predicate);
            var entities = await _dbSet.Where(predicate).ToListAsync(cancellationToken).ConfigureAwait(false);
            if (entities.Count > 0)
            {
                _dbSet.RemoveRange(entities);
            }
            return entities.Count;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error deleting entities by predicate for type {EntityType}", typeof(T).Name);
            throw new RepositoryException("Error deleting entities by predicate", ex);
        }
    }

    #endregion

    #region Bulk Operations

    public virtual async Task<int> BulkDeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(BulkDeleteAsync)}");
        try
        {
            ArgumentNullException.ThrowIfNull(predicate);
            return await _dbSet
                .Where(predicate)
                .ExecuteDeleteAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in BulkDeleteAsync for entity {EntityType}", typeof(T).Name);
            throw new RepositoryException("Error performing bulk delete", ex);
        }
    }

    public virtual async Task<int> BulkUpdateAsync<TProperty>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TProperty>> propertyExpression,
        TProperty newValue,
        CancellationToken cancellationToken = default)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(BulkUpdateAsync)}");
        try
        {
            ArgumentNullException.ThrowIfNull(predicate);
            ArgumentNullException.ThrowIfNull(propertyExpression);

            var setPropertyCalls = Expression.Lambda<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>(
                Expression.Call(
                    typeof(SetPropertyCalls<T>).GetMethods()
                        .Single(m => m.Name == nameof(SetPropertyCalls<T>.SetProperty) && m.IsGenericMethod && m.GetParameters().Length == 2 && m.GetParameters()[0].ParameterType.GetGenericArguments().Length == 2)
                        .MakeGenericMethod(typeof(TProperty)),
                    Expression.Parameter(typeof(SetPropertyCalls<T>), "s"),
                    propertyExpression,
                    Expression.Constant(newValue, typeof(TProperty))
                ),
                Expression.Parameter(typeof(SetPropertyCalls<T>), "s")
            );

            return await _dbSet
                .Where(predicate)
                .ExecuteUpdateAsync(setPropertyCalls, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in BulkUpdateAsync for entity {EntityType}", typeof(T).Name);
            throw new RepositoryException("Error performing bulk update", ex);
        }
    }

    public virtual async Task<int> BulkInsertAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(BulkInsertAsync)}");
        try
        {
            ArgumentNullException.ThrowIfNull(entities);
            var entityList = entities.ToList();
            if (entityList.Count == 0) return 0;

            await _dbSet.AddRangeAsync(entityList, cancellationToken).ConfigureAwait(false);
            // Bulk insert is typically a "unit of work" in itself, so SaveChanges might be called here
            // or by the UnitOfWork. For this implementation, we assume UoW handles it.
            return entityList.Count;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in BulkInsertAsync for entity {EntityType}", typeof(T).Name);
            throw new RepositoryException("Error performing bulk insert", ex);
        }
    }

    #endregion

    #region Specification Pattern

    public virtual async Task<IReadOnlyList<T>> FindWithSpecificationAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken = default)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(FindWithSpecificationAsync)}");
        try
        {
            ArgumentNullException.ThrowIfNull(specification);
            return await BuildSpecificationQuery(specification).ToListAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in FindWithSpecificationAsync for {EntityType}", typeof(T).Name);
            throw new RepositoryException("Error retrieving entities with specification", ex);
        }
    }

    public virtual async Task<T> GetFirstOrDefaultWithSpecificationAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken = default)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(GetFirstOrDefaultWithSpecificationAsync)}");
        try
        {
            ArgumentNullException.ThrowIfNull(specification);
            return await BuildSpecificationQuery(specification).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in GetFirstOrDefaultWithSpecificationAsync for {EntityType}", typeof(T).Name);
            throw new RepositoryException("Error retrieving entity with specification", ex);
        }
    }

    public virtual async Task<PagedResult<T>> GetPagedWithSpecificationAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken = default)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(GetPagedWithSpecificationAsync)}");
        try
        {
            ArgumentNullException.ThrowIfNull(specification);
            ValidatePagination(specification.Pagination);

            var query = BuildSpecificationQuery(specification, ignorePaging: true);
            var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

            var pagedQuery = ApplyPaging(query, specification);
            var items = await pagedQuery.ToListAsync(cancellationToken).ConfigureAwait(false);

            return new PagedResult<T>(items, specification.Pagination.PageIndex, specification.Pagination.PageSize, totalCount);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in GetPagedWithSpecificationAsync for {EntityType}", typeof(T).Name);
            throw new RepositoryException("Error retrieving paged results with specification", ex);
        }
    }

    public virtual async Task<IReadOnlyList<TProjection>> FindProjectionWithSpecificationAsync<TProjection>(
        ISpecification<T> specification,
        Expression<Func<T, TProjection>> projection,
        CancellationToken cancellationToken = default)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(FindProjectionWithSpecificationAsync)}");
        try
        {
            ArgumentNullException.ThrowIfNull(specification);
            ArgumentNullException.ThrowIfNull(projection);

            var query = BuildSpecificationQuery(specification).Select(projection);
            return await query.ToListAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in FindProjectionWithSpecificationAsync for {EntityType}", typeof(T).Name);
            throw new RepositoryException("Error retrieving projected results with specification", ex);
        }
    }

    public virtual async Task<int> CountWithSpecificationAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken = default)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(CountWithSpecificationAsync)}");
        try
        {
            ArgumentNullException.ThrowIfNull(specification);
            var query = BuildSpecificationQuery(specification, ignorePaging: true, ignoreOrdering: true);
            return await query.CountAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in CountWithSpecificationAsync for {EntityType}", typeof(T).Name);
            throw new RepositoryException("Error counting entities with specification", ex);
        }
    }

    #endregion

    #region Advanced Query Operations

    public virtual async Task<IReadOnlyList<T>> GetOrderedAsync(
        Expression<Func<T, bool>> predicate = null,
        CancellationToken cancellationToken = default,
        params (Expression<Func<T, object>> KeySelector, bool Descending)[] orderExpressions)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(GetOrderedAsync)}");
        try
        {
            var query = _dbSet.AsNoTracking();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            query = ApplyOrdering(query, orderExpressions);

            return await query.ToListAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in GetOrderedAsync for entity {EntityType}", typeof(T).Name);
            throw new RepositoryException("Error retrieving ordered entities", ex);
        }
    }

    public virtual async Task<TResult> AggregateAsync<TResult>(
        Expression<Func<IQueryable<T>, TResult>> aggregateExpression,
        Expression<Func<T, bool>> predicate = null,
        CancellationToken cancellationToken = default)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(AggregateAsync)}");
        try
        {
            ArgumentNullException.ThrowIfNull(aggregateExpression);

            var query = _dbSet.AsNoTracking();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            // This is a bit complex. The expression needs to be invoked.
            // EF Core can often translate this if the expression is simple enough (e.g., q => q.Sum(e => e.Property))
            // This approach provides maximum flexibility.
            var compiledAggregate = aggregateExpression.Compile();
            return compiledAggregate(query);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in AggregateAsync for entity {EntityType}", typeof(T).Name);
            throw new RepositoryException("Error performing aggregation", ex);
        }
    }

    public virtual async Task<IReadOnlyList<TProperty>> GetDistinctAsync<TProperty>(
        Expression<Func<T, TProperty>> propertySelector,
        Expression<Func<T, bool>> predicate = null,
        CancellationToken cancellationToken = default)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(GetDistinctAsync)}");
        try
        {
            ArgumentNullException.ThrowIfNull(propertySelector);

            var query = _dbSet.AsNoTracking();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.Select(propertySelector).Distinct().ToListAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in GetDistinctAsync for entity {EntityType}", typeof(T).Name);
            throw new RepositoryException("Error retrieving distinct values", ex);
        }
    }

    #endregion

    #region Helper Methods

    protected virtual IQueryable<T> BuildQuery(
        bool tracking = true,
        params Expression<Func<T, object>>[] includeProperties)
    {
        var query = tracking ? _dbSet.AsQueryable() : _dbSet.AsNoTracking();

        if (includeProperties is { Length: > 0 })
        {
            query = includeProperties.Aggregate(query, (current, include) => current.Include(include));
        }

        return query;
    }

    protected virtual IQueryable<T> BuildSpecificationQuery(ISpecification<T> spec, bool ignorePaging = false, bool ignoreOrdering = false)
    {
        var query = spec.IsTrackingEnabled ? _dbSet.AsQueryable() : _dbSet.AsNoTracking();

        if (spec.Criteria != null)
        {
            query = query.Where(spec.Criteria);
        }

        query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
        query = spec.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

        if (!ignoreOrdering && spec.OrderByExpressions.Count > 0)
        {
            query = ApplyOrdering(query, spec.OrderByExpressions.Select(o => (o.Expression, o.IsDescending)).ToArray());
        }

        if (!ignorePaging && spec.IsPagingEnabled)
        {
            query = ApplyPaging(query, spec);
        }

        return query;
    }

    protected virtual IQueryable<T> ApplyPaging(IQueryable<T> query, ISpecification<T> spec)
    {
        return query.Skip((spec.Pagination.PageIndex - 1) * spec.Pagination.PageSize)
                    .Take(spec.Pagination.PageSize);
    }

    protected virtual IQueryable<T> ApplyOrdering(IQueryable<T> query, params (Expression<Func<T, object>> KeySelector, bool Descending)[] orderExpressions)
    {
        if (orderExpressions is null || orderExpressions.Length == 0)
        {
            return query.OrderBy(e => e.Id); // Default order
        }

        IOrderedQueryable<T> orderedQuery = null;
        foreach (var (keySelector, descending) in orderExpressions)
        {
            if (orderedQuery is null)
            {
                orderedQuery = descending
                    ? query.OrderByDescending(keySelector)
                    : query.OrderBy(keySelector);
            }
            else
            {
                orderedQuery = descending
                    ? orderedQuery.ThenByDescending(keySelector)
                    : orderedQuery.ThenBy(keySelector);
            }
        }
        return orderedQuery ?? query;
    }

    protected virtual void ValidatePagination(PaginationDto pagination)
    {
        ArgumentNullException.ThrowIfNull(pagination);
        if (pagination.PageIndex < 1)
            throw new ArgumentException("Page index must be greater than 0.", nameof(pagination.PageIndex));
        if (pagination.PageSize < 1 || pagination.PageSize > 500) // Max page size guard
            throw new ArgumentException("Page size must be between 1 and 500.", nameof(pagination.PageSize));
    }

    #endregion
}
