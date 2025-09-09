using Acontplus.Core.Domain.Common.Entities;

namespace Acontplus.Core.Extensions;

/// <summary>
/// Extension methods for audit functionality on repositories with BaseEntity types.
/// </summary>
public static class RepositoryAuditExtensions
{
    /// <summary>
    /// Soft deletes an entity by marking it as deleted.
    /// </summary>
    public static async Task SoftDeleteAsync<TEntity>(
        this IRepository<TEntity> repository,
        TEntity entity,
        int? deletedByUserId = default,
        CancellationToken cancellationToken = default)
        where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(entity);

        entity.MarkAsDeleted(deletedByUserId);
        await repository.UpdateAsync(entity, cancellationToken);
    }

    /// <summary>
    /// Restores a soft-deleted entity.
    /// </summary>
    public static async Task RestoreAsync<TEntity>(
        this IRepository<TEntity> repository,
        TEntity entity,
        int? restoredByUserId = default,
        CancellationToken cancellationToken = default)
        where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(entity);

        entity.RestoreFromDeleted();
        entity.UpdateAuditFields(restoredByUserId);
        await repository.UpdateAsync(entity, cancellationToken);
    }

    /// <summary>
    /// Soft deletes an entity by ID.
    /// </summary>
    public static async Task<bool> SoftDeleteByIdAsync<TEntity>(
        this IRepository<TEntity> repository,
        int id,
        int? deletedByUserId = default,
        CancellationToken cancellationToken = default)
        where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(repository);

        var entity = await repository.GetByIdOrDefaultAsync(id, cancellationToken);
        if (entity == null)
            return false;

        entity.MarkAsDeleted(deletedByUserId);
        await repository.UpdateAsync(entity, cancellationToken);
        return true;
    }

    /// <summary>
    /// Soft deletes multiple entities matching a predicate.
    /// </summary>
    public static async Task<int> SoftDeleteRangeAsync<TEntity>(
        this IRepository<TEntity> repository,
        Expression<Func<TEntity, bool>> predicate,
        int? deletedByUserId = default,
        CancellationToken cancellationToken = default)
        where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(predicate);

        var entities = await repository.FindAsync(predicate, cancellationToken);

        foreach (var entity in entities)
        {
            entity.MarkAsDeleted(deletedByUserId);
        }

        if (entities.Any())
        {
            await repository.UpdateRangeAsync(entities, cancellationToken);
        }

        return entities.Count;
    }
}
