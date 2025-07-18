using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acontplus.Persistence.Postgres.Configurations;

public class BaseEntityTypeConfiguration<TEntity, TId> : IEntityTypeConfiguration<TEntity>
    where TEntity : AuditableEntity<TId>
    where TId : struct
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Configure common audit properties for PostgreSQL
        builder.Property(x => x.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.UpdatedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        builder.Property(x => x.DeletedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(x => x.IsMobileRequest)
            .HasDefaultValue(false);

        // Configure query filter for soft delete
        builder.HasQueryFilter(x => !x.IsDeleted);

        // Configure indexes for audit fields (optional)
        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.CreatedByUserId);
        builder.HasIndex(x => x.IsDeleted);
        builder.HasIndex(x => x.IsActive);
    }
}
