using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Acontplus.Persistence.SqlServer.Context;

public class SqlServerModelBuilderOptions
{
    public bool EnableDecimalConversion { get; set; } = true;
    public bool EnableNonUnicodeStrings { get; set; } = true;
}

public class BaseContext(DbContextOptions options) : DbContext(options)
{
    protected SqlServerModelBuilderOptions SqlServerOptions { get; } = new SqlServerModelBuilderOptions();

    public override int SaveChanges()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e is { Entity: BaseEntity, State: EntityState.Added or EntityState.Modified });

        foreach (var entityEntry in entries)
        {
            var entity = (BaseEntity)entityEntry.Entity;

            if (entityEntry.State == EntityState.Modified)
            {
                // Automatically update UpdatedAt
                entity.UpdatedAt = DateTime.UtcNow;
            }

            // Handle soft delete automatically
            HandleSoftDelete(entityEntry, entity);
        }
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e is { Entity: BaseEntity, State: EntityState.Added or EntityState.Modified });

        foreach (var entityEntry in entries)
        {
            var entity = (BaseEntity)entityEntry.Entity;

            if (entityEntry.State == EntityState.Modified)
            {
                // Automatically update UpdatedAt
                entity.UpdatedAt = DateTime.UtcNow;
            }

            // Handle soft delete automatically
            HandleSoftDelete(entityEntry, entity);
        }
        var result = await base.SaveChangesAsync(cancellationToken);
        return result;
    }

    // NEW: Method to handle soft delete automatically
    private void HandleSoftDelete(EntityEntry entityEntry, BaseEntity entity)
    {
        // If IsDeleted is being marked as true and DeletedAt has no value
        if (entity.IsDeleted && !entity.DeletedAt.HasValue)
        {
            entity.DeletedAt = DateTime.UtcNow;
            // Also update deprecated field for compatibility
            entity.Deleted = true;
            entity.Enabled = false; // Optionally disable the entity if it is marked as deleted
            entity.IsActive = false; // Optionally mark the entity as inactive
        }
        // If IsDeleted is being marked as false, clear DeletedAt
        else if (!entity.IsDeleted && entity.DeletedAt.HasValue)
        {
            entity.DeletedAt = null;
            entity.DeletedByUserId = null;
            // Also update deprecated field for compatibility
            entity.Deleted = false;
            entity.Enabled = true; // Optionally enable the entity if it is marked as not deleted
            entity.IsActive = true; // Optionally mark the entity as active
        }

        // Handle compatibility with deprecated 'Deleted' field
        // If deprecated field is used, sync with the new one
        var deletedProperty = entityEntry.Property(nameof(BaseEntity.Deleted));
        if (deletedProperty.IsModified)
        {
            if (entity.Deleted && !entity.IsDeleted)
            {
                entity.IsDeleted = true;
                entity.DeletedAt = DateTime.UtcNow;
                entity.IsActive = false; // Optionally mark the entity as inactive
                entity.Enabled = false; // Optionally disable the entity
            }
            else if (!entity.Deleted && entity.IsDeleted)
            {
                entity.IsDeleted = false;
                entity.DeletedAt = null;
                entity.DeletedByUserId = null;
                entity.IsActive = true; // Optionally mark the entity as active
                entity.Enabled = true; // Optionally enable the entity
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure all DateTime properties as UTC
        ConfigureDateTimeProperties(builder);

        // SQL Server configurations (only if enabled)
        if (Database.ProviderName == "Microsoft.EntityFrameworkCore.SqlServer")
        {
            ApplySqlServerConfigurations(builder);
        }
    }

    // FIXED: Method to configure DateTime properties
    protected virtual void ConfigureDateTimeProperties(ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var properties = entityType.ClrType.GetProperties();

            // Handle non-nullable DateTime properties
            var dateTimeProperties = properties.Where(p => p.PropertyType == typeof(DateTime));
            foreach (var property in dateTimeProperties)
            {
                builder.Entity(entityType.Name)
                    .Property<DateTime>(property.Name)
                    .HasConversion(
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                    );
            }

            // Handle nullable DateTime properties
            var nullableDateTimeProperties = properties.Where(p => p.PropertyType == typeof(DateTime?));
            foreach (var property in nullableDateTimeProperties)
            {
                builder.Entity(entityType.Name)
                    .Property<DateTime?>(property.Name)
                    .HasConversion<DateTime?>(
                        v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null,
                        v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null
                    );
            }
        }
    }

    protected virtual void ApplySqlServerConfigurations(ModelBuilder builder)
    {
        // Convert decimals to double (optional)
        if (SqlServerOptions.EnableDecimalConversion)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(decimal));
                foreach (var property in properties)
                    builder.Entity(entityType.Name).Property(property.Name).HasConversion<double>();
            }
        }

        // Force string columns to non-unicode (optional)
        if (SqlServerOptions.EnableNonUnicodeStrings)
        {
            foreach (var property in builder.Model.GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.ClrType == typeof(string) && p.GetColumnType() == null))
                property.SetIsUnicode(false);
        }
    }
}
