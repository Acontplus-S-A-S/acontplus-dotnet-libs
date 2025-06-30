using Acontplus.FactElect.Models.Documents;

namespace Acontplus.FactElect.Interfaces.Services;

// Interface for details parser
public interface IDetailsParser
{
    void Parse(XmlNode nodeDetails, ComprobanteElectronico comprobante);
}