namespace Acontplus.Persistence.Postgres.Configurations;

/// <summary>
/// Base configuration for simple entities that don't inherit from BaseEntity.
/// </summary>
public class SimpleEntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : class
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Configure the primary key - assumes Id property exists
        var idProperty = typeof(TEntity).GetProperty("Id");
        if (idProperty != null)
        {
            builder.HasKey("Id");
            builder.Property("Id")
                .UseIdentityByDefaultColumn() // PostgreSQL IDENTITY column
                .ValueGeneratedOnAdd();
        }
    }
}
