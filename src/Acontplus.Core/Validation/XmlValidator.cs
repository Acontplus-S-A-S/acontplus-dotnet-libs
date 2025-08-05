using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Acontplus.Core.Validation;

public class ValidationError
{
    public required string Message { get; set; }
    public XmlSeverityType Severity { get; set; }
    public int LineNumber { get; set; }
    public int LinePosition { get; set; }
}

public static class XmlValidator
{
    /// <summary>
    ///     Validates the provided XmlDocument against an XSD schema file.
    /// </summary>
    /// <param name="xmlDocument">The XML document to validate.</param>
    /// <param name="xsdFilePath">The path to the XSD file.</param>
    /// <returns>A list of ValidationError objects containing error details.</returns>
    public static List<ValidationError> Validate(XmlDocument xmlDocument, Stream xsdStream)
    {
        var validationErrors = new List<ValidationError>();

        if (xmlDocument == null)
        {
            throw new ArgumentNullException(nameof(xmlDocument));
        }

        if (xsdStream == null)
        {
            throw new ArgumentNullException(nameof(xsdStream));
        }

        try
        {
            var schemaSet = new XmlSchemaSet();
            schemaSet.Add(null, XmlReader.Create(xsdStream));

            // Configure XmlReaderSettings
            var settings = new XmlReaderSettings { ValidationType = ValidationType.Schema, Schemas = schemaSet };

            settings.ValidationEventHandler += (sender, e) =>
            {
                validationErrors.Add(new ValidationError
                {
                    Message = e.Message,
                    Severity = e.Severity,
                    LineNumber = e.Exception?.LineNumber ?? 0,
                    LinePosition = e.Exception?.LinePosition ?? 0
                });
            };

            // Validate XmlDocument
            using (var stringReader = new StringReader(xmlDocument.OuterXml))
            using (var reader = XmlReader.Create(stringReader, settings))
            {
                while (reader.Read()) { } // Read and validate the entire XML
            }
        }
        catch (XmlException ex)
        {
            validationErrors.Add(new ValidationError
            {
                Message = $"XML Exception: {ex.Message}",
                Severity = XmlSeverityType.Error
            });
        }
        catch (Exception ex)
        {
            validationErrors.Add(new ValidationError
            {
                Message = $"Unexpected Exception: {ex.Message}",
                Severity = XmlSeverityType.Error
            });
        }

        return validationErrors;
    }

