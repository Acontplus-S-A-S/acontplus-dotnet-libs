using Acontplus.Core.Domain.Common.Events;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace Acontplus.Persistence.SqlServer.Context;

public class SqlServerModelBuilderOptions
{
    public bool EnableDecimalConversion { get; set; } = true;
    public bool EnableNonUnicodeStrings { get; set; } = true;
}

public abstract class BaseContext(DbContextOptions options) : DbContext(options)
{
    private readonly SqlServerModelBuilderOptions _sqlServerOptions = new();
    private readonly IDomainEventDispatcher? _eventDispatcher;

    protected BaseContext(DbContextOptions options, IDomainEventDispatcher eventDispatcher)
        : this(options)
    {
        _eventDispatcher = eventDispatcher;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await DispatchDomainEventsAsync();
        await UpdateAuditFieldsAsync();
        await HandleSoftDeletesAsync();

        var result = await base.SaveChangesAsync(cancellationToken);
        return result;
    }

    public override int SaveChanges()
    {
        DispatchDomainEventsAsync().GetAwaiter().GetResult();
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
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is AuditableEntity<object> &&
                        (e.State == EntityState.Added || e.State == EntityState.Modified))
            .Select(e => new { Entry = e, Entity = (AuditableEntity<object>)e.Entity });

        foreach (var item in entries)
        {
            var entity = item.Entity;
            var entry = item.Entry;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;

                if (entity.CreatedByUserId != null)
                {
                    ((IEntityWithDomainEvents)entity).AddDomainEvent(
                        new EntityCreatedEvent<object>(
                            entity.Id,
                            entity.GetType().Name,
                            entity.CreatedByUserId));
                }
            }
            else
            {
                entity.UpdatedAt = DateTime.UtcNow;

                if (entry.Property(nameof(AuditableEntity<object>.UpdatedByUserId)).IsModified)
                {
                    ((IEntityWithDomainEvents)entity).AddDomainEvent(
                        new EntityModifiedEvent<object>(
                            entity.Id,
                            entity.GetType().Name,
                            entity.UpdatedByUserId));
                }
            }
        }
    }

    private async Task HandleSoftDeletesAsync()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is AuditableEntity<object> &&
                        (e.State == EntityState.Deleted ||
                         e.Property(nameof(AuditableEntity<object>.IsDeleted)).IsModified))
            .Select(e => new { Entry = e, Entity = (AuditableEntity<object>)e.Entity });

        foreach (var item in entries)
        {
            var entity = item.Entity;
            var entry = item.Entry;

            if (entry.State == EntityState.Deleted || entity.IsDeleted)
            {
                entry.State = EntityState.Modified;
                entity.IsDeleted = true;
                entity.DeletedAt = DateTime.UtcNow;
                entity.IsActive = false;

                ((IEntityWithDomainEvents)entity).AddDomainEvent(
                    new EntityDeletedEvent<object>(
                        entity.Id,
                        entity.GetType().Name,
                        entity.DeletedByUserId));
            }
            else if (!entity.IsDeleted && entity.DeletedAt.HasValue)
            {
                entity.DeletedAt = null;
                entity.DeletedByUserId = default;
                entity.IsActive = true;

                ((IEntityWithDomainEvents)entity).AddDomainEvent(
                    new EntityRestoredEvent<object>(
                        entity.Id,
                        entity.GetType().Name,
                        entity.UpdatedByUserId));
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureGlobalFilters(builder);
        ConfigureDateTimeProperties(builder);

        if (Database.IsSqlServer())
        {
            ApplySqlServerConfigurations(builder);
        }
    }

    private static void ConfigureGlobalFilters(ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(IAuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(AuditableEntity<object>.IsDeleted));
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
                                ? (v.Value.Kind == DateTimeKind.Unspecified
                                    ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)
                                    : v.Value.ToUniversalTime())
                                : (DateTime?)null,
                            v => v
                        );
                }
            }
        }
    }

    protected virtual void ApplySqlServerConfigurations(ModelBuilder builder)
    {
        if (_sqlServerOptions.EnableDecimalConversion)
        {
            foreach (var property in builder.Model.GetEntityTypes()
                         .SelectMany(t => t.GetProperties())
                         .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetPrecision(18);
                property.SetScale(2);
            }
        }

        if (_sqlServerOptions.EnableNonUnicodeStrings)
        {
            foreach (var property in builder.Model.GetEntityTypes()
                         .SelectMany(t => t.GetProperties())
                         .Where(p => p.ClrType == typeof(string) && p.GetColumnType() == null))
            {
                property.SetIsUnicode(false);
            }
        }
    }
}