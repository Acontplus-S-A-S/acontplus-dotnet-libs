using Acontplus.Billing.Models.Documents;

namespace Acontplus.Billing.Interfaces.Services;

public interface IElectronicDocumentService
{
    bool TryParseDocument(XmlDocument xmlSri, out ComprobanteElectronico comprobante, out string errorMessage);
}