using System.ComponentModel.DataAnnotations;

namespace Acontplus.Core.Domain.Common.Entities;

public abstract class BaseEntity : Entity<int>, IAuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public int? CreatedByUserId { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    public bool IsMobileRequest { get; set; }

    [MaxLength(100)]
    public string? CreatedBy { get; set; }

    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    [MaxLength(100)]
    public string? DeletedBy { get; set; }

    protected BaseEntity()
    {
    }

    protected BaseEntity(int createdByUserId, bool isMobileRequest = false)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedByUserId = createdByUserId;
        IsMobileRequest = isMobileRequest;
    }

    public void MarkAsDeleted(int? deletedByUserId = default)
    {
        if (IsDeleted) return;

        IsDeleted = true;
        DeletedByUserId = deletedByUserId;
        DeletedAt = DateTime.UtcNow;
        IsActive = false;

        AddDomainEvent(new EntityDeletedEvent(
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

        AddDomainEvent(new EntityRestoredEvent(
            Id,
            GetType().Name,
            UpdatedByUserId
        ));
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

    public void UpdateAuditFields(int? updatedByUserId)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedByUserId = updatedByUserId;

        AddDomainEvent(new EntityModifiedEvent(
            Id,
            GetType().Name,
            updatedByUserId
        ));
    }

    protected static T Create<T>(int id, int createdByUserId, bool isMobileRequest = false)
        where T : BaseEntity, new()
    {
        return new T
        {
            Id = id,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = createdByUserId,
            IsMobileRequest = isMobileRequest
        };
    }
}
