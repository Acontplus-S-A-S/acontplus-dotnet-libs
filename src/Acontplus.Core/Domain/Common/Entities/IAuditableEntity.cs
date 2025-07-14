namespace Acontplus.Core.Domain.Common.Entities;

public interface IAuditableEntity
{
    bool IsDeleted { get; set; }
}