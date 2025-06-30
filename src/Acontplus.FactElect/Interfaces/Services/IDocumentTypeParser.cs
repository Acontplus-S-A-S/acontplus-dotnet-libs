using Acontplus.FactElect.Models.Documents;

namespace Acontplus.FactElect.Interfaces.Services;

// Interface for specific document type parsers
public interface IDocumentTypeParser
{
    bool Parse(XmlDocument xmlDocument, ComprobanteElectronico comprobante, out string errorMessage);
}