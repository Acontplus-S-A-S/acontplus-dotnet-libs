namespace Acontplus.Core.Base;

public abstract class BaseEntity : AuditableEntity<int>
{
    protected BaseEntity()
    {
    }

    protected BaseEntity(int createdByUserId, bool fromMobile = false)
        : base(createdByUserId, fromMobile)
    {
    }
}