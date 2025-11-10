namespace Acontplus.Core.Domain.Common.Entities;

public interface IEntityWithDomainEvents
{
    void AddDomainEvent(IDomainEvent domainEvent);
    void ClearDomainEvents();
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
}