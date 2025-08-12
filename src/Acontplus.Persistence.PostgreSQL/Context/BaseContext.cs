using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace Acontplus.Persistence.PostgreSQL.Context;

public abstract class BaseContext(DbContextOptions options) : DbContext(options)
{
    private readonly IDomainEventDispatcher? _eventDispatcher;

    protected BaseContext(DbContextOptions options, IDomainEventDispatcher eventDispatcher)
        : this(options)
    {
        _eventDispatcher = eventDispatcher;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await DispatchDomainEventsAsync();
        // Only update audit fields and handle soft deletes for auditable entities
        await UpdateAuditFieldsAsync();
        await HandleSoftDeletesAsync();

        var result = await base.SaveChangesAsync(cancellationToken);
        return result;
    }

    public override int SaveChanges()
    {
        DispatchDomainEventsAsync().GetAwaiter().GetResult();
        // Only update audit fields and handle soft deletes for auditable entities
        UpdateAuditFieldsAsync().GetAwaiter().GetResult();
        HandleSoftDeletesAsync().GetAwaiter().GetResult();

        return base.SaveChanges();
    }

    private async Task DispatchDomainEventsAsync()
    {
        if (_eventDispatcher == null) return;

        var entitiesWithEvents = ChangeTracker
            .Entries<IEntityWithDomainEvents>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        foreach (var entity in entitiesWithEvents)
        {
            var events = entity.DomainEvents.ToArray();
            entity.ClearDomainEvents();

            foreach (var domainEvent in events)
            {
                await _eventDispatcher.Dispatch(domainEvent);
            }
        }
    }

    private async Task UpdateAuditFieldsAsync()
    {
        // Only process entities that implement BaseEntity
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity &&
                        (e.State == EntityState.Added || e.State == EntityState.Modified))
            .ToList();

        foreach (var entry in entries)
        {
            var auditable = (BaseEntity)entry.Entity;
            // Use reflection or pattern matching to set audit fields if needed
            // (Assume BaseEntity for full audit support)
            if (entry.State == EntityState.Added)
            {
                var createdAtProp = entry.Entity.GetType().GetProperty(nameof(BaseEntity.CreatedAt));
                createdAtProp?.SetValue(entry.Entity, DateTime.UtcNow);
            }
            else
            {
                var updatedAtProp = entry.Entity.GetType().GetProperty(nameof(BaseEntity.UpdatedAt));
                updatedAtProp?.SetValue(entry.Entity, DateTime.UtcNow);
            }
        }
    }

    private async Task HandleSoftDeletesAsync()
    {
        // Only process entities that implement BaseEntity
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity &&
                        (e.State == EntityState.Deleted ||
                         e.Property(nameof(BaseEntity.IsDeleted)).IsModified))
            .ToList();

        foreach (var entry in entries)
        {
            var auditable = (BaseEntity)entry.Entity;
            if (entry.State == EntityState.Deleted || auditable.IsDeleted)
            {
                entry.State = EntityState.Modified;
                auditable.IsDeleted = true;
                var deletedAtProp = entry.Entity.GetType().GetProperty(nameof(BaseEntity.DeletedAt));
                deletedAtProp?.SetValue(entry.Entity, DateTime.UtcNow);
                var isActiveProp = entry.Entity.GetType().GetProperty(nameof(BaseEntity.IsActive));
                isActiveProp?.SetValue(entry.Entity, false);
            }
            else if (!auditable.IsDeleted && entry.Entity.GetType().GetProperty(nameof(BaseEntity.DeletedAt))?.GetValue(entry.Entity) != null)
            {
                entry.Entity.GetType().GetProperty(nameof(BaseEntity.DeletedAt))?.SetValue(entry.Entity, null);
                var deletedByUserIdProp = entry.Entity.GetType().GetProperty(nameof(BaseEntity.DeletedByUserId));
                deletedByUserIdProp?.SetValue(entry.Entity, null);
                var isActiveProp = entry.Entity.GetType().GetProperty(nameof(BaseEntity.IsActive));
                isActiveProp?.SetValue(entry.Entity, true);
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureGlobalFilters(modelBuilder);
        ConfigureDateTimeProperties(modelBuilder);
    }

    private static void ConfigureGlobalFilters(ModelBuilder builder)
    {
        // Only apply soft delete filter to auditable entities
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                var condition = Expression.Lambda(Expression.Not(property), parameter);

                builder.Entity(entityType.ClrType).HasQueryFilter(condition);
            }
        }
    }

    protected virtual void ConfigureDateTimeProperties(ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties()
                         .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?)))
            {
                if (property.ClrType == typeof(DateTime))
                {
                    builder.Entity(entityType.ClrType)
                        .Property<DateTime>(property.Name)
                        .HasConversion(
                            v => v.Kind == DateTimeKind.Unspecified
                                ? DateTime.SpecifyKind(v, DateTimeKind.Utc)
                                : v.ToUniversalTime(),
                            v => v
                        );
                }
                else
                {
                    builder.Entity(entityType.ClrType)
                        .Property<DateTime?>(property.Name)
                        .HasConversion(
                            v => v.HasValue
                                ? v.Value.Kind == DateTimeKind.Unspecified
                                    ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)
                                    : v.Value.ToUniversalTime()
                                : (DateTime?)null,
                            v => v
                        );
                }
            }
        }
    }
}
