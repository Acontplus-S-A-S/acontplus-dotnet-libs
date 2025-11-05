using Acontplus.Billing.Models.Documents;

namespace Acontplus.Billing.Services.Documents;

public class DataXmlComprobante
{
    public bool GetData(XmlDocument xmlSri, ref ComprobanteElectronico comp, ref string message)
    {
        var resp = true;
        try
        {
            var nodeAuth = xmlSri.GetElementsByTagName("autorizacion")[0];
            var nodeComp = nodeAuth?.SelectSingleNode("comprobante");
            var xmlComp = new XmlDocument();
            if (nodeComp != null)
                xmlComp.LoadXml(nodeComp.InnerText);
            else
                return false;

            comp.NumeroAutorizacion = nodeAuth.SelectSingleNode("numeroAutorizacion")?.InnerText;
            comp.FechaAutorizacion = nodeAuth.SelectSingleNode("fechaAutorizacion")?.InnerText;

            var nodeInfoTrib = xmlComp.GetElementsByTagName("infoTributaria")[0];
            if (nodeInfoTrib != null)
            {
                comp.CodDoc = nodeInfoTrib.SelectSingleNode("codDoc")?.InnerText;
                GetInfoTributaria(comp, nodeInfoTrib);
            }

            switch (comp.CodDoc)
            {
                case "01":
                    var nodeFact = xmlComp.GetElementsByTagName("factura")[0];
                    comp.VersionComp = nodeFact?.Attributes?["version"]?.Value;

                    var nodeInfoFactura = xmlComp.GetElementsByTagName("infoFactura")[0];
                    if (nodeInfoFactura != null) GetInfoFactura(comp.CodDoc, comp, nodeInfoFactura);

                    GetDetails(comp, xmlComp.GetElementsByTagName("detalles")[0]);

                    break;
                case "03":
                    var nodeLiq = xmlComp.GetElementsByTagName("liquidacionCompra")[0];
                    comp.VersionComp = nodeLiq?.Attributes?["version"]?.Value;
                    break;
                case "04":
                    var nodeNc = xmlComp.GetElementsByTagName("notaCredito")[0];
                    comp.VersionComp = nodeNc?.Attributes?["version"]?.Value;

                    var nodeInfoNotaCredito = xmlComp.GetElementsByTagName("infoNotaCredito")[0];

                    if (nodeInfoNotaCredito != null) GetInfoNotaCredito(comp.CodDoc, comp, nodeInfoNotaCredito);

                    GetDetails(comp, xmlComp.GetElementsByTagName("detalles")[0]);

                    break;
                case "05":
                    var nodeNd = xmlComp.GetElementsByTagName("notaDebito")[0];
                    comp.VersionComp = nodeNd?.Attributes?["version"]?.Value;
                    break;
                case "06":
                    var nodeGr = xmlComp.GetElementsByTagName("guiaRemision")[0];
                    comp.VersionComp = nodeGr?.Attributes?["version"]?.Value;
                    break;
                case "07":
                    var nodeRet = xmlComp.GetElementsByTagName("comprobanteRetencion")[0];
                    comp.VersionComp = nodeRet?.Attributes?["version"]?.Value;

                    var nodeInfoCompRetencion = xmlComp.GetElementsByTagName("infoCompRetencion")[0];

                    GetInfoCompRetencion(comp.VersionComp, comp, nodeInfoCompRetencion);

                    if (comp.VersionComp == "2.0.0")
                        GetDocSustento(comp, xmlComp.GetElementsByTagName("docsSustento")[0]);
                    else
                        GetImpuestoRetencion(comp, xmlComp.GetElementsByTagName("impuestos")[0]);

                    break;
            }

            if (xmlComp.GetElementsByTagName("infoAdicional")[0] != null)
                GetInfoAdicional(comp, xmlComp.GetElementsByTagName("infoAdicional")[0]);
        }
        catch (Exception ex)
        {
            resp = false;
            message = "Error al obtener la informacion del comprobante: " + ex.Message;
        }

        return resp;
    }

