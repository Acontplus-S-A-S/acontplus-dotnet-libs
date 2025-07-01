namespace Acontplus.Core.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}