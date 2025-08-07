using System.Reflection;

namespace Acontplus.TestInfrastructure.Persistence;

public class TestContext(DbContextOptions<TestContext> options) : BaseContext(options)
{
    public DbSet<Dia> Dias { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<WhatsAppUsage> WhatsAppUsages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // 1. Register entities with default conventions and BaseEntityTypeConfiguration
        // Usuario will be mapped to table "Usuarios" (from DbSet name)
        BaseEntityRegistration.RegisterEntities(modelBuilder, typeof(TestContext), typeof(Usuario));
        SimpleEntityRegistration.RegisterEntities(modelBuilder, typeof(TestContext), typeof(Dia));

        // 2. Register entities with explicit schema/table names (overrides [Table] attribute)
        //SimpleEntityRegistration.RegisterEntitiesWithNames(modelBuilder, typeof(TestContext),
        //    (typeof(Usuario), "seguridad", "usuarios_app"), // Explicitly set schema and table name
        //    (typeof(Producto), "inventario", null) // Set schema, Producto will be "Productos" (from DbSet name)
        //);
        //SimpleEntityRegistration.RegisterEntitiesWithNames(modelBuilder, typeof(TestContext),
        //    (typeof(Usuario), "seguridad", "usuarios_app"), // Explicitly set schema and table name
        //    (typeof(Producto), "inventario", null) // Set schema, Producto will be "Productos" (from DbSet name)
        //);
        //SimpleEntityRegistration.RegisterEntitiesWithNames(modelBuilder, typeof(TestContext),
        //    (typeof(Usuario), "seguridad", null) // Explicitly set schema and table name
        //);

        // 3. Register entities with explicit schemas only
        // Cliente will be in 'crm' schema, table name will be 'cliente' (from [Table] attribute)
        // If [Table] was absent, it would try "Clientes" (from DbSet name)
        //SimpleEntityRegistration.RegisterEntitiesWithSchemas(modelBuilder, typeof(TestContext),
        //    (typeof(Usuario), "seguridad")
        //);

        //// 4. Register entities with custom IEntityTypeConfiguration
        //var customConfigs = new Dictionary<Type, Type>
        //{
        //    { typeof(Producto), typeof(ProductoConfiguration) }
        //};
        //SimpleEntityRegistration.RegisterEntitiesWithCustomConfigurations(modelBuilder, typeof(TestContext), customConfigs, typeof(Producto));
    }
}
