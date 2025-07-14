namespace Acontplus.Core.Domain.Common.Entities;

public interface IAuditableEntity
{
    bool IsDeleted { get; set; }
    bool IsActive { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
    DateTime? DeletedAt { get; set; }
    bool IsMobileRequest { get; set; }
    string? CreatedBy { get; set; }
    string? UpdatedBy { get; set; }
    string? DeletedBy { get; set; }
}