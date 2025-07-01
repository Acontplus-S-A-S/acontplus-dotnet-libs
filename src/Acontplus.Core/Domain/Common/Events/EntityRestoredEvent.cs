namespace Acontplus.Core.Domain.Common.Events;

public record EntityRestoredEvent<TId>(TId EntityId, string EntityType, TId? DeletedByUserId)
    : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}