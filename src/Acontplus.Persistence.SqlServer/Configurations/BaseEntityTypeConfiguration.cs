using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acontplus.Persistence.SqlServer.Configurations;

public class BaseEntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        ConfigurePrimaryKey(builder);
        ConfigureTimestamps(builder);
        ConfigureStatusFields(builder);
        ConfigureExternalUserTracking(builder);
        ConfigureSoftDeleteAndIndexes(builder);
    }

    protected virtual void ConfigurePrimaryKey(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
    }

    protected virtual void ConfigureTimestamps(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property(x => x.CreatedAt)
            .HasColumnType("datetime2")
            .HasPrecision(7)
            .HasDefaultValueSql("SYSUTCDATETIME()")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnType("datetime2")
            .HasPrecision(7)
            .IsRequired(false);

        builder.Property(x => x.DeletedAt)
            .HasColumnType("datetime2")
            .HasPrecision(7)
            .IsRequired(false);
    }

    protected virtual void ConfigureStatusFields(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property(x => x.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.IsMobileRequest)
            .HasDefaultValue(false)
            .IsRequired();
    }

    protected virtual void ConfigureExternalUserTracking(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property(x => x.CreatedBy)
            .IsRequired(false)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(x => x.UpdatedBy)
            .IsRequired(false)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(x => x.DeletedBy)
            .IsRequired(false)
            .HasMaxLength(100)
            .IsUnicode(false);
    }

    protected virtual void ConfigureSoftDeleteAndIndexes(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);
        ConfigureIndexes(builder);
    }

    protected virtual void ConfigureIndexes(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName($"IX_{typeof(TEntity).Name}_CreatedAt");

        builder.HasIndex(x => x.CreatedByUserId)
            .HasDatabaseName($"IX_{typeof(TEntity).Name}_CreatedByUserId");

        builder.HasIndex(x => x.IsDeleted)
            .HasDatabaseName($"IX_{typeof(TEntity).Name}_IsDeleted");

        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName($"IX_{typeof(TEntity).Name}_IsActive");

        builder.HasIndex(x => new { x.IsActive, x.IsDeleted })
            .HasDatabaseName($"IX_{typeof(TEntity).Name}_Status");
    }
}
