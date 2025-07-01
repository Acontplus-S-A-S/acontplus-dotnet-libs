using Acontplus.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Acontplus.Persistence.SqlServer.Configurations;

public static class SqlServerEntityRegistration
{
    /// <summary>
    /// Registers all entities inheriting from AuditableEntity with SQL Server specific configurations
    /// </summary>
    public static void RegisterEntities(
        ModelBuilder modelBuilder,
        Assembly domainAssembly,
        Type dbContextType = null,
        Dictionary<Type, (string schema, string tableName)> tableMap = null)
    {
        var entityTypes = domainAssembly.GetTypes()
            .Where(t => t.IsClass &&
                        !t.IsAbstract &&
                        IsAssignableToGenericType(t, typeof(AuditableEntity<>)));

        foreach (var entityType in entityTypes)
        {
            var builder = modelBuilder.Entity(entityType);

            // Apply table/schema naming
            ApplyTableNaming(builder, entityType, dbContextType, tableMap);

            // Apply base configuration for the specific ID type
            var idType = entityType.BaseType?.GetGenericArguments().FirstOrDefault();
            if (idType != null)
            {
                var configType = typeof(SqlServerBaseEntityConfiguration<,>)
                    .MakeGenericType(entityType, idType);
                var config = Activator.CreateInstance(configType);
                modelBuilder.ApplyConfiguration((dynamic)config);
            }
        }
    }

    private static void ApplyTableNaming(
        EntityTypeBuilder builder,
        Type entityType,
        Type dbContextType,
        Dictionary<Type, (string schema, string tableName)> tableMap)
    {
        // 1. Check explicit mapping first
        if (tableMap != null && tableMap.TryGetValue(entityType, out var mapping))
        {
            builder.ToTable(mapping.tableName, mapping.schema);
            return;
        }

        // 2. Check Table attribute
        var tableAttr = entityType.GetCustomAttribute<TableAttribute>();
        if (tableAttr != null)
        {
            builder.ToTable(tableAttr.Name, tableAttr.Schema);
            return;
        }

        // 3. Infer from DbSet name if DbContext provided
        if (dbContextType != null)
        {
            var dbSetProperty = dbContextType.GetProperties()
                .FirstOrDefault(p => p.PropertyType.IsGenericType &&
                                    p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) &&
                                    p.PropertyType.GetGenericArguments()[0] == entityType);

            if (dbSetProperty != null)
            {
                builder.ToTable(dbSetProperty.Name);
                return;
            }
        }

        // 4. Default to class name
        builder.ToTable(entityType.Name);
    }

    private static bool IsAssignableToGenericType(Type givenType, Type genericType)
    {
        return givenType.GetInterfaces()
            .Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == genericType) ||
               givenType.BaseType != null && (
                   givenType.BaseType.IsGenericType &&
                   givenType.BaseType.GetGenericTypeDefinition() == genericType ||
                   IsAssignableToGenericType(givenType.BaseType, genericType));
    }

    /// <summary>
    /// Simplified registration with just schema names
    /// </summary>
    public static void RegisterWithSchemas(
        ModelBuilder modelBuilder,
        Assembly domainAssembly,
        Dictionary<Type, string> schemaMap,
        Type dbContextType = null)
    {
        var tableMap = schemaMap?.ToDictionary(
            kvp => kvp.Key,
            kvp => (schema: kvp.Value, tableName: (string)null));

        RegisterEntities(modelBuilder, domainAssembly, dbContextType, tableMap);
    }
}