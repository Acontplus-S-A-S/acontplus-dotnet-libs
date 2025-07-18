using Acontplus.Core.Abstractions.Persistence;
using Acontplus.Core.Domain.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Acontplus.Persistence.Shared.Repositories;

/// <summary>
/// Generic auditable repository base for EF Core, provider-agnostic. Inherit and override for provider-specific logic.
/// </summary>
public abstract class AuditableBaseRepository<TEntity, TId> : BaseRepository<TEntity, TId>, IAuditableRepository<TEntity, TId>
    where TEntity : AuditableEntity<TId>
    where TId : notnull
{
    protected AuditableBaseRepository(DbContext context, ILogger logger = null)
        : base(context, logger) { }

    public virtual async Task SoftDeleteAsync(TEntity entity, TId? deletedByUserId = default, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        entity.IsDeleted = true;
        entity.DeletedAt = DateTimeOffset.UtcNow;
        if (deletedByUserId is not null)
            entity.DeletedByUserId = deletedByUserId;
        _dbSet.Update(entity);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task RestoreAsync(TEntity entity, TId? restoredByUserId = default, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        entity.IsDeleted = false;
        entity.DeletedAt = null;
        if (restoredByUserId is not null)
            entity.RestoredByUserId = restoredByUserId;
        _dbSet.Update(entity);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
} 