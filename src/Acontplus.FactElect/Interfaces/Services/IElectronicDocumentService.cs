namespace Acontplus.FactElect.Interfaces.Services;

public interface IElectronicDocumentService
{
    bool TryParseDocument(XmlDocument xmlSri, out ComprobanteElectronico comprobante, out string errorMessage);
}