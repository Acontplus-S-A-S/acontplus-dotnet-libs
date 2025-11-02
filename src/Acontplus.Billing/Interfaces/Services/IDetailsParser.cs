using Acontplus.Billing.Models.Documents;

namespace Acontplus.Billing.Interfaces.Services;

// Interface for details parser
public interface IDetailsParser
{
    void Parse(XmlNode nodeDetails, ComprobanteElectronico comprobante);
}