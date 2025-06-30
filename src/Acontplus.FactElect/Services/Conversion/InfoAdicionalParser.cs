using Acontplus.FactElect.Interfaces.Services;
using Acontplus.FactElect.Models.Documents;

namespace Acontplus.FactElect.Services.Conversion;

// Implementation of additional info parser
public class InfoAdicionalParser : IInfoAdicionalParser
{
    public void Parse(XmlNode nodeInfoAdicional, ComprobanteElectronico comprobante)
    {
        var infoAdicionals = (from XmlNode item in nodeInfoAdicional
                              select new InfoAdicional
                              {
                                  nombre = (item.Attributes?.GetNamedItem("nombre"))?.Value ?? "",
                                  valor = item.InnerText ?? ""
                              })
            .ToList();

        comprobante.CreateAdditionalInfo(infoAdicionals);
    }
}