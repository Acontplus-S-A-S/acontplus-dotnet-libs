namespace Acontplus.Core.Base;

public class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? CreatedByUserId { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }
    public bool Enabled { get; set; } = true; // Deprecated field, use IsActive instead
    public bool IsActive { get; set; } = true;
    public bool Deleted { get; set; } = false; // Deprecated field, use IsDeleted instead
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    public int? UserId { get; set; } // Deprecated field, use CreatedByUserId instead
    public bool FromMobile { get; set; } = false;
    //[Timestamp]
    //public byte[] RowVersion { get; set; } // Use in your entity if you need concurrency control

    // Convenience methods for soft delete
    public void MarkAsDeleted(int? deletedByUserId = null)
    {
        IsDeleted = true;
        Deleted = true; // Maintain compatibility
        DeletedByUserId = deletedByUserId;
        // DeletedAt will be set automatically in SaveChanges
    }

    public void RestoreFromDeleted()
    {
        IsDeleted = false;
        Deleted = false; // Maintain compatibility
        DeletedAt = null;
        DeletedByUserId = null;
    }

    public void Deactivate()
    {
        IsActive = false;
        Enabled = false; // Maintain compatibility
    }

    public void Activate()
    {
        IsActive = true;
        Enabled = true; // Maintain compatibility
    }
}
