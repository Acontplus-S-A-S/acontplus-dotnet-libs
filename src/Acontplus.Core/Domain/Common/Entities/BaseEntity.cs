namespace Acontplus.Core.Domain.Common.Entities;

public abstract class BaseEntity : AuditableEntity<int>
{
    protected BaseEntity()
    {
    }

    protected BaseEntity(int createdByUserId, bool isMobileRequest = false)
        : base(createdByUserId, isMobileRequest)
    {
    }
}