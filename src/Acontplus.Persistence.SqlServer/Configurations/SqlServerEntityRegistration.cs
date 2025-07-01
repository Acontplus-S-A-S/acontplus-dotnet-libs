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
            if (!string.IsNullOrEmpty(mapping.schema))
            {
                // If a schema is explicitly provided in the map
                if (!string.IsNullOrEmpty(mapping.tableName))
                {
                    // If both table name and schema are provided, use them
                    builder.ToTable(mapping.tableName, mapping.schema);
                }
                else
                {
                    // If ONLY schema is provided in the map,
                    // we need to determine the table name from other sources
                    // before applying the schema.
                    // This is the core of the fix for RegisterWithSchemas.

                    string tableName = null;

                    // Try TableAttribute first for the table name
                    var tableAttr = entityType.GetCustomAttribute<TableAttribute>();
                    if (tableAttr != null && !string.IsNullOrEmpty(tableAttr.Name))
                    {
                        tableName = tableAttr.Name;
                    }
                    // Then try DbSet name
                    else if (dbContextType != null)
                    {
                        var dbSetProperty = dbContextType.GetProperties()
                            .FirstOrDefault(p => p.PropertyType.IsGenericType &&
                                                p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) &&
                                                p.PropertyType.GetGenericArguments()[0] == entityType);
                        if (dbSetProperty != null)
                        {
                            tableName = dbSetProperty.Name;
                        }
                    }

                    // Finally, default to class name if no other table name was found
                    if (string.IsNullOrEmpty(tableName))
                    {
                        tableName = entityType.Name;
                    }

                    builder.ToTable(tableName, mapping.schema);
                }
            }
            else if (!string.IsNullOrEmpty(mapping.tableName))
            {
                // Only table name is provided in the map (no schema), use it.
                builder.ToTable(mapping.tableName);
            }
            // If both are null, it will fall through to other methods.
            return; // Important: Return after handling the map entry
        }

        // 2. Check Table attribute (if not already handled by explicit map for schema only)
        var tableAttrFallback = entityType.GetCustomAttribute<TableAttribute>();
        if (tableAttrFallback != null)
        {
            builder.ToTable(tableAttrFallback.Name, tableAttrFallback.Schema);
            return;
        }

        // 3. Infer from DbSet name if DbContext provided (if not already handled)
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
            kvp => (schema: kvp.Value, tableName: (string)null)); // Keep tableName null here, as ApplyTableNaming will handle it

        RegisterEntities(modelBuilder, domainAssembly, dbContextType, tableMap);
    }
}