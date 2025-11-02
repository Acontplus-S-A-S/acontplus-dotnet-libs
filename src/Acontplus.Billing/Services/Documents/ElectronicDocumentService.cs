using Acontplus.Billing.Interfaces.Services;
using Acontplus.Billing.Models.Documents;

namespace Acontplus.Billing.Services.Documents;

public class ElectronicDocumentService(IXmlDocumentParser<ComprobanteElectronico> parser) : IElectronicDocumentService
{
    private readonly IXmlDocumentParser<ComprobanteElectronico> _parser =
        parser ?? throw new ArgumentNullException(nameof(parser));

    public bool TryParseDocument(XmlDocument xmlSri, out ComprobanteElectronico comprobante, out string errorMessage)
    {
        return _parser.TryParse(xmlSri, out comprobante, out errorMessage);
    }
}