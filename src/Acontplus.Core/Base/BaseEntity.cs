namespace Acontplus.Core.Base;

public class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? CreatedByUserId { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    public bool FromMobile { get; set; } = false;
    //[Timestamp]
    //public byte[] RowVersion { get; set; } // Use in your entity if you need concurrency control

    // Convenience methods for soft delete
    public void MarkAsDeleted(int? deletedByUserId = null)
    {
        IsDeleted = true;
        DeletedByUserId = deletedByUserId;
        // DeletedAt will be set automatically in SaveChanges
    }

    public void RestoreFromDeleted()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedByUserId = null;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
