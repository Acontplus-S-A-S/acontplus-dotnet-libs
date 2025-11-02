using Acontplus.Billing.Interfaces.Services;
using Acontplus.Billing.Models.Documents;

namespace Acontplus.Billing.Services.Conversion;

public class FacturaDocumentParser(IDetailsParser detailsParser) : IDocumentTypeParser
{
    private readonly IDetailsParser _detailsParser = detailsParser ?? throw new ArgumentNullException(nameof(detailsParser));

    public bool Parse(XmlDocument xmlDocument, ComprobanteElectronico comprobante, out string errorMessage)
    {
        errorMessage = string.Empty;

        try
        {
            var nodeFact = xmlDocument.GetElementsByTagName("factura")[0];
            comprobante.VersionComp = nodeFact?.Attributes?["version"]?.Value;

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
            FechaEmision = nodeInfoFactura.SelectSingleNode("fechaEmision")?.InnerText,
            DirEstablecimiento = nodeInfoFactura.SelectSingleNode("dirEstablecimiento")?.InnerText ?? "",
            ContribuyenteEspecial = nodeInfoFactura.SelectSingleNode("contribuyenteEspecial")?.InnerText ?? "",
            ObligadoContabilidad = nodeInfoFactura.SelectSingleNode("obligadoContabilidad")?.InnerText ?? "",
            TipoIdentificacionComprador = nodeInfoFactura.SelectSingleNode("tipoIdentificacionComprador")?.InnerText,
            RazonSocialComprador = nodeInfoFactura.SelectSingleNode("razonSocialComprador")?.InnerText,
            IdentificacionComprador = nodeInfoFactura.SelectSingleNode("identificacionComprador")?.InnerText,
            DireccionComprador = nodeInfoFactura.SelectSingleNode("direccionComprador")?.InnerText ?? "",
            GuiaRemision = nodeInfoFactura.SelectSingleNode("guiaRemision")?.InnerText ?? "",
            TotalSinImpuestos = nodeInfoFactura.SelectSingleNode("totalSinImpuestos")?.InnerText,
            TotalDescuento = nodeInfoFactura.SelectSingleNode("totalDescuento")?.InnerText,
            Propina = nodeInfoFactura.SelectSingleNode("propina")?.InnerText ?? "0.00",
            ImporteTotal = nodeInfoFactura.SelectSingleNode("importeTotal")?.InnerText,
            Moneda = nodeInfoFactura.SelectSingleNode("moneda")?.InnerText ?? ""
        };

        ParseTotalTaxes(comprobante.CodDoc, infoFac, nodeInfoFactura.SelectSingleNode("totalConImpuestos"));

        var pagosNode = nodeInfoFactura.SelectSingleNode("pagos");
        if (pagosNode != null)
        {
            ParseInvoicePayments(infoFac, pagosNode);
        }

        comprobante.CreateInfoComp(comprobante.CodDoc, infoFac);
    }

    private void ParseTotalTaxes(string codDoc, InfoFactura infoFac, XmlNode impuestos)
    {
        var totalImpuestos = (from XmlNode item in impuestos
                              select new TotalImpuesto
                              {
                                  Codigo = item.SelectSingleNode("codigo")?.InnerText,
                                  CodigoPorcentaje = item.SelectSingleNode("codigoPorcentaje")?.InnerText,
                                  DescuentoAdicional = item.SelectSingleNode("descuentoAdicional")?.InnerText ?? "0.00",
                                  BaseImponible = item.SelectSingleNode("baseImponible")?.InnerText,
                                  Valor = item.SelectSingleNode("valor")?.InnerText
                              }).ToList();

        infoFac.CreateTotalTaxes(totalImpuestos);
    }

    private static void ParseInvoicePayments(InfoFactura infoFac, XmlNode payments)
    {
        var pagos = (from XmlNode item in payments
                     select new Pago
                     {
                         FormaPago = item.SelectSingleNode("formaPago")?.InnerText,
                         Total = item.SelectSingleNode("total")?.InnerText,
                         Plazo = item.SelectSingleNode("plazo")?.InnerText ?? "",
                         UnidadTiempo = item.SelectSingleNode("unidadTiempo")?.InnerText ?? ""
                     }).ToList();

        infoFac.CreatePayments(pagos);
    }
}