namespace Acontplus.TestApi.Endpoints.Business;

public static class DocumentoElectronicoEndpoints
{
    public static void MapDocumentoElectronicoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/documento-electronico")
            .WithTags("Documento Electronico");

        group.MapPost("/", ValidateXml)
            .WithName("ValidateXml")
            .Produces<ApiResponse<List<ValidationError>>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> ValidateXml(IFormFile file, HttpContext httpContext)
    {
        var xmlSriFileService = httpContext.RequestServices.GetRequiredService<IXmlSriFileService>();
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
    }
}
