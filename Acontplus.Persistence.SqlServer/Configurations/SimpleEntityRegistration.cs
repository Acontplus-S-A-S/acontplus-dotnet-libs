using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Acontplus.Persistence.SqlServer.Configurations;

public static class SimpleEntityRegistration
{
    /// <summary>
    /// Registers entities with the ModelBuilder, applying base configurations,
    /// custom schema/table names, and optional specific entity configurations.
    /// </summary>
    /// <param name="modelBuilder">The ModelBuilder instance.</param>
    /// <param name="dbContextType">The type of the DbContext. Used to infer DbSet names for entities if no other table name is specified.</param>
    /// <param name="nameMap">
    /// An optional dictionary mapping entity types to their desired schema and table names.
    /// This will override any [Table] attributes or default conventions.
    /// </param>
    /// <param name="customConfigurations">
    /// An optional dictionary mapping entity types to their specific IEntityTypeConfiguration<T> types.
    /// These configurations will be applied after the BaseEntityTypeConfiguration.
    /// </param>
    /// <param name="entityTypes">An array of entity types to register.</param>
    public static void RegisterEntities(
        ModelBuilder modelBuilder,
        Type dbContextType,
        Dictionary<Type, (string schema, string table)> nameMap,
        Dictionary<Type, Type> customConfigurations,
        params Type[] entityTypes)
    {
        foreach (var entityType in entityTypes)
        {
            // Ensure the entity type is a class and inherits from BaseEntity (or your desired base)
            if (!entityType.IsClass || entityType.IsAbstract || !typeof(BaseEntity).IsAssignableFrom(entityType))
            {
                Console.WriteLine($"Skipping type {entityType.Name} as it's not a valid entity (must be a concrete class inheriting from BaseEntity).");
                continue;
            }

            // Get the EntityTypeBuilder for the current entity
            var entityBuilder = modelBuilder.Entity(entityType);

            string determinedTableName = null;
            string determinedSchemaName = null;

            // Flags to track if table/schema names were explicitly provided by the user
            var isTableNameExplicitlyProvided = false;
            var isSchemaNameExplicitlyProvided = false;

            // 1. Prioritize nameMap for both table and schema
            if (nameMap != null && nameMap.TryGetValue(entityType, out var mapConfig))
            {
                if (mapConfig.table != null)
                {
                    determinedTableName = mapConfig.table;
                    isTableNameExplicitlyProvided = true;
                }
                if (mapConfig.schema != null)
                {
                    determinedSchemaName = mapConfig.schema;
                    isSchemaNameExplicitlyProvided = true;
                }
            }

            // 2. If not set by nameMap, check [Table] attribute
            var tableAttribute = entityType.GetCustomAttribute<TableAttribute>();
            if (tableAttribute != null)
            {
                if (!isTableNameExplicitlyProvided && tableAttribute.Name != null)
                {
                    determinedTableName = tableAttribute.Name;
                    isTableNameExplicitlyProvided = true; // Mark as explicitly provided by attribute
                }
                if (!isSchemaNameExplicitlyProvided && tableAttribute.Schema != null)
                {
                    determinedSchemaName = tableAttribute.Schema;
                    isSchemaNameExplicitlyProvided = true; // Mark as explicitly provided by attribute
                }
            }

            // 3. If table name is still null (meaning it's convention-based), try to get it from DbSet property name, then fallback to class name
            if (determinedTableName == null)
            {
                if (dbContextType != null && typeof(DbContext).IsAssignableFrom(dbContextType))
                {
                    // Attempt to find a DbSet property for this entity type in the provided DbContext
                    var dbSetProperty = dbContextType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                     .FirstOrDefault(p => p.PropertyType.IsGenericType &&
                                                                          p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) &&
                                                                          p.PropertyType.GetGenericArguments()[0] == entityType);
                    if (dbSetProperty != null)
                    {
                        determinedTableName = dbSetProperty.Name; // Use DbSet property name (e.g., "Usuarios")
                    }
                }

                // Final fallback to entity's class name if DbSet name couldn't be found or DbContext type wasn't provided
                if (determinedTableName == null)
                {
                    determinedTableName = entityType.Name; // Fallback to class name (e.g., "Usuario")
                }
            }

            // Apply the determined name and schema based on explicit intent
            if (isTableNameExplicitlyProvided)
            {
                // If the table name was explicitly provided (from nameMap or [Table] attribute),
                // then use ToTable to set both the name and schema.
                entityBuilder.ToTable(determinedTableName!, determinedSchemaName);
            }
            else if (isSchemaNameExplicitlyProvided)
            {
                // If only the schema was explicitly provided (and table name is convention-based),
                // set the schema using Metadata, which allows EF Core's naming conventions
                // (like snake_case) to still apply to the table name.
                entityBuilder.Metadata.SetSchema(determinedSchemaName);
            }
            // If neither table nor schema was explicitly provided, do nothing here.
            // EF Core's default conventions (including any global naming conventions) will apply to both.


            // 4. Apply the BaseEntityTypeConfiguration for common properties/conventions
            var baseConfigurationType = typeof(BaseEntityTypeConfiguration<>).MakeGenericType(entityType);
            var baseConfiguration = Activator.CreateInstance(baseConfigurationType);
            modelBuilder.ApplyConfiguration((dynamic)baseConfiguration);

            // 5. Apply any specific custom configuration for this entity
            if (customConfigurations != null && customConfigurations.TryGetValue(entityType, out var customConfigType))
            {
                if (!typeof(IEntityTypeConfiguration<>).MakeGenericType(entityType).IsAssignableFrom(customConfigType))
                {
                    Console.WriteLine($"Warning: Custom configuration type {customConfigType.Name} for entity {entityType.Name} does not implement IEntityTypeConfiguration<{entityType.Name}>. Skipping custom configuration for this entity.");
                }
                else
                {
                    var customConfigInstance = Activator.CreateInstance(customConfigType);
                    modelBuilder.ApplyConfiguration((dynamic)customConfigInstance);
                }
            }
        }
    }

    /// <summary>
    /// Registers entities with default conventions and base configuration.
    /// </summary>
    /// <param name="modelBuilder">The ModelBuilder instance.</param>
    /// <param name="dbContextType">The type of the DbContext. Used to infer DbSet names for entities.</param>
    /// <param name="entityTypes">An array of entity types to register.</param>
    public static void RegisterEntities(ModelBuilder modelBuilder, Type dbContextType, params Type[] entityTypes)
    {
        RegisterEntities(modelBuilder, dbContextType, null, null, entityTypes);
    }

    /// <summary>
    /// Registers entities, explicitly setting schemas for specified types.
    /// </summary>
    /// <param name="modelBuilder">The ModelBuilder instance.</param>
    /// <param name="dbContextType">The type of the DbContext. Used to infer DbSet names for entities.</param>
    /// <param name="entitySchemas">An array of tuples containing entity type and its desired schema.</param>
    public static void RegisterEntitiesWithSchemas(
        ModelBuilder modelBuilder,
        Type dbContextType,
        params (Type entityType, string schema)[] entitySchemas)
    {
        var schemaMap = entitySchemas.ToDictionary(
            x => x.entityType,
            x => (x.schema, table: (string)null)); // Table is null, EF will try DbSet name or class name

        var entityTypes = entitySchemas.Select(x => x.entityType).ToArray();
        RegisterEntities(modelBuilder, dbContextType, schemaMap, null, entityTypes);
    }

    /// <summary>
    /// Registers entities, explicitly setting schema and/or table names for specified types.
    /// </summary>
    /// <param name="modelBuilder">The ModelBuilder instance.</param>
    /// <param name="dbContextType">The type of the DbContext. Used to infer DbSet names for entities.</param>
    /// <param name="nameConfigs">An array of tuples containing entity type, and optional schema and table names.</param>
    public static void RegisterEntitiesWithNames(
        ModelBuilder modelBuilder,
        Type dbContextType,
        params (Type entityType, string schema, string table)[] nameConfigs)
    {
        var nameMap = nameConfigs.ToDictionary(
            x => x.entityType,
            x => (x.schema, x.table));

        var entityTypes = nameConfigs.Select(x => x.entityType).ToArray();
        RegisterEntities(modelBuilder, dbContextType, nameMap, null, entityTypes);
    }

    /// <summary>
    /// Registers entities with specific custom configurations.
    /// </summary>
    /// <param name="modelBuilder">The ModelBuilder instance.</param>
    /// <param name="dbContextType">The type of the DbContext. Used to infer DbSet names for entities.</param>
    /// <param name="customConfigurations">
    /// A dictionary mapping entity types to their specific IEntityTypeConfiguration<T> types.
    /// </param>
    /// <param name="entityTypes">An array of entity types to register.</param>
    public static void RegisterEntitiesWithCustomConfigurations(
        ModelBuilder modelBuilder,
        Type dbContextType,
        Dictionary<Type, Type> customConfigurations,
        params Type[] entityTypes)
    {
        RegisterEntities(modelBuilder, dbContextType, null, customConfigurations, entityTypes);
    }
}
