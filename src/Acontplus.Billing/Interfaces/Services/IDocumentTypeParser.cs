using Acontplus.Billing.Models.Documents;

namespace Acontplus.Billing.Interfaces.Services;

// Interface for specific document type parsers
public interface IDocumentTypeParser
{
    bool Parse(XmlDocument xmlDocument, ComprobanteElectronico comprobante, out string errorMessage);
}