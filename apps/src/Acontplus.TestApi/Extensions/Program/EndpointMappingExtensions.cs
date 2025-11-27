using Acontplus.Billing.Models.Documents;
using Acontplus.TestApi.Endpoints;
using Acontplus.TestApi.Endpoints.Business;
using Acontplus.TestApi.Endpoints.Core;
using Acontplus.TestApi.Endpoints.Demo;
using Acontplus.TestApi.Endpoints.Infrastructure;

namespace Acontplus.TestApi.Extensions.Program;

public static class EndpointMappingExtensions
{
    public static void MapTestApiEndpoints(this WebApplication app)
    {
        // Map health checks
        app.MapHealthChecks("/health");
        app.MapHealthChecks("/health/ready");
        app.MapHealthChecks("/health/live");

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

