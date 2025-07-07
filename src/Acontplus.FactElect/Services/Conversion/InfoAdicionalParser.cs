namespace Acontplus.FactElect.Services.Conversion;

// Implementation of additional info parser
public class InfoAdicionalParser : IInfoAdicionalParser
{
    public void Parse(XmlNode nodeInfoAdicional, ComprobanteElectronico comprobante)
    {
        var infoAdicionals = (from XmlNode item in nodeInfoAdicional
                              select new InfoAdicional
                              {
                                  Nombre = (item.Attributes?.GetNamedItem("nombre"))?.Value ?? "",
                                  Valor = item.InnerText ?? ""
                              })
            .ToList();

        comprobante.CreateAdditionalInfo(infoAdicionals);
    }
}