using Microsoft.EntityFrameworkCore;

namespace Acontplus.Persistence.Shared.Registration;

public static class SimpleEntityRegistration
{
    public static void RegisterEntities(ModelBuilder modelBuilder, Type dbContextType, params Type[] entityTypes)
        => EntityRegistration.RegisterEntities(modelBuilder, dbContextType, entityTypes);

    public static void RegisterEntities(
        ModelBuilder modelBuilder,
        Type dbContextType,
        Dictionary<Type, (string schema, string table)> nameMap,
        Dictionary<Type, Type> customConfigurations,
        params Type[] entityTypes)
        => EntityRegistration.RegisterEntities(modelBuilder, dbContextType, nameMap, customConfigurations, entityTypes);
} 