    private void GetInfoTributaria(ComprobanteElectronico ce, XmlNode nodeInfoTrib)
    {
        ce.InfoTributaria = new InfoTributaria
        {
            Ambiente = nodeInfoTrib.SelectSingleNode("ambiente")?.InnerText,
            TipoEmision = nodeInfoTrib.SelectSingleNode("tipoEmision")?.InnerText,
            RazonSocial = nodeInfoTrib.SelectSingleNode("razonSocial")?.InnerText,
            NombreComercial =
                nodeInfoTrib.SelectSingleNode("nombreComercial") == null
                    ? ""
                    : nodeInfoTrib.SelectSingleNode("nombreComercial")?.InnerText,
            Ruc = nodeInfoTrib.SelectSingleNode("ruc")?.InnerText,
            ClaveAcceso = nodeInfoTrib.SelectSingleNode("claveAcceso")?.InnerText,
            CodDoc = nodeInfoTrib.SelectSingleNode("codDoc")?.InnerText,
            Estab = nodeInfoTrib.SelectSingleNode("estab")?.InnerText,
            PtoEmi = nodeInfoTrib.SelectSingleNode("ptoEmi")?.InnerText,
            Secuencial = nodeInfoTrib.SelectSingleNode("secuencial")?.InnerText,
            DirMatriz = nodeInfoTrib.SelectSingleNode("dirMatriz")?.InnerText
        };
    }

    public void GetInfoNotaCredito(string codDoc, ComprobanteElectronico ce, XmlNode nodeInfoNotaCredito)
    {
        var infoFac = new InfoNotaCredito
        {
            FechaEmision = nodeInfoNotaCredito.SelectSingleNode("fechaEmision")?.InnerText,
            DirEstablecimiento = nodeInfoNotaCredito.SelectSingleNode("dirEstablecimiento") == null
                ? ""
                : nodeInfoNotaCredito.SelectSingleNode("dirEstablecimiento")?.InnerText,
            TipoIdentificacionComprador =
                nodeInfoNotaCredito.SelectSingleNode("tipoIdentificacionComprador")?.InnerText,
            RazonSocialComprador = nodeInfoNotaCredito.SelectSingleNode("razonSocialComprador")?.InnerText,
            IdentificacionComprador = nodeInfoNotaCredito.SelectSingleNode("identificacionComprador")?.InnerText,
            ContribuyenteEspecial = nodeInfoNotaCredito.SelectSingleNode("contribuyenteEspecial") == null
                ? ""
                : nodeInfoNotaCredito.SelectSingleNode("contribuyenteEspecial")?.InnerText,
            ObligadoContabilidad = nodeInfoNotaCredito.SelectSingleNode("obligadoContabilidad") == null
                ? ""
                : nodeInfoNotaCredito.SelectSingleNode("obligadoContabilidad")?.InnerText,
            Rise = nodeInfoNotaCredito.SelectSingleNode("rise") == null
                ? ""
                : nodeInfoNotaCredito.SelectSingleNode("rise")?.InnerText,
            CodDocModificado = nodeInfoNotaCredito.SelectSingleNode("codDocModificado")?.InnerText,
            NumDocModificado = nodeInfoNotaCredito.SelectSingleNode("numDocModificado")?.InnerText,
            FechaEmisionDocSustento = nodeInfoNotaCredito.SelectSingleNode("fechaEmisionDocSustento") == null
                ? ""
                : nodeInfoNotaCredito.SelectSingleNode("fechaEmisionDocSustento")?.InnerText,
            TotalSinImpuestos = nodeInfoNotaCredito.SelectSingleNode("totalSinImpuestos")?.InnerText,
            ValorModificacion = nodeInfoNotaCredito.SelectSingleNode("valorModificacion") == null
                ? ""
                : nodeInfoNotaCredito.SelectSingleNode("valorModificacion")?.InnerText,
            Moneda = nodeInfoNotaCredito.SelectSingleNode("moneda") == null
                ? ""
                : nodeInfoNotaCredito.SelectSingleNode("moneda")?.InnerText,
            Motivo = nodeInfoNotaCredito.SelectSingleNode("motivo") == null
                ? ""
                : nodeInfoNotaCredito.SelectSingleNode("motivo")?.InnerText
        };
        GetTotalTaxes(codDoc, infoFac, nodeInfoNotaCredito.SelectSingleNode("totalConImpuestos"));
        ce.CreateInfoComp(codDoc, infoFac);
    }

