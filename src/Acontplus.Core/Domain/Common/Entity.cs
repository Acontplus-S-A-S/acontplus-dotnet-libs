namespace Acontplus.Core.Domain.Common;

public abstract class Entity<TId> : IEntityWithDomainEvents where TId : notnull
{
    public TId Id { get; protected init; } = default!;

    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // Protected for domain logic (only derived classes can call directly)
    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    // Explicit implementation for external access (e.g., DbContext)
    void IEntityWithDomainEvents.AddDomainEvent(IDomainEvent domainEvent) => AddDomainEvent(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        if (Id.Equals(default) || other.Id.Equals(default))
            return false;

        return Id.Equals(other.Id);
    }

    public static bool operator ==(Entity<TId>? a, Entity<TId>? b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(Entity<TId>? a, Entity<TId>? b) => !(a == b);

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);
}