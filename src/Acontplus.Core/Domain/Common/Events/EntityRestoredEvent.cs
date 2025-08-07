namespace Acontplus.Core.Domain.Common.Events;

public record EntityRestoredEvent(int EntityId, string EntityType, int? DeletedByUserId)
    : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
