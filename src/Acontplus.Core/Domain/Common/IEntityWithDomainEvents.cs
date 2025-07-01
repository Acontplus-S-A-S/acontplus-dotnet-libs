namespace Acontplus.Core.Domain.Common;

public interface IEntityWithDomainEvents
{
    void AddDomainEvent(IDomainEvent domainEvent);
    void ClearDomainEvents();
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
}