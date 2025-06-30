using Acontplus.FactElect.Interfaces.Services;
using Acontplus.FactElect.Models.Documents;

namespace Acontplus.FactElect.Services.Documents;

public class ElectronicDocumentService(IXmlDocumentParser<ComprobanteElectronico> parser) : IElectronicDocumentService
{
    private readonly IXmlDocumentParser<ComprobanteElectronico> _parser =
        parser ?? throw new ArgumentNullException(nameof(parser));

    public bool TryParseDocument(XmlDocument xmlSri, out ComprobanteElectronico comprobante, out string errorMessage)
    {
        return _parser.TryParse(xmlSri, out comprobante, out errorMessage);
    }
}