using Acontplus.Persistence.SqlServer.Context;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Acontplus.TestHostApi.Data;

public class TestContext(DbContextOptions<TestContext> options) : BaseContext(options)
{
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<WhatsAppUsage> WhatsAppUsages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        //Basic Registration

        SqlServerEntityRegistration.RegisterEntities(
        modelBuilder,
        typeof(Usuario).Assembly,
        typeof(TestContext));

        //With Schema Mapping

        //var schemaMap = new Dictionary<Type, string>
        //{
        //    { typeof(Usuario), "security" }
        //    //{ typeof(Product), "inventory" }
        //};

        //SqlServerEntityRegistration.RegisterWithSchemas(
        //    modelBuilder,
        //    typeof(Usuario).Assembly,
        //    schemaMap);

        //With Full Table Mapping

        //var tableMap = new Dictionary<Type, (string, string)>
        //{
        //    { typeof(Usuario), ("Security", "Usuarios") },
        //    //{ typeof(Product), ("Inventory", "Products") }
        //};

        //SqlServerEntityRegistration.RegisterEntities(
        //    modelBuilder,
        //    typeof(Usuario).Assembly,
        //    tableMap: tableMap);

    }
}
