﻿using Acontplus.ApiDocumentation;
using Acontplus.Core.Domain.Enums;
using Acontplus.Core.Domain.Exceptions;
using Acontplus.Core.Extensions;
using Acontplus.FactElect.Models.Documents;
using Acontplus.FactElect.Services.Authentication;
using Acontplus.FactElect.Services.External;
using Acontplus.FactElect.Services.Validation;
using Acontplus.Notifications.Services;
using Acontplus.Persistence.SqlServer.DependencyInjection;
using Acontplus.Persistence.SqlServer.Exceptions;
using Acontplus.Persistence.SqlServer.Repositories;
using Acontplus.Services.Extensions;
using Acontplus.TestApi.Endpoints;
using Acontplus.TestApi.Extensions;
using Acontplus.TestInfrastructure.Persistence;
using Scrutor;
using Serilog;

// 1. Optional: Create a bootstrap logger for early startup issues
//    This captures logs from WebApplication.CreateBuilder() itself.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Get environment name once for clarity and re-use
    var environment = builder.Environment.EnvironmentName;

    // 2. Configure Serilog for the web host using the new extension method
    builder.Host.UseSerilog((hostContext, services, loggerConfiguration) =>
    {
        // Call the new method to apply your advanced logging settings to the LoggerConfiguration
        loggerConfiguration.ConfigureAdvancedLogger(hostContext.Configuration, environment);

        // This part tells Serilog to load its configuration from appsettings.json
        // and resolve any services (e.g., custom enrichers requiring DI)
        loggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);
        loggerConfiguration.ReadFrom.Services(services);
    });

    // 3. Register your LoggingOptions class into the DI container
    //    This is where builder.Services (an IServiceCollection) is available.
    builder.Services.AddAdvancedLoggingOptions(builder.Configuration);

    // Configure enterprise services with modern patterns
    builder.Services.AddEnterpriseServices(builder.Configuration);
    builder.Services.AddEnterpriseAuthorizationPolicies(new List<string>
    {
        "web-app", "mobile-app", "admin-portal", "test-client"
    });
    builder.Services.AddEnterpriseMvc();
    builder.Services.AddEnterpriseHealthChecks(builder.Configuration);


    // --- Start new try-catch block for service registration ---
    try
    {
        builder.Services.AddApplicationServices(builder.Configuration); // <--- This is a likely suspect



        //builder.Services.AddDbContextPool<TestContext>(options =>
        //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        //builder.Services.AddScoped<BaseContext>(sp => sp.GetRequiredService<TestContext>());
        //builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork<TestContext>));
        //builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));

        // Registro para la base de datos principal de la aplicación
        builder.Services.AddSqlServerPersistence<TestContext>(sqlServerOptions =>
        {
            // Configure SQL Server options
            sqlServerOptions.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsAssembly("Acontplus.TestInfrastructure"));
        });
        // Registro para una base de datos de auditoría (con clave)
        //builder.Services.AddDbContextWithUnitOfWork<AuditDbContext>(options =>
        //    options.UseSqlServer(builder.Configuration.GetConnectionString("AuditConnection")), "audit");

        string[] nameSpaces =
        [
            "Acontplus.Reports.Services",
            "Acontplus.Core.Security.Services",
            "Acontplus.TestApplication.Services"
        ];

        builder.Services.Scan(scan => scan
            .FromApplicationDependencies()
            .AddClasses(classes => classes.InNamespaces(nameSpaces))
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );


        builder.Services.AddScoped<IAdoRepository, AdoRepository>();
        builder.Services.AddScoped<IAtsXmlService, AtsXmlService>();
        builder.Services.AddScoped<IWebServiceSri, WebServiceSri>();
        builder.Services.AddScoped<IRucService, RucService>();
        builder.Services.AddScoped<ICookieService, CookieService>();
        builder.Services.AddScoped<ICaptchaService, CaptchaService>();
        builder.Services.AddScoped<ICedulaService, CedulaService>();
        builder.Services.AddScoped<IMailKitService, AmazonSesService>();
        builder.Services.AddTransient<ISqlExceptionTranslator, SqlExceptionTranslator>();
        builder.Services.AddDataProtection();

        builder.Services.AddApiVersioningAndDocumentation();
    }
    catch (Exception serviceEx)
    {
        Log.Fatal(serviceEx, "An error occurred during service registration.");
        // Re-throw or exit to ensure the host aborts, but now you have the log
        throw;
    }

    // --- End new try-catch block ---
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    // Use Serilog request logging BEFORE other middleware like UseRouting, UseAuthentication, etc.
    app.UseSerilogRequestLogging(); // Captures HTTP request/response details

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();

        app.UseApiVersioningAndDocumentation();
    }

    // Use enterprise middleware pipeline
    app.UseEnterpriseMiddleware(app.Environment);

    app.UseRouting();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    // Map enterprise demonstration endpoints
    app.MapEnterpriseEndpoints();

    // Map health checks
    app.MapHealthChecks("/health");

    app.MapAllEndpoints();

    app.MapGet("/generateats", async (IAtsXmlService atsXmlService) =>
    {
        // In a real application, you'd get this data from a database or another service
        // For demonstration, let's create some dummy data
        var atsData = new AtsData
        {
            Header = new AtsHeader
            {
                TipoIdInformante = "01",
                IdInformante = "1234567890001",
                RazonSocial = "EXAMPLE S.A.",
                Anio = "2023",
                Mes = "12",
                NumEstabRuc = "001",
                TotalVentas = "1000.00",
                CodigoOperativo = "IVA"
            },
            Purchases = new List<Purchase>
            {
                new Purchase
                {
                    CodSustento = "01",
                    TpIdProv = "04",
                    IdProv = "1234567890",
                    TipoComprobante = "01",
                    ParteRel = "NO",
                    FechaRegistro = "01/12/2023",
                    Establecimiento = "001",
                    PuntoEmision = "001",
                    Secuencial = "000000001",
                    FechaEmision = "01/12/2023",
                    Autorizacion = "1234567890123456789012345678901234567890123",
                    BaseNoGraIva = "0.00",
                    BaseImponible = "100.00",
                    BaseImpGrav = "0.00",
                    BaseImpExe = "0.00",
                    MontoIce = "0.00",
                    MontoIva = "12.00",
                    ValRetBien10 = "0.00",
                    ValRetServ20 = "0.00",
                    ValorRetBienes = "0.00",
                    ValRetServ50 = "0.00",
                    ValorRetServicios = "0.00",
                    ValRetServ100 = "0.00",
                    TotbasesImpReemb = "0.00",
                    PagoLocExt = "01",
                    FormaPago = "01",
                    NroDocumento = "000000001" // Important for linking
                }
            },
            WithholdingTaxes = new List<WithholdingTax>
            {
                new WithholdingTax
                {
                    NroDocumento = "000000001",
                    ClaveAcceso = "1234567890123456789012345678901234567890123",
                    CodRetAir = "303",
                    BaseImpAir = "100.00",
                    PorcentajeAir = "1.00",
                    ValRetAir = "1.00"
                }
            },
            Sales = new List<Sale>
            {
                new Sale
                {
                    TpIdCliente = "04",
                    IdCliente = "1234567890",
                    TipoComprobante = "18",
                    TipoEmision = "F",
                    NumeroComprobantes = "1",
                    BaseNoGraIva = "0.00",
                    BaseImponible = "100.00",
                    BaseImpGrav = "0.00",
                    MontoIva = "12.00",
                    MontoIce = "0.00",
                    ValorRetIva = "0.00",
                    ValorRetRenta = "0.00",
                    FormaPago = "01"
                }
            },
            EstablishmentSales = new List<EstablishmentSale>
            {
                new EstablishmentSale
                {
                    CodEstab = "001",
                    VentasEstab = "1000.00",
                    IvaComp = "12.00"
                }
            },
            CanceledDocuments = new List<CanceledDocument>
            {
                new CanceledDocument
                {
                    TipoComprobante = "01",
                    Establecimiento = "001",
                    PuntoEmision = "001",
                    SecuencialInicio = "000000002",
                    SecuencialFin = "000000002",
                    Autorizacion = "1234567890123456789012345678901234567890123"
                }
            }
        };

        try
        {
            var xmlBytes = await atsXmlService.CreateAtsXmlAsync(atsData);
            return Results.Bytes(xmlBytes, "application/xml", "ats.xml");
        }
        catch (Exception ex)
        {
            // Log the exception (use ILogger for enterprise apps)
            Console.WriteLine($"Error generating XML: {ex.Message}");
            return Results.Problem("Error generating ATS XML", statusCode: 500);
        }
    });

    //app.Use(async (context, next) =>
    //{
    //    var serializer = context.RequestServices.GetService<JsonSerializer>(); // Newtonsoft.Json
    //    var jsonOptions = context.RequestServices.GetService<IOptions<JsonOptions>>(); // System.Text.Json
    //    Console.WriteLine($"Using Newtonsoft.Json: {serializer != null}");
    //    Console.WriteLine($"Using System.Text.Json: {jsonOptions != null}");
    //    await next();
    //});

    app.MapGet("/minimal-test", () =>
    //Results.Json(new ApiResponse { Status = ResponseStatus.Success }));
    Results.Json(ApiResponse.Success("great")));
    var options = JsonExtensions.DefaultOptions;
    var json = ApiResponse.Success("Test").SerializeModern();
    Console.WriteLine(json);

    var test = new Test { Status = ResponseStatus.Success };
    Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(test, options));


    //Endpoints

    app.MapPost("/dia", async (IDiaService diaService, CreateDiaDto createDiaDto) =>
    {
        return await diaService.CreateAsync(createDiaDto).ToMinimalApiResultAsync();
    });
    app.MapPut("/dia/{id:int}", async (IDiaService diaService, int id, UpdateDiaDto updateDiaDto) =>
    {
        return await diaService.UpdateAsync(id, updateDiaDto).ToMinimalApiResultAsync();
    });


    await app.RunAsync();
}
catch (Exception ex)
{
    // Catch any critical startup errors
    Log.Fatal(ex, "API host terminated unexpectedly.");
}
finally
{
    // Ensure all buffered logs are flushed on application shutdown
    Log.CloseAndFlush();
}


public class Test
{
    public ResponseStatus Status { get; set; }
}
