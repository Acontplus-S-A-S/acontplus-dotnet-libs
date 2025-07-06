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
            var detail = new Detalle { IdDetalle = idDetalle };

            if (comprobante.CodDoc == "01")
                detail.CodigoPrincipal = item.SelectSingleNode("codigoPrincipal")?.InnerText ?? "";
            else
                detail.CodigoPrincipal = item.SelectSingleNode("codigoInterno")?.InnerText ?? "";

            detail.CodigoAuxiliar = item.SelectSingleNode("codigoAuxiliar")?.InnerText ?? "";
            detail.Descripcion = item.SelectSingleNode("descripcion")?.InnerText;
            detail.Cantidad = item.SelectSingleNode("cantidad")?.InnerText;
            detail.PrecioUnitario = item.SelectSingleNode("precioUnitario")?.InnerText;
            detail.Descuento = item.SelectSingleNode("descuento")?.InnerText ?? "";
            detail.PrecioTotalSinImpuesto = item.SelectSingleNode("precioTotalSinImpuesto")?.InnerText;
            detail.Impuestos = item.SelectSingleNode("impuestos") == null
                ? ""
                : item.SelectNodes("impuestos")?[0]?.OuterXml;
            detail.DetallesAdicionales = item.SelectSingleNode("detallesAdicionales") == null
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
                        IdDetalle = idDetalle,
                        CodArticulo = detail.CodigoPrincipal,
                        Codigo = taxes.GetElementsByTagName("codigo")[0]?.InnerText,
                        CodigoPorcentaje = taxes.GetElementsByTagName("codigoPorcentaje")[0]?.InnerText,
                        Tarifa = taxes.GetElementsByTagName("tarifa")[0]?.InnerText,
                        BaseImponible = taxes.GetElementsByTagName("baseImponible")[0]?.InnerText,
                        Valor = taxes.GetElementsByTagName("valor")[0]?.InnerText
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