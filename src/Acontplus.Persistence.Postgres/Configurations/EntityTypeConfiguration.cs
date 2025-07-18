using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acontplus.Persistence.Postgres.Configurations;

/// <summary>
/// Base configuration for non-auditable entities.
/// </summary>
public class EntityTypeConfiguration<TEntity, TId> : IEntityTypeConfiguration<TEntity>
    where TEntity : Entity<TId>
    where TId : notnull
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Configure the primary key
        builder.HasKey(x => x.Id);
        // Add more common configuration for non-auditable entities here if needed
    }
}