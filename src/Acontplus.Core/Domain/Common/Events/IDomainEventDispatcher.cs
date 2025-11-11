namespace Acontplus.Core.Domain.Common.Events;

public interface IDomainEventDispatcher
{
    Task Dispatch(IDomainEvent domainEvent);
}