using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Acontplus.Persistence.Shared;

public static class EntityRegistration
{
    private static Type GetPrimaryKeyType(Type entityType)
    {
        var currentType = entityType;
        while (currentType != null)
        {
            if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == typeof(Entity<>))
            {
                return currentType.GetGenericArguments()[0];
            }
            currentType = currentType.BaseType;
        }
        return typeof(int);
    }

    public static void RegisterEntities(
        ModelBuilder modelBuilder,
        Type dbContextType,
        Dictionary<Type, (string schema, string table)> nameMap,
        Dictionary<Type, Type> customConfigurations,
        params Type[] entityTypes)
    {
        foreach (var entityType in entityTypes)
        {
            var isValidEntity = entityType.IsClass && !entityType.IsAbstract && IsAssignableToGenericType(entityType, typeof(Entity<>));
            if (!isValidEntity) continue;
            var entityBuilder = modelBuilder.Entity(entityType);
            string determinedTableName = null;
            string determinedSchemaName = null;
            var isTableNameExplicitlyProvided = false;
            var isSchemaNameExplicitlyProvided = false;
            if (nameMap != null && nameMap.TryGetValue(entityType, out var mapConfig))
            {
                if (mapConfig.table != null) { determinedTableName = mapConfig.table; isTableNameExplicitlyProvided = true; }
                if (mapConfig.schema != null) { determinedSchemaName = mapConfig.schema; isSchemaNameExplicitlyProvided = true; }
            }
            var tableAttribute = entityType.GetCustomAttribute<TableAttribute>();
            if (tableAttribute != null)
            {
                if (!isTableNameExplicitlyProvided && tableAttribute.Name != null) { determinedTableName = tableAttribute.Name; isTableNameExplicitlyProvided = true; }
                if (!isSchemaNameExplicitlyProvided && tableAttribute.Schema != null) { determinedSchemaName = tableAttribute.Schema; isSchemaNameExplicitlyProvided = true; }
            }
            if (determinedTableName == null)
            {
                if (dbContextType != null && typeof(DbContext).IsAssignableFrom(dbContextType))
                {
                    var dbSetProperty = dbContextType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .FirstOrDefault(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) && p.PropertyType.GetGenericArguments()[0] == entityType);
                    if (dbSetProperty != null) determinedTableName = dbSetProperty.Name;
                }
                if (determinedTableName == null) determinedTableName = entityType.Name;
            }
            if (isTableNameExplicitlyProvided) entityBuilder.ToTable(determinedTableName!, determinedSchemaName);
            else if (isSchemaNameExplicitlyProvided) entityBuilder.Metadata.SetSchema(determinedSchemaName);
            // Provider-specific configuration should be applied in the provider project
            if (customConfigurations != null && customConfigurations.TryGetValue(entityType, out var customConfigType))
            {
                if (typeof(IEntityTypeConfiguration<>).MakeGenericType(entityType).IsAssignableFrom(customConfigType))
                {
                    var customConfigInstance = Activator.CreateInstance(customConfigType);
                    modelBuilder.ApplyConfiguration((dynamic)customConfigInstance);
                }
            }
        }
    }

    private static bool IsAssignableToGenericType(Type givenType, Type genericType)
    {
        var interfaceTypes = givenType.GetInterfaces();
        foreach (var it in interfaceTypes)
        {
            if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType) return true;
        }
        if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType) return true;
        Type baseType = givenType.BaseType;
        if (baseType == null) return false;
        return IsAssignableToGenericType(baseType, genericType);
    }

    public static void RegisterEntities(ModelBuilder modelBuilder, Type dbContextType, params Type[] entityTypes)
    {
        RegisterEntities(modelBuilder, dbContextType, null, null, entityTypes);
    }
} 