namespace Acontplus.FactElect.Services.Conversion;

public class FacturaDocumentParser(IDetailsParser detailsParser) : IDocumentTypeParser
{
    private readonly IDetailsParser _detailsParser = detailsParser ?? throw new ArgumentNullException(nameof(detailsParser));

    public bool Parse(XmlDocument xmlDocument, ComprobanteElectronico comprobante, out string errorMessage)
    {
        errorMessage = string.Empty;

        try
        {
            var nodeFact = xmlDocument.GetElementsByTagName("factura")[0];
            comprobante.versionComp = nodeFact?.Attributes?["version"]?.Value;

            var nodeInfoFactura = xmlDocument.GetElementsByTagName("infoFactura")[0];
            if (nodeInfoFactura != null)
            {
                ParseInfoFactura(nodeInfoFactura, comprobante);
            }
            else
            {
                errorMessage = "No invoice information found in document";
                return false;
            }

            var nodeDetails = xmlDocument.GetElementsByTagName("detalles")[0];
            if (nodeDetails != null)
            {
                _detailsParser.Parse(nodeDetails, comprobante);
            }

            return true;
        }
        catch (Exception ex)
        {
            errorMessage = $"Error parsing invoice document: {ex.Message}";
            return false;
        }
    }

    private void ParseInfoFactura(XmlNode nodeInfoFactura, ComprobanteElectronico comprobante)
    {
        var infoFac = new InfoFactura
        {
            fechaEmision = nodeInfoFactura.SelectSingleNode("fechaEmision")?.InnerText,
            dirEstablecimiento = nodeInfoFactura.SelectSingleNode("dirEstablecimiento")?.InnerText ?? "",
            contribuyenteEspecial = nodeInfoFactura.SelectSingleNode("contribuyenteEspecial")?.InnerText ?? "",
            obligadoContabilidad = nodeInfoFactura.SelectSingleNode("obligadoContabilidad")?.InnerText ?? "",
            tipoIdentificacionComprador = nodeInfoFactura.SelectSingleNode("tipoIdentificacionComprador")?.InnerText,
            razonSocialComprador = nodeInfoFactura.SelectSingleNode("razonSocialComprador")?.InnerText,
            identificacionComprador = nodeInfoFactura.SelectSingleNode("identificacionComprador")?.InnerText,
            direccionComprador = nodeInfoFactura.SelectSingleNode("direccionComprador")?.InnerText ?? "",
            guiaRemision = nodeInfoFactura.SelectSingleNode("guiaRemision")?.InnerText ?? "",
            totalSinImpuestos = nodeInfoFactura.SelectSingleNode("totalSinImpuestos")?.InnerText,
            totalDescuento = nodeInfoFactura.SelectSingleNode("totalDescuento")?.InnerText,
            propina = nodeInfoFactura.SelectSingleNode("propina")?.InnerText ?? "0.00",
            importeTotal = nodeInfoFactura.SelectSingleNode("importeTotal")?.InnerText,
            moneda = nodeInfoFactura.SelectSingleNode("moneda")?.InnerText ?? ""
        };

        ParseTotalTaxes(comprobante.codDoc, infoFac, nodeInfoFactura.SelectSingleNode("totalConImpuestos"));

        var pagosNode = nodeInfoFactura.SelectSingleNode("pagos");
        if (pagosNode != null)
        {
            ParseInvoicePayments(infoFac, pagosNode);
        }

        comprobante.CreateInfoComp(comprobante.codDoc, infoFac);
    }

    private void ParseTotalTaxes(string codDoc, InfoFactura infoFac, XmlNode impuestos)
    {
        var totalImpuestos = (from XmlNode item in impuestos
                              select new TotalImpuesto
                              {
                                  codigo = item.SelectSingleNode("codigo")?.InnerText,
                                  codigoPorcentaje = item.SelectSingleNode("codigoPorcentaje")?.InnerText,
                                  descuentoAdicional = item.SelectSingleNode("descuentoAdicional")?.InnerText ?? "0.00",
                                  baseImponible = item.SelectSingleNode("baseImponible")?.InnerText,
                                  valor = item.SelectSingleNode("valor")?.InnerText
                              }).ToList();

        infoFac.CreateTotalTaxes(totalImpuestos);
    }

    private static void ParseInvoicePayments(InfoFactura infoFac, XmlNode payments)
    {
        var pagos = (from XmlNode item in payments
                     select new Pago
                     {
                         formaPago = item.SelectSingleNode("formaPago")?.InnerText,
                         total = item.SelectSingleNode("total")?.InnerText,
                         plazo = item.SelectSingleNode("plazo")?.InnerText ?? "",
                         unidadTiempo = item.SelectSingleNode("unidadTiempo")?.InnerText ?? ""
                     }).ToList();

        infoFac.CreatePayments(pagos);
    }
}