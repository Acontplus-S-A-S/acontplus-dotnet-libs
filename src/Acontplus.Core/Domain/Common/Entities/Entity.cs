namespace Acontplus.Core.Domain.Common.Entities;

public abstract class Entity<TId> : IEntityWithDomainEvents where TId : notnull
{
    public required TId Id { get; init; }

    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    void IEntityWithDomainEvents.AddDomainEvent(IDomainEvent domainEvent) => AddDomainEvent(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();

    public override bool Equals(object? obj)
    {
        return obj is not Entity<TId> other
            ? false
            : ReferenceEquals(this, other) || GetType() == other.GetType() && !Id.Equals(default) && !other.Id.Equals(default) && Id.Equals(other.Id);
    }

    public static bool operator ==(Entity<TId>? a, Entity<TId>? b)
    {
        return a is null && b is null || a is not null && b is not null && a.Equals(b);
    }

    public static bool operator !=(Entity<TId>? a, Entity<TId>? b) => !(a == b);

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);

    protected static T Create<T>(TId id) where T : Entity<TId>, new()
    {
        return new T { Id = id };
    }
}
