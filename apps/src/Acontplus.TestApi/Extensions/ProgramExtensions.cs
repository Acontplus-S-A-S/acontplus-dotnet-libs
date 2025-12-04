using Acontplus.TestApi.Endpoints.Demo;
using Acontplus.TestApi.Endpoints.Infrastructure;
using Acontplus.TestInfrastructure.Persistence;
using Scrutor;

namespace Acontplus.TestApi.Extensions;

/// <summary>
/// Consolidated extension methods for configuring the Test API application.
/// </summary>
public static class ProgramExtensions
{
    /// <summary>
    /// Adds all services required for the Test API application.
    /// </summary>
    public static IServiceCollection AddAllTestServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Test API specific services (HTTP context, OpenAPI, JSON config)
        services.AddHttpContextAccessor();
        services.AddOpenApi();
        JsonConfigurationService.ConfigureAspNetCore(services, useStrictMode: false);
        JsonConfigurationService.RegisterJsonConfiguration(services);

        // Configure Infrastructure Services (v2.0) with health checks and response compression
        services.AddInfrastructureServices(configuration, addHealthChecks: true, addResponseCompression: true);

        // Configure Application Services (v2.0)
        services.AddApplicationServices(configuration);

        // Configure Lookup Service
        services.AddLookupService();

        // Configure Report Services
        services.AddReportServices(configuration);

        return services;
    }

    /// <summary>
    /// Adds database services for SQL Server persistence.
    /// </summary>
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Registro para la base de datos principal de la aplicación
        services.AddSqlServerPersistence<TestContext>(sqlServerOptions =>
        {
            // Configure SQL Server options
            sqlServerOptions.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsAssembly("Acontplus.TestInfrastructure"));
        });

        return services;
    }

    /// <summary>
    /// Adds business services using assembly scanning.
    /// </summary>
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        string[] nameSpaces =
        [
            "Acontplus.Reports.Services",
            "Acontplus.Core.Security.Services",
            "Acontplus.TestApplication.Services",
            "Acontplus.TestApi.Services"
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

    /// <summary>
    /// Configures the middleware pipeline for the Test API.
    /// </summary>
    public static void ConfigureTestApiMiddleware(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        // Use Serilog request logging BEFORE other middleware like UseRouting, UseAuthentication, etc.
        app.UseSerilogRequestLogging(); // Captures HTTP request/response details

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseApiVersioningAndDocumentation();
        }

        app.UseRouting();
        app.UseHttpsRedirection();
        app.UseResponseCompression();
        app.UseAuthorization();

        // Controllers have been converted to Minimal API endpoints
        // app.MapControllers();
    }

    /// <summary>
    /// Maps all endpoints for the Test API.
    /// </summary>
    public static void MapTestApiEndpoints(this WebApplication app)
    {
        // Map health checks using infrastructure extension
        app.MapHealthCheckEndpoints();

        // Map all organized endpoints
        app.MapAllEndpoints();

        // Map specific business endpoints
        app.MapAtsEndpoints();
        app.MapDocumentoElectronicoEndpoints();
        app.MapReportsEndpoints();
        app.MapUsuarioEndpoints();

        // Map core endpoints
        app.MapEncryptionEndpoints();
        app.MapLookupEndpoints();

        // Map infrastructure endpoints
        app.MapBarcodeEndpoints();
        app.MapConfigurationTestEndpoints();
        app.MapPrintEndpoints();

        // Map demo endpoints
        app.MapBusinessExceptionTestEndpoints();
        app.MapExceptionTestEndpoints();
    }
}
