namespace Acontplus.Core.Domain.Common.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
