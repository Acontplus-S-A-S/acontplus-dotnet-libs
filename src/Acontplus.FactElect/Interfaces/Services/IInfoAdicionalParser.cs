namespace Acontplus.FactElect.Interfaces.Services;

// Interface for additional info parser
public interface IInfoAdicionalParser
{
    void Parse(XmlNode nodeInfoAdicional, ComprobanteElectronico comprobante);
}