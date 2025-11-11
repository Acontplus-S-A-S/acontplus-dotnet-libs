namespace Acontplus.TestApi.Controllers.Business;

using Acontplus.TestApi.Controllers.Core;

public class DocumentoElectronicoController(IXmlSriFileService xmlSriFileService) : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Post(IFormFile file)
    {
        //if (file.ContentType != "text/xml")
        //{
        //    return BadRequest("Formato de archivo no soportado");
        //}
        //if (file.ContentType != "application/xml")
        //{
        //    return BadRequest("Formato de archivo no soportado");
        //}

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

        return Ok(errors);
    }
}

