using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acontplus.Persistence.SqlServer.Configurations;

public class BaseEntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Get the provider name (this will be set during model creation)
        var providerName = GetDatabaseProvider(builder);

        ConfigureCreatedAtDefault(builder, providerName);

        builder.Property(x => x.Enabled).HasDefaultValue(true);
        builder.Property(x => x.IsActive).HasDefaultValue(true);
        builder.Property(x => x.FromMobile).HasDefaultValue(false);
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);
        builder.Property(x => x.Deleted).HasDefaultValue(false);
    }

    private string GetDatabaseProvider(EntityTypeBuilder<TEntity> builder)
    {
        // Method 1: Check if we're using Npgsql by looking for specific extensions
        if (builder.Metadata.Model.GetEntityTypes()
            .Any(e => e.GetAnnotations()
                .Any(a => a.Value?.ToString().Contains("Npgsql") == true)))
        {
            return "Npgsql.EntityFrameworkCore.PostgreSQL";
        }

        // Method 2: Check for SQL Server specific annotations
        if (builder.Metadata.Model.GetEntityTypes()
            .Any(e => e.GetAnnotations()
                .Any(a => a.Value?.ToString().Contains("SqlServer") == true)))
        {
            return "Microsoft.EntityFrameworkCore.SqlServer";
        }

        // Method 3: Assembly scanning fallback
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        if (assemblies.Any(a => a.GetName().Name?.Contains("Npgsql") == true))
            return "Npgsql.EntityFrameworkCore.PostgreSQL";

        if (assemblies.Any(a => a.GetName().Name?.Contains("SqlServer") == true))
            return "Microsoft.EntityFrameworkCore.SqlServer";

        if (assemblies.Any(a => a.GetName().Name?.Contains("Sqlite") == true))
            return "Microsoft.EntityFrameworkCore.Sqlite";

        // Default fallback
        return "Microsoft.EntityFrameworkCore.SqlServer";
    }

    private void ConfigureCreatedAtDefault(EntityTypeBuilder<TEntity> builder, string dbProvider)
    {
        var cleanProvider = dbProvider?.Replace("\"", "").Trim();

        switch (cleanProvider)
        {
            case "Npgsql.EntityFrameworkCore.PostgreSQL":
                builder.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");
                break;
            case "Microsoft.EntityFrameworkCore.SqlServer":
                builder.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                break;
            case "Microsoft.EntityFrameworkCore.Sqlite":
                builder.Property(x => x.CreatedAt).HasDefaultValueSql("datetime('now')");
                break;
            case "Pomelo.EntityFrameworkCore.MySql":
            case "MySql.EntityFrameworkCore":
                builder.Property(x => x.CreatedAt).HasDefaultValueSql("UTC_TIMESTAMP()");
                break;
            case "Oracle.EntityFrameworkCore":
                builder.Property(x => x.CreatedAt).HasDefaultValueSql("SYS_EXTRACT_UTC(SYSTIMESTAMP)");
                break;
            default:
                builder.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                break;
        }
    }
}
