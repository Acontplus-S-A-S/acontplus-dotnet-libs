namespace Acontplus.FactElect.Services.Conversion;

// Implementation of info tributaria parser
public class InfoTributariaParser : IInfoTributariaParser
{
    public void Parse(XmlNode nodeInfoTrib, ComprobanteElectronico comprobante)
    {
        comprobante.infoTributaria = new InfoTributaria
        {
            ambiente = nodeInfoTrib.SelectSingleNode("ambiente")?.InnerText,
            tipoEmision = nodeInfoTrib.SelectSingleNode("tipoEmision")?.InnerText,
            razonSocial = nodeInfoTrib.SelectSingleNode("razonSocial")?.InnerText,
            nombreComercial = nodeInfoTrib.SelectSingleNode("nombreComercial")?.InnerText ?? "",
            ruc = nodeInfoTrib.SelectSingleNode("ruc")?.InnerText,
            claveAcceso = nodeInfoTrib.SelectSingleNode("claveAcceso")?.InnerText,
            codDoc = nodeInfoTrib.SelectSingleNode("codDoc")?.InnerText,
            estab = nodeInfoTrib.SelectSingleNode("estab")?.InnerText,
            ptoEmi = nodeInfoTrib.SelectSingleNode("ptoEmi")?.InnerText,
            secuencial = nodeInfoTrib.SelectSingleNode("secuencial")?.InnerText,
            dirMatriz = nodeInfoTrib.SelectSingleNode("dirMatriz")?.InnerText
        };
    }
}