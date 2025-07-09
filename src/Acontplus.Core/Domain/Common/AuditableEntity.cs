namespace Acontplus.Core.Domain.Common;

public abstract class AuditableEntity<TId> : Entity<TId> where TId : notnull
{
    public DateTime CreatedAt { get; set; }
    public TId? CreatedByUserId { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public TId? UpdatedByUserId { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public TId? DeletedByUserId { get; set; }
    public bool IsMobileRequest { get; set; }

    protected AuditableEntity()
    {
    }

    protected AuditableEntity(TId createdByUserId, bool isMobileRequest = false)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedByUserId = createdByUserId;
        IsMobileRequest = isMobileRequest;
    }

    public void MarkAsDeleted(TId? deletedByUserId = default)
    {
        if (IsDeleted) return;

        IsDeleted = true;
        DeletedByUserId = deletedByUserId;
        DeletedAt = DateTime.UtcNow;
        IsActive = false;

        // Could add a DomainEvent here like:
        AddDomainEvent(new EntityDeletedEvent<TId>(
            Id,
            GetType().Name,
            deletedByUserId
        ));
    }

    public void RestoreFromDeleted()
    {
        if (!IsDeleted) return;

        IsDeleted = false;
        DeletedAt = null;
        DeletedByUserId = default;
    }

    public void Deactivate()
    {
        if (!IsActive) return;

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (IsActive) return;

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateAuditFields(TId? updatedByUserId)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedByUserId = updatedByUserId;
    }
}