    public void GetInfoAdicional(ComprobanteElectronico comp, XmlNode infoAdi)
    {
        var infoAdicionals = (from XmlNode item in infoAdi
                              select new InfoAdicional
                              {
                                  Nombre = item.Attributes?.GetNamedItem("nombre")!.Value,
                                  Valor = item.InnerText
                              })
            .ToList();

        comp.CreateAdditionalInfo(infoAdicionals);
    }

    private void GetInfoFactura(string codDoc, ComprobanteElectronico ce, XmlNode nodeInfoFactura)
    {
        var infoFac = new InfoFactura();
        infoFac.FechaEmision = nodeInfoFactura.SelectSingleNode("fechaEmision")?.InnerText;
        infoFac.DirEstablecimiento = nodeInfoFactura.SelectSingleNode("dirEstablecimiento") == null
            ? ""
            : nodeInfoFactura.SelectSingleNode("dirEstablecimiento")?.InnerText;
        infoFac.ContribuyenteEspecial = nodeInfoFactura.SelectSingleNode("contribuyenteEspecial") == null
            ? ""
            : nodeInfoFactura.SelectSingleNode("contribuyenteEspecial")?.InnerText;
        infoFac.ObligadoContabilidad = nodeInfoFactura.SelectSingleNode("obligadoContabilidad") == null
            ? ""
            : nodeInfoFactura.SelectSingleNode("obligadoContabilidad")?.InnerText;
        infoFac.TipoIdentificacionComprador =
            nodeInfoFactura.SelectSingleNode("tipoIdentificacionComprador")?.InnerText;
        infoFac.RazonSocialComprador = nodeInfoFactura.SelectSingleNode("razonSocialComprador")?.InnerText;
        infoFac.IdentificacionComprador = nodeInfoFactura.SelectSingleNode("identificacionComprador")?.InnerText;
        infoFac.DireccionComprador = nodeInfoFactura.SelectSingleNode("direccionComprador") == null
            ? ""
            : nodeInfoFactura.SelectSingleNode("direccionComprador")?.InnerText;
        infoFac.GuiaRemision = nodeInfoFactura.SelectSingleNode("guiaRemision") == null
            ? ""
            : nodeInfoFactura.SelectSingleNode("guiaRemision")?.InnerText;
        infoFac.TotalSinImpuestos = nodeInfoFactura.SelectSingleNode("totalSinImpuestos")?.InnerText;
        infoFac.TotalDescuento = nodeInfoFactura.SelectSingleNode("totalDescuento")?.InnerText;
        infoFac.Propina = nodeInfoFactura.SelectSingleNode("propina") == null
            ? "0.00"
            : nodeInfoFactura.SelectSingleNode("propina")?.InnerText;
        infoFac.ImporteTotal = nodeInfoFactura.SelectSingleNode("importeTotal")?.InnerText;
        infoFac.Moneda = nodeInfoFactura.SelectSingleNode("moneda") == null
            ? ""
            : nodeInfoFactura.SelectSingleNode("moneda")?.InnerText;

        GetTotalTaxes(codDoc, infoFac, nodeInfoFactura.SelectSingleNode("totalConImpuestos"));

        if (nodeInfoFactura.SelectSingleNode("pagos") != null)
            GetInvoicePayments(infoFac, nodeInfoFactura.SelectSingleNode("pagos"));

        ce.CreateInfoComp(codDoc, infoFac);
    }