    /// <summary>
    ///     Exports validation errors to a JSON file.
    /// </summary>
    /// <param name="errors">List of validation errors.</param>
    /// <param name="outputFilePath">The path to save the JSON file.</param>
    public static void ExportErrorsToJson(List<ValidationError> errors, string outputFilePath)
    {
        if (errors == null || errors.Count == 0)
        {
            return;
        }

        var json = JsonSerializer.Serialize(errors, new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(outputFilePath, json);
    }
    /// <summary>
    /// Limpia un XML para hacerlo compatible con SQL Server
    /// </summary>
    public static string CleanXmlForSqlServer(string xml)
    {
        // Si el XML está vacío o es nulo, retornarlo tal cual
        if (string.IsNullOrWhiteSpace(xml))
            return xml;
        try
        {
            // 1. Eliminar la declaración XML (<?xml version="1.0" encoding="UTF-8"?>)
            xml = Regex.Replace(xml, @"<\?xml.*?\?>", "", RegexOptions.Singleline).TrimStart();

            // 2. Eliminar caracteres BOM (Byte Order Mark) si existen
            xml = RemoveBomChars(xml);

            // 3. Limpiar etiquetas HTML que pueden estar mezcladas con XML
            xml = CleanHtmlTags(xml);

            // 4. Corregir ampersands no escapados (&) que no sean parte de entidades XML
            xml = EscapeUnescapedAmpersands(xml);

            // 5. Normalizar saltos de línea
            xml = NormalizeLineBreaks(xml);

            // 6. Eliminar caracteres no válidos para XML
            xml = RemoveInvalidXmlChars(xml);

            return xml;
        }
        catch (Exception ex)
        {
            //Log.Error(ex, "Error limpiando XML");
            // En caso de error, al menos eliminar la declaración XML
            return RemoveXmlDeclaration(xml);
        }
    }

    /// <summary>
    /// Limpia etiquetas HTML que pueden estar mezcladas con XML
    /// </summary>
    private static string CleanHtmlTags(string xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
            return xml;

        // 1. Convertir entidades HTML comunes a sus equivalentes XML
        xml = ConvertHtmlEntitiesToXml(xml);

        // 2. Eliminar o convertir etiquetas HTML comunes que pueden causar problemas
        xml = RemoveOrConvertHtmlTags(xml);

        // 3. Limpiar atributos HTML que no son válidos en XML
        xml = CleanHtmlAttributes(xml);

        return xml;
    }

    /// <summary>
    /// Convierte entidades HTML a sus equivalentes XML válidos
    /// </summary>
    private static string ConvertHtmlEntitiesToXml(string xml)
    {
        // Mapeo de entidades HTML comunes a XML
        var htmlToXmlEntities = new Dictionary<string, string>
    {
        { "&nbsp;", "&#160;" },      // Espacio no separable
        { "&copy;", "&#169;" },      // Copyright
        { "&reg;", "&#174;" },       // Registered
        { "&trade;", "&#8482;" },    // Trademark
        { "&hellip;", "&#8230;" },   // Ellipsis
        { "&mdash;", "&#8212;" },    // Em dash
        { "&ndash;", "&#8211;" },    // En dash
        { "&lsquo;", "&#8216;" },    // Left single quote
        { "&rsquo;", "&#8217;" },    // Right single quote
        { "&ldquo;", "&#8220;" },    // Left double quote
        { "&rdquo;", "&#8221;" },    // Right double quote
        { "&euro;", "&#8364;" },     // Euro symbol
        { "&pound;", "&#163;" },     // Pound symbol
        { "&yen;", "&#165;" },       // Yen symbol
        { "&cent;", "&#162;" },      // Cent symbol
    };

        foreach (var entity in htmlToXmlEntities)
        {
            xml = xml.Replace(entity.Key, entity.Value);
        }

        return xml;
    }

    /// <summary>
    /// Elimina o convierte etiquetas HTML problemáticas
    /// </summary>
    private static string RemoveOrConvertHtmlTags(string xml)
    {
        // Etiquetas HTML que se pueden convertir a texto plano o eliminar
        var tagsToRemove = new[]
        {
        "br", "BR",           // Salto de línea
        "hr", "HR",           // Línea horizontal
        "img", "IMG",         // Imágenes
        "script", "SCRIPT",   // Scripts
        "style", "STYLE",     // Estilos
        "meta", "META",       // Metadatos
        "link", "LINK",       // Enlaces externos
    };

        // Eliminar etiquetas auto-cerradas problemáticas
        foreach (var tag in tagsToRemove)
        {
            // Etiquetas auto-cerradas: <br/>, <hr/>, <img.../>, etc.
            xml = Regex.Replace(xml, $@"<{tag}[^>]*?/>", "", RegexOptions.IgnoreCase);
            // Etiquetas simples: <br>, <hr>, etc.
            xml = Regex.Replace(xml, $@"<{tag}[^>]*?>", "", RegexOptions.IgnoreCase);
        }

        // Eliminar contenido completo de etiquetas script y style
        xml = Regex.Replace(xml, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        xml = Regex.Replace(xml, @"<style[^>]*?>.*?</style>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        // Convertir <br> y </br> a saltos de línea si están dentro de contenido XML
        xml = Regex.Replace(xml, @"</?br[^>]*?>", "\n", RegexOptions.IgnoreCase);

        return xml;
    }

    /// <summary>
    /// Limpia atributos HTML que pueden no ser válidos en XML
    /// </summary>
    private static string CleanHtmlAttributes(string xml)
    {
        // Atributos HTML comunes que pueden causar problemas en XML
        var problematicAttributes = new[]
        {
        "onclick", "onload", "onmouseover", "onmouseout", "onfocus", "onblur",
        "class", "id", "style", "href", "src", "alt", "title"
    };

        foreach (var attr in problematicAttributes)
        {
            // Eliminar atributos problemáticos de cualquier etiqueta
            xml = Regex.Replace(xml, $@"\s+{attr}\s*=\s*[""'][^""']*[""']", "", RegexOptions.IgnoreCase);
            xml = Regex.Replace(xml, $@"\s+{attr}\s*=\s*[^>\s]+", "", RegexOptions.IgnoreCase);
        }

        return xml;
    }

    /// <summary>
    /// Escapa los ampersands y que no sean parte de entidades XML válidas
    /// </summary>
    private static string EscapeUnescapedAmpersands(string xml)
    {
        // Patrón para encontrar ampersands no escapados
        // Un ampersand es considerado no escapado si no es seguido por:
        // 1. Una entidad XML predefinida (amp;, lt;, gt;, quot;, apos;)
        // 2. Una referencia numérica (&#123; o &#xABC;)
        // 3. El inicio de una referencia de entidad que termina con ;
        return Regex.Replace(
            xml,
            @"&(?!(amp;|lt;|gt;|quot;|apos;|#[0-9]+;|#x[0-9a-fA-F]+;|\w+;))",
            "&amp;",
            RegexOptions.IgnoreCase
        );
    }

    /// <summary>
    /// Elimina caracteres BOM (Byte Order Mark) que pueden causar problemas de codificación
    /// </summary>
    private static string RemoveBomChars(string xml)
    {
        // BOM para UTF-8: EF BB BF
        if (xml.StartsWith("\xEF\xBB\xBF"))
            xml = xml.Substring(3);

        // Otros BOM comunes
        if (xml.StartsWith("\xFE\xFF") || xml.StartsWith("\xFF\xFE"))
            xml = xml.Substring(2);

        return xml;
    }

    /// <summary>
    /// Normaliza los saltos de línea para evitar problemas con diferentes sistemas operativos
    /// </summary>
    private static string NormalizeLineBreaks(string xml)
    {
        // Convertir todos los tipos de saltos de línea a \n
        return Regex.Replace(xml, @"\r\n?|\n", "\n");
    }

    /// <summary>
    /// Elimina caracteres que no son válidos en XML según la especificación
    /// </summary>
    private static string RemoveInvalidXmlChars(string xml)
    {
        // Según la especificación XML, estos caracteres no son válidos
        return Regex.Replace(xml, @"[\x00-\x08\x0B\x0C\x0E-\x1F]", "");
    }

    /// <summary>
    /// Método original para eliminar declaración XML
    /// </summary>
    private static string RemoveXmlDeclaration(string xml)
    {
        if (string.IsNullOrWhiteSpace(xml)) return xml;
        // Elimina cualquier declaración como <?xml version="1.0" encoding="UTF-8"?>
        return Regex.Replace(xml, @"<\?xml.*?\?>", "", RegexOptions.Singleline).TrimStart();
    }

    /// <summary>
    /// Método alternativo más agresivo para casos extremos donde hay mucho HTML mezclado
    /// </summary>
    private static string AggressiveHtmlClean(string xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
            return xml;

        // Eliminar TODOS los tags HTML conocidos manteniendo solo el contenido
        xml = Regex.Replace(xml, @"</?(?:div|span|p|h[1-6]|ul|ol|li|table|tr|td|th|thead|tbody|tfoot|strong|b|em|i|u|small|big)[^>]*?>", "", RegexOptions.IgnoreCase);

        // Si después de limpiar queda muy poco contenido, es probable que fuera principalmente HTML
        if (xml.Trim().Length < 10)
            return string.Empty;

        return xml;
    }
}
