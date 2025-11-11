using Acontplus.Core.Domain.Exceptions;
using Acontplus.Notifications.Services;
using Acontplus.Persistence.SqlServer.DependencyInjection;
using Acontplus.Persistence.SqlServer.Exceptions;
using Acontplus.Persistence.SqlServer.Repositories;
using Acontplus.Reports.Extensions;
using Acontplus.Services.Configuration;
using Acontplus.Services.Extensions;
using Acontplus.TestApi.Extensions;
using Acontplus.TestInfrastructure.Persistence;
using Scrutor;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddTestApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddOpenApi();
        JsonConfigurationService.ConfigureAspNetCore(services, useStrictMode: false);
        JsonConfigurationService.RegisterJsonConfiguration(services);

        return services;
    }

    public static IServiceCollection AddTestApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure Acontplus.Services - Core infrastructure
        services.AddAcontplusServices(configuration);

        // Configure Report Services
        services.AddReportServices(configuration);

        // Configure application services
        services.AddTestServices(configuration);

        services.AddAuthorizationPolicies(new List<string>
        {
            "web-app", "mobile-app", "admin-portal", "test-client"
        });

        services.AddApplicationMvc();

        return services;
    }

    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Registro para la base de datos principal de la aplicación
        services.AddSqlServerPersistence<TestContext>(sqlServerOptions =>
        {
            // Configure SQL Server options
            sqlServerOptions.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsAssembly("Acontplus.TestInfrastructure"));
        });

        // Registro para una base de datos de auditoría (con clave)
        //builder.Services.AddDbContextWithUnitOfWork<AuditDbContext>(options =>
        //    options.UseSqlServer(builder.Configuration.GetConnectionString("AuditConnection")), "audit");

        return services;
    }

    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        string[] nameSpaces =
        [
            "Acontplus.Reports.Services",
            "Acontplus.Core.Security.Services",
            "Acontplus.TestApplication.Services"
        ];

        services.Scan(scan => scan
            .FromApplicationDependencies()
            .AddClasses(classes => classes.InNamespaces(nameSpaces))
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        services.AddScoped<IAdoRepository, AdoRepository>();
        services.AddScoped<IAtsXmlService, AtsXmlService>();
        services.AddScoped<IWebServiceSri, WebServiceSri>();
        services.AddScoped<IRucService, RucService>();
        services.AddScoped<ICookieService, CookieService>();
        services.AddScoped<ICaptchaService, CaptchaService>();
        services.AddScoped<ICedulaService, CedulaService>();
        services.AddScoped<IMailKitService, AmazonSesService>();
        services.AddTransient<ISqlExceptionTranslator, SqlExceptionTranslator>();
        services.AddDataProtection();

        return services;
    }
}

