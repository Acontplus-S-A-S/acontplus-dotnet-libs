namespace Acontplus.Core.Domain.Common;

public interface IAuditableEntity
{
    bool IsDeleted { get; set; }
}