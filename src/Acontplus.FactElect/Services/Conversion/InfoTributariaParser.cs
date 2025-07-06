namespace Acontplus.FactElect.Services.Conversion;

// Implementation of info tributaria parser
public class InfoTributariaParser : IInfoTributariaParser
{
    public void Parse(XmlNode nodeInfoTrib, ComprobanteElectronico comprobante)
    {
        comprobante.InfoTributaria = new InfoTributaria
        {
            Ambiente = nodeInfoTrib.SelectSingleNode("ambiente")?.InnerText,
            TipoEmision = nodeInfoTrib.SelectSingleNode("tipoEmision")?.InnerText,
            RazonSocial = nodeInfoTrib.SelectSingleNode("razonSocial")?.InnerText,
            NombreComercial = nodeInfoTrib.SelectSingleNode("nombreComercial")?.InnerText ?? "",
            Ruc = nodeInfoTrib.SelectSingleNode("ruc")?.InnerText,
            ClaveAcceso = nodeInfoTrib.SelectSingleNode("claveAcceso")?.InnerText,
            CodDoc = nodeInfoTrib.SelectSingleNode("codDoc")?.InnerText,
            Estab = nodeInfoTrib.SelectSingleNode("estab")?.InnerText,
            PtoEmi = nodeInfoTrib.SelectSingleNode("ptoEmi")?.InnerText,
            Secuencial = nodeInfoTrib.SelectSingleNode("secuencial")?.InnerText,
            DirMatriz = nodeInfoTrib.SelectSingleNode("dirMatriz")?.InnerText
        };
    }
}