using Acontplus.Billing.Models.Documents;

namespace Acontplus.Billing.Interfaces.Services;

// Interface for tributary info parser
public interface IInfoTributariaParser
{
    void Parse(XmlNode nodeInfoTrib, ComprobanteElectronico comprobante);
}
