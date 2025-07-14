using Acontplus.Core.Domain.Common.Entities;

namespace Acontplus.Core.Abstractions.Persistence;

/// <summary>
/// Defines generic data access operations for auditable entities.
/// </summary>
/// TEntity: The type of the entity.
/// TId: The type of the entity's primary key must be not null.
public interface IAuditableRepository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : AuditableEntity<TId>
    where TId : notnull
{
    #region Audit Methods
    /// <summary>
    /// Soft deletes an entity (sets IsDeleted, DeletedAt, etc.).
    /// </summary>
    Task SoftDeleteAsync(TEntity entity, TId? deletedByUserId = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores a soft-deleted entity.
    /// </summary>
    Task RestoreAsync(TEntity entity, TId? restoredByUserId = default, CancellationToken cancellationToken = default);
    #endregion
}