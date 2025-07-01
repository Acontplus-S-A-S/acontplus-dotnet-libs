namespace Acontplus.Core.Domain.Common.Events;

public record EntityCreatedEvent<TId>(TId EntityId, string EntityType, TId? DeletedByUserId)
    : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}