    private void GetTotalTaxes(string codDoc, object obj, XmlNode impuestos)
    {
        var totalImpuestos = (from XmlNode item in impuestos
                              select new TotalImpuesto
                              {
                                  Codigo = item.SelectSingleNode("codigo")?.InnerText,
                                  CodigoPorcentaje = item.SelectSingleNode("codigoPorcentaje")?.InnerText,
                                  DescuentoAdicional = item.SelectSingleNode("descuentoAdicional") == null
                                      ? "0.00"
                                      : item.SelectSingleNode("descuentoAdicional")?.InnerText,
                                  BaseImponible = item.SelectSingleNode("baseImponible")?.InnerText,
                                  Valor = item.SelectSingleNode("valor")?.InnerText
                              }).ToList();

        switch (codDoc)
        {
            case "01":
                var infoFactura = obj as InfoFactura;
                infoFactura?.CreateTotalTaxes(totalImpuestos);
                break;
            case "04":
                var infoNotaCredito = obj as InfoNotaCredito;
                infoNotaCredito?.CreateTotalTaxes(totalImpuestos);
                break;
        }
    }

    private void GetInvoicePayments(InfoFactura comp, XmlNode payments)
    {
        var pagos = (from XmlNode item in payments
                     select new Pago
                     {
                         FormaPago = item.SelectSingleNode("formaPago")?.InnerText,
                         Total = item.SelectSingleNode("total")?.InnerText,
                         Plazo = item.SelectSingleNode("plazo") == null ? "" : item.SelectSingleNode("plazo")?.InnerText,
                         UnidadTiempo = item.SelectSingleNode("unidadTiempo") == null
                             ? ""
                             : item.SelectSingleNode("unidadTiempo")?.InnerText
                     }).ToList();

        comp.CreatePayments(pagos);
    }

    private void GetDetails(ComprobanteElectronico comp, XmlNode details)
    {
        var detalles = new List<Detalle>();
        var impuestos = new List<Impuesto>();
        var idDetalle = 0;
        foreach (XmlNode item in details)
        {
            var detail = new Detalle { IdDetalle = idDetalle };
            detail.CodigoPrincipal = comp.CodDoc == "01"
                ? item.SelectSingleNode("codigoPrincipal") == null
                    ? ""
                    : item.SelectSingleNode("codigoPrincipal")?.InnerText
                : item.SelectSingleNode("codigoInterno") == null
                    ? ""
                    : item.SelectSingleNode("codigoInterno")?.InnerText;

            detail.CodigoAuxiliar = item.SelectSingleNode("codigoAuxiliar") == null
                ? ""
                : item.SelectSingleNode("codigoAuxiliar")?.InnerText;
            detail.Descripcion = item.SelectSingleNode("descripcion")?.InnerText;
            detail.Cantidad = item.SelectSingleNode("cantidad")?.InnerText;
            detail.PrecioUnitario = item.SelectSingleNode("precioUnitario")?.InnerText;
            detail.Descuento = item.SelectSingleNode("descuento") == null
                ? ""
                : item.SelectSingleNode("descuento")?.InnerText;
            detail.PrecioTotalSinImpuesto = item.SelectSingleNode("precioTotalSinImpuesto")?.InnerText;
            detail.Impuestos = item.SelectSingleNode("impuestos") == null
                ? ""
                : item.SelectNodes("impuestos")?[0]?.OuterXml;
            detail.DetallesAdicionales = item.SelectSingleNode("detallesAdicionales") == null
                ? ""
                : item.SelectNodes("detallesAdicionales")?[0]?.OuterXml;
            impuestos.AddRange(from XmlElement taxes in item.SelectNodes("impuestos")
                               select new Impuesto
                               {
                                   IdDetalle = idDetalle,
                                   CodArticulo = detail.CodigoPrincipal,
                                   Codigo = taxes.GetElementsByTagName("codigo")[0]?.InnerText,
                                   CodigoPorcentaje = taxes.GetElementsByTagName("codigoPorcentaje")[0]?.InnerText,
                                   Tarifa = taxes.GetElementsByTagName("tarifa")[0]?.InnerText,
                                   BaseImponible = taxes.GetElementsByTagName("baseImponible")[0]?.InnerText,
                                   Valor = taxes.GetElementsByTagName("valor")[0]?.InnerText
                               });

            detalles.Add(detail);
            idDetalle++;
        }

        comp.CreateTaxes(impuestos);

        comp.CreateDetails(detalles);
    }

    private void GetInfoCompRetencion(string? versionComp, ComprobanteElectronico ce, XmlNode nodeInfoCompRetencion)
    {
        if (nodeInfoCompRetencion != null)
        {
            var infoRet = new InfoCompRetencion
            {
                FechaEmision = nodeInfoCompRetencion.SelectSingleNode("fechaEmision")?.InnerText,
                DirEstablecimiento = nodeInfoCompRetencion.SelectSingleNode("dirEstablecimiento") == null
                    ? ""
                    : nodeInfoCompRetencion.SelectSingleNode("dirEstablecimiento")?.InnerText,
                ContribuyenteEspecial = nodeInfoCompRetencion.SelectSingleNode("contribuyenteEspecial") == null
                    ? ""
                    : nodeInfoCompRetencion.SelectSingleNode("contribuyenteEspecial")?.InnerText,
                ObligadoContabilidad = nodeInfoCompRetencion.SelectSingleNode("obligadoContabilidad")?.InnerText,
                TipoIdentificacionSujetoRetenido = nodeInfoCompRetencion.SelectSingleNode("tipoIdentificacionSujetoRetenido")?.InnerText,
                RazonSocialSujetoRetenido = nodeInfoCompRetencion.SelectSingleNode("razonSocialSujetoRetenido")?.InnerText,
                IdentificacionSujetoRetenido = nodeInfoCompRetencion.SelectSingleNode("identificacionSujetoRetenido")?.InnerText,
                PeriodoFiscal = nodeInfoCompRetencion.SelectSingleNode("periodoFiscal") == null
                    ? ""
                    : nodeInfoCompRetencion.SelectSingleNode("periodoFiscal")?.InnerText
            };
            if (versionComp == "2.0.0")
            {
                infoRet.ParteRel = nodeInfoCompRetencion.SelectSingleNode("parteRel")?.InnerText;
                infoRet.TipoSujetoRetenido = nodeInfoCompRetencion.SelectSingleNode("tipoSujetoRetenido") == null
                    ? ""
                    : nodeInfoCompRetencion.SelectSingleNode("tipoSujetoRetenido")?.InnerText;
            }

            ce.CreateInfoComp("07", infoRet);
        }
    }

    private void GetImpuestoRetencion(ComprobanteElectronico comp, XmlNode impuestosRet)
    {
        var impuestos = new List<ImpuestoRetencion>();
        foreach (XmlElement taxes in impuestosRet)
        {
            var tax = new ImpuestoRetencion
            {
                Codigo = taxes.GetElementsByTagName("codigo")[0]?.InnerText,
                CodigoRetencion = taxes.GetElementsByTagName("codigoRetencion")[0]?.InnerText,
                BaseImponible = taxes.GetElementsByTagName("baseImponible")[0]?.InnerText,
                PorcentajeRetener = taxes.GetElementsByTagName("porcentajeRetener")[0]?.InnerText,
                ValorRetenido = taxes.GetElementsByTagName("valorRetenido")[0]?.InnerText,
                CodDocSustento = taxes.GetElementsByTagName("codDocSustento")[0]?.InnerText,
                NumDocSustento = taxes.GetElementsByTagName("numDocSustento")[0]?.InnerText,
                FechaEmisionDocSustento = taxes.GetElementsByTagName("fechaEmisionDocSustento")[0]?.InnerText
            };
            impuestos.Add(tax);
        }

        comp.CreateRetencionTaxes(impuestos);
    }

    private void GetDocSustento(ComprobanteElectronico ce, XmlNode nodeDocsSustento)
    {
        var docsSustento = new List<DocSustento>();
        foreach (XmlElement item in nodeDocsSustento)
        {
            var docSustento = new DocSustento
            {
                CodSustento = item.GetElementsByTagName("codSustento")[0]?.InnerText,
                CodDocSustento = item.GetElementsByTagName("codDocSustento")[0]?.InnerText,
                NumDocSustento = item.GetElementsByTagName("numDocSustento")[0]?.InnerText,
                FechaEmisionDocSustento = item.GetElementsByTagName("fechaEmisionDocSustento")[0]?.InnerText,
                NumAutDocSustento = item.SelectSingleNode("numAutDocSustento") == null
                    ? ""
                    : item.GetElementsByTagName("numAutDocSustento")[0]?.InnerText,
                PagoLocExt = item.GetElementsByTagName("pagoLocExt")[0]?.InnerText,
                TipoRegi = item.SelectSingleNode("tipoRegi") == null
                    ? ""
                    : item.GetElementsByTagName("tipoRegi")[0]?.InnerText,
                PaisEfecPago = item.SelectSingleNode("paisEfecPago") == null
                    ? ""
                    : item.GetElementsByTagName("paisEfecPago")[0]?.InnerText,
                AplicConvDobTrib = item.SelectSingleNode("aplicConvDobTrib") == null
                    ? ""
                    : item.GetElementsByTagName("aplicConvDobTrib")[0]?.InnerText,
                PagExtSujRetNorLeg = item.SelectSingleNode("pagExtSujRetNorLeg") == null
                    ? ""
                    : item.GetElementsByTagName("pagExtSujRetNorLeg")[0]?.InnerText,
                PagoRegFis = item.SelectSingleNode("pagoRegFis") == null
                    ? ""
                    : item.GetElementsByTagName("pagoRegFis")[0]?.InnerText,
                TotalComprobantesReembolso = item.SelectSingleNode("totalComprobantesReembolso") == null
                    ? ""
                    : item.GetElementsByTagName("totalComprobantesReembolso")[0]?.InnerText,
                TotalBaseImponibleReembolso = item.SelectSingleNode("totalBaseImponibleReembolso") == null
                    ? ""
                    : item.GetElementsByTagName("totalBaseImponibleReembolso")[0]?.InnerText,
                TotalImpuestoReembolso = item.SelectSingleNode("totalImpuestoReembolso") == null
                    ? ""
                    : item.GetElementsByTagName("totalImpuestoReembolso")[0]?.InnerText,
                TotalSinImpuestos = item.GetElementsByTagName("totalSinImpuestos")[0]?.InnerText,
                ImporteTotal = item.GetElementsByTagName("importeTotal")[0]?.InnerText
            };
            GetImpuestoDocSustento(docSustento, item.SelectSingleNode("impuestosDocSustento"));
            GetRetenciones(docSustento, item.SelectSingleNode("retenciones"), ce);
            if (docSustento.CodDocSustento == "41") GetReembolsos(docSustento, item.SelectSingleNode("reembolsos"));

            if (item.SelectSingleNode("pagos") != null)
                GetRetencionPayments(docSustento, item.SelectSingleNode("pagos"));

            docsSustento.Add(docSustento);
        }

        ce.CreateDocSustentos(docsSustento);
    }

    private void GetImpuestoDocSustento(DocSustento doc, XmlNode nodeImpuestos)
    {
        var impuestos = (from XmlElement item in nodeImpuestos
                         select new ImpuestoDocSustento
                         {
                             CodImpuestoDocSustento = item.GetElementsByTagName("codImpuestoDocSustento")[0]?.InnerText,
                             CodigoPorcentaje = item.GetElementsByTagName("codigoPorcentaje")[0]?.InnerText,
                             BaseImponible = item.GetElementsByTagName("baseImponible")[0]?.InnerText,
                             Tarifa = item.GetElementsByTagName("tarifa")[0]?.InnerText,
                             ValorImpuesto = item.GetElementsByTagName("valorImpuesto")[0]?.InnerText
                         }).ToList();

        doc.CreateTax(impuestos);
    }

    private void GetRetenciones(DocSustento doc, XmlNode nodeRetenciones, ComprobanteElectronico ce)
    {
        var retenciones = new List<Retencion>();
        var retToView = new List<ImpuestoRetencion>();

        foreach (XmlElement item in nodeRetenciones)
        {
            var retencion = new Retencion
            {
                Codigo = item.GetElementsByTagName("codigo")[0]?.InnerText,
                CodigoRetencion = item.GetElementsByTagName("codigoRetencion")[0]?.InnerText,
                BaseImponible = item.GetElementsByTagName("baseImponible")[0]?.InnerText,
                PorcentajeRetener = item.GetElementsByTagName("porcentajeRetener")[0]?.InnerText,
                ValorRetenido = item.GetElementsByTagName("valorRetenido")[0]?.InnerText
            };
            retenciones.Add(retencion);

            var ir = new ImpuestoRetencion
            {
                Codigo = item.GetElementsByTagName("codigo")[0]?.InnerText,
                CodigoRetencion = item.GetElementsByTagName("codigoRetencion")[0]?.InnerText,
                BaseImponible = item.GetElementsByTagName("baseImponible")[0]?.InnerText,
                PorcentajeRetener = item.GetElementsByTagName("porcentajeRetener")[0]?.InnerText,
                ValorRetenido = item.GetElementsByTagName("valorRetenido")[0]?.InnerText,
                CodDocSustento = doc.CodDocSustento,
                NumDocSustento = doc.NumDocSustento,
                FechaEmisionDocSustento = doc.FechaEmisionDocSustento
            };
            retToView.Add(ir);
        }

        ce.CreateRetencionTaxes(retToView);
        doc.CreateRetencion(retenciones);
    }

    private void GetReembolsos(DocSustento doc, XmlNode nodeReemb)
    {
        var reembolsos = new List<ReembolsoDetalle>();
        foreach (XmlElement item in nodeReemb)
        {
            var reembolso = new ReembolsoDetalle
            {
                TipoIdentificacionProveedorReembolso = item.GetElementsByTagName("tipoIdentificacionProveedorReembolso")[0]?.InnerText,
                IdentificacionProveedorReembolso = item.GetElementsByTagName("identificacionProveedorReembolso")[0]?.InnerText,
                CodPaisPagoProveedorReembolso = item.GetElementsByTagName("codPaisPagoProveedorReembolso")[0]?.InnerText,
                TipoProveedorReembolso = item.GetElementsByTagName("tipoProveedorReembolso")[0]?.InnerText,
                CodDocReembolso = item.GetElementsByTagName("codDocReembolso")[0]?.InnerText,
                EstabDocReembolso = item.GetElementsByTagName("estabDocReembolso")[0]?.InnerText,
                PtoEmiDocReembolso = item.GetElementsByTagName("ptoEmiDocReembolso")[0]?.InnerText,
                SecuencialDocReembolso = item.GetElementsByTagName("secuencialDocReembolso")[0]?.InnerText,
                FechaEmisionDocReembolso = item.GetElementsByTagName("fechaEmisionDocReembolso")[0]?.InnerText,
                NumeroAutorizacionDocReemb = item.GetElementsByTagName("numeroAutorizacionDocReemb")[0]?.InnerText
            };
            GetImpuestosReembolsos(reembolso, item.SelectSingleNode("detalleImpuestos"));
            reembolsos.Add(reembolso);
        }

        doc.CreateReembolsos(reembolsos);
    }

    private void GetImpuestosReembolsos(ReembolsoDetalle reembolsoDetalles, XmlNode impuestosReembolso)
    {
        var impuestos = (from XmlElement item in impuestosReembolso
                         select new DetalleImpuesto
                         {
                             Codigo = item.GetElementsByTagName("codigo")[0]?.InnerText,
                             CodigoPorcentaje = item.GetElementsByTagName("codigoPorcentaje")[0]?.InnerText,
                             Tarifa = item.GetElementsByTagName("tarifa")[0]?.InnerText,
                             BaseImponibleReembolso = item.GetElementsByTagName("baseImponibleReembolso")[0]?.InnerText,
                             ImpuestoReembolso = item.GetElementsByTagName("impuestoReembolso")[0]?.InnerText
                         }).ToList();

        reembolsoDetalles.CreateTax(impuestos);
    }

    private void GetRetencionPayments(DocSustento doc, XmlNode payments)
    {
        var pagos = (from XmlNode item in payments select new Pago { FormaPago = item.SelectSingleNode("formaPago")?.InnerText, Total = item.SelectSingleNode("total")?.InnerText }).ToList();

        doc.CreatePayments(pagos);
    }
}