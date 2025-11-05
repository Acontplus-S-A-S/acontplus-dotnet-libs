using Acontplus.Billing.Models.Documents;
using Acontplus.Core.Domain.Enums;
using Acontplus.Core.Extensions;
using Acontplus.TestApi.Endpoints;

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
        app.MapAtsEndpoint();
        app.MapMinimalTestEndpoint();
        app.MapDiaEndpoints();
    }

    private static void MapAtsEndpoint(this WebApplication app)
    {
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
    }

    private static void MapMinimalTestEndpoint(this WebApplication app)
    {
        app.MapGet("/minimal-test", () =>
            Results.Json(ApiResponse.Success("Minimal API test successful")));
    }

    private static void MapDiaEndpoints(this WebApplication app)
    {
        //Endpoints
        app.MapPost("/dia", async (IDiaService diaService, CreateDiaDto createDiaDto) =>
        {
            return await diaService.CreateAsync(createDiaDto).ToMinimalApiResultAsync();
        });

        app.MapPut("/dia/{id:int}", async (IDiaService diaService, int id, UpdateDiaDto updateDiaDto) =>
        {
            return await diaService.UpdateAsync(id, updateDiaDto).ToMinimalApiResultAsync();
        });
    }
}
