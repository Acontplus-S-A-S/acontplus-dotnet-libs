namespace Acontplus.FactElect.Services.Conversion;

// Implementation of details parser
public class DetailsParser : IDetailsParser
{
    public void Parse(XmlNode nodeDetails, ComprobanteElectronico comprobante)
    {
        var detalles = new List<Detalle>();
        var impuestos = new List<Impuesto>();
        var idDetalle = 0;

        foreach (XmlNode item in nodeDetails)
        {
            var detail = new Detalle { idDetalle = idDetalle };

            if (comprobante.codDoc == "01")
                detail.codigoPrincipal = item.SelectSingleNode("codigoPrincipal")?.InnerText ?? "";
            else
                detail.codigoPrincipal = item.SelectSingleNode("codigoInterno")?.InnerText ?? "";

            detail.codigoAuxiliar = item.SelectSingleNode("codigoAuxiliar")?.InnerText ?? "";
            detail.descripcion = item.SelectSingleNode("descripcion")?.InnerText;
            detail.cantidad = item.SelectSingleNode("cantidad")?.InnerText;
            detail.precioUnitario = item.SelectSingleNode("precioUnitario")?.InnerText;
            detail.descuento = item.SelectSingleNode("descuento")?.InnerText ?? "";
            detail.precioTotalSinImpuesto = item.SelectSingleNode("precioTotalSinImpuesto")?.InnerText;
            detail.impuestos = item.SelectSingleNode("impuestos") == null
                ? ""
                : item.SelectNodes("impuestos")?[0]?.OuterXml;
            detail.detallesAdicionales = item.SelectSingleNode("detallesAdicionales") == null
                ? ""
                : item.SelectNodes("detallesAdicionales")?[0]?.OuterXml;

            // Process tax information
            var taxNodes = item.SelectNodes("impuestos");
            if (taxNodes != null)
            {
                foreach (XmlElement taxes in taxNodes)
                {
                    impuestos.Add(new Impuesto
                    {
                        idDetalle = idDetalle,
                        codArticulo = detail.codigoPrincipal,
                        codigo = taxes.GetElementsByTagName("codigo")[0]?.InnerText,
                        codigoPorcentaje = taxes.GetElementsByTagName("codigoPorcentaje")[0]?.InnerText,
                        tarifa = taxes.GetElementsByTagName("tarifa")[0]?.InnerText,
                        baseImponible = taxes.GetElementsByTagName("baseImponible")[0]?.InnerText,
                        valor = taxes.GetElementsByTagName("valor")[0]?.InnerText
                    });
                }
            }

            detalles.Add(detail);
            idDetalle++;
        }

        comprobante.CreateTaxes(impuestos);
        comprobante.CreateDetails(detalles);
    }
}