using Acontplus.Billing.Models.Documents;
using Acontplus.TestApi.Endpoints;
using Acontplus.TestApi.Endpoints.Business;
using Acontplus.TestApi.Endpoints.Core;
using Acontplus.TestApi.Endpoints.Demo;
using Acontplus.TestApi.Endpoints.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;

namespace Acontplus.TestApi.Extensions.Program;

public static class EndpointMappingExtensions
{
    public static void MapTestApiEndpoints(this WebApplication app)
    {
        // Map health checks with proper tags and response formatting
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description,
                        data = e.Value.Data
                    }),
                    totalDuration = report.TotalDuration
                });
                await context.Response.WriteAsync(result);
            }
        });

        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready")
        });

        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false // No checks = always healthy (liveness probe)
        });

        // Map all organized endpoints
        app.MapAllEndpoints();

        // Map specific business endpoints
        app.MapAtsEndpoints();
        app.MapDocumentoElectronicoEndpoints();
        app.MapReportsEndpoints();
        app.MapUsuarioEndpoints();

        // Map core endpoints
        app.MapEncryptionEndpoints();

        // Map infrastructure endpoints
        app.MapBarcodeEndpoints();
        app.MapConfigurationTestEndpoints();
        app.MapPrintEndpoints();

        // Map demo endpoints
        app.MapBusinessExceptionTestEndpoints();
        app.MapExceptionTestEndpoints();
    }
}

