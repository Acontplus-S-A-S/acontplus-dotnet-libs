namespace Acontplus.Persistence.Shared.Repositories;

/// <summary>
/// Repository for auditable entities, adds audit-specific methods.
/// </summary>
public class AuditableBaseRepository<TEntity, TId> : BaseRepository<TEntity, TId>, IAuditableRepository<TEntity, TId>
    where TEntity : AuditableEntity<TId>
    where TId : notnull
{
    public AuditableBaseRepository(DbContext context, ILogger<BaseRepository<TEntity, TId>> logger = null)
        : base(context, logger) { }

    public virtual async Task SoftDeleteAsync(TEntity entity, TId? deletedByUserId = default, CancellationToken cancellationToken = default)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(SoftDeleteAsync)}");
        try
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedByUserId = deletedByUserId;
            entity.IsActive = false;
            _dbSet.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error soft deleting entity of type {EntityType}", typeof(TEntity).Name);
            throw new RepositoryException("Error soft deleting entity", ex);
        }
    }

    public virtual async Task RestoreAsync(TEntity entity, TId? restoredByUserId = default, CancellationToken cancellationToken = default)
    {
        using var activity = DiagnosticConfig.ActivitySource.StartActivity($"{nameof(RestoreAsync)}");
        try
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            entity.IsDeleted = false;
            entity.DeletedAt = null;
            entity.DeletedByUserId = default;
            entity.IsActive = true;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedByUserId = restoredByUserId;
            _dbSet.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error restoring entity of type {EntityType}", typeof(TEntity).Name);
            throw new RepositoryException("Error restoring entity", ex);
        }
    }
}