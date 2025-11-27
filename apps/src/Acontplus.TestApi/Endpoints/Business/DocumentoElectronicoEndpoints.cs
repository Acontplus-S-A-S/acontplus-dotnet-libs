namespace Acontplus.TestApi.Endpoints.Business;

using Acontplus.Billing;
using Acontplus.Core.Dtos;
using Acontplus.Billing.Interfaces.Services;

public static class DocumentoElectronicoEndpoints
{
    public static void MapDocumentoElectronicoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/documento-electronico")
            .WithTags("Documento Electronico");

        group.MapPost("/", async (IXmlSriFileService xmlSriFileService, IFormFile file) =>
        {
            var xmlSriFile = await xmlSriFileService.GetAsync(file);

            var xsdStream = ResourceHelper.GetXsdStream("Schemas.factura_V1.1.0.xsd");

            var errors = XmlValidator.Validate(xmlSriFile.XmlSri, xsdStream);

            // Handle validation results
            if (errors.Count == 0)
            {
                Console.WriteLine("XML is valid.");
            }
            else
            {
                Console.WriteLine("XML validation failed. Errors:");
                foreach (var error in errors)
                {
                    Console.WriteLine(
                        $"{error.Severity}: {error.Message} (Line {error.LineNumber}, Pos {error.LinePosition})");
                }

                // Export errors to JSON
                //XmlValidator.ExportErrorsToJson(errors, "validation_errors.json");
                Console.WriteLine("Errors exported to validation_errors.json");
            }

            return Results.Ok(ApiResponse.Success(errors));
        });
    }
}
