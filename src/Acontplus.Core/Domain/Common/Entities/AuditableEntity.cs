using System.ComponentModel.DataAnnotations;

namespace Acontplus.Core.Domain.Common.Entities;

/// <summary>
/// Base auditable entity with modern .NET 9+ features and improved audit patterns.
/// </summary>
/// <typeparam name="TId">The type of the entity's primary key.</typeparam>
public abstract class AuditableEntity<TId> : Entity<TId>, IAuditableEntity where TId : notnull
{
    public DateTime CreatedAt { get; set; }
    /// <summary>
    /// The local user ID (foreign key) who created the entity, if available.
    /// </summary>
    public TId? CreatedByUserId { get; set; }
    public DateTime? UpdatedAt { get; set; }
    /// <summary>
    /// The local user ID (foreign key) who last updated the entity, if available.
    /// </summary>
    public TId? UpdatedByUserId { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    /// <summary>
    /// The local user ID (foreign key) who deleted the entity, if available.
    /// </summary>
    public TId? DeletedByUserId { get; set; }
    public bool IsMobileRequest { get; set; }
    /// <summary>
    /// The username, email, or external identifier of the creator (for cross-system or external users).
    /// </summary>
    [MaxLength(100)]
    public string? CreatedBy { get; set; } // e.g., "admin@company.com" or "erpuser123"
    /// <summary>
    /// The username, email, or external identifier of the last updater (for cross-system or external users).
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
    /// <summary>
    /// The username, email, or external identifier of the deleter (for cross-system or external users).
    /// </summary>
    [MaxLength(100)]
    public string? DeletedBy { get; set; }

    protected AuditableEntity()
    {
    }

    protected AuditableEntity(TId createdByUserId, bool isMobileRequest = false)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedByUserId = createdByUserId;
        IsMobileRequest = isMobileRequest;
    }

    /// <summary>
    /// Marks the entity as deleted with audit information.
    /// </summary>
    /// <param name="deletedByUserId">The user ID who deleted the entity.</param>
    public void MarkAsDeleted(TId? deletedByUserId = default)
    {
        if (IsDeleted) return;

        IsDeleted = true;
        DeletedByUserId = deletedByUserId;
        DeletedAt = DateTime.UtcNow;
        IsActive = false;

        // Add domain event for deletion
        AddDomainEvent(new EntityDeletedEvent<TId>(
            Id,
            GetType().Name,
            deletedByUserId
        ));
    }

    /// <summary>
    /// Restores the entity from deleted state.
    /// </summary>
    public void RestoreFromDeleted()
    {
        if (!IsDeleted) return;

        IsDeleted = false;
        DeletedAt = null;
        DeletedByUserId = default;

        // Add domain event for restoration
        AddDomainEvent(new EntityRestoredEvent<TId>(
            Id,
            GetType().Name,
            UpdatedByUserId
        ));
    }

    /// <summary>
    /// Deactivates the entity.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive) return;

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Activates the entity.
    /// </summary>
    public void Activate()
    {
        if (IsActive) return;

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates audit fields when the entity is modified.
    /// </summary>
    /// <param name="updatedByUserId">The user ID who updated the entity.</param>
    public void UpdateAuditFields(TId? updatedByUserId)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedByUserId = updatedByUserId;

        // Add domain event for modification
        AddDomainEvent(new EntityModifiedEvent<TId>(
            Id,
            GetType().Name,
            updatedByUserId
        ));
    }

    /// <summary>
    /// Creates a new auditable entity with the specified ID and creator.
    /// </summary>
    /// <typeparam name="T">The type of the auditable entity.</typeparam>
    /// <param name="id">The entity ID.</param>
    /// <param name="createdByUserId">The user ID who created the entity.</param>
    /// <param name="isMobileRequest">Whether the request came from a mobile device.</param>
    /// <returns>A new auditable entity instance.</returns>
    protected static T Create<T>(TId id, TId createdByUserId, bool isMobileRequest = false)
        where T : AuditableEntity<TId>, new()
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