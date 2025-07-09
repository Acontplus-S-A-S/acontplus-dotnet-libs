using Acontplus.Core.Domain.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acontplus.Persistence.SqlServer.Configurations;

public class BaseEntityTypeConfiguration<TEntity, TId> : IEntityTypeConfiguration<TEntity>
    where TEntity : AuditableEntity<TId>
    where TId : struct
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Configure common audit properties for SQL Server
        builder.Property(x => x.CreatedAt)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.Property(x => x.UpdatedAt)
            .HasColumnType("datetime2")
            .IsRequired(false);

        builder.Property(x => x.DeletedAt)
            .HasColumnType("datetime2")
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
