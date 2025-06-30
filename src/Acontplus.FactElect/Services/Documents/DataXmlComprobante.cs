using Acontplus.FactElect.Models.Documents;

namespace Acontplus.FactElect.Services.Documents;

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

            comp.numeroAutorizacion = nodeAuth.SelectSingleNode("numeroAutorizacion")?.InnerText;
            comp.fechaAutorizacion = nodeAuth.SelectSingleNode("fechaAutorizacion")?.InnerText;

            var nodeInfoTrib = xmlComp.GetElementsByTagName("infoTributaria")[0];
            if (nodeInfoTrib != null)
            {
                comp.codDoc = nodeInfoTrib.SelectSingleNode("codDoc")?.InnerText;
                GetInfoTributaria(comp, nodeInfoTrib);
            }

            switch (comp.codDoc)
            {
                case "01":
                    var nodeFact = xmlComp.GetElementsByTagName("factura")[0];
                    comp.versionComp = nodeFact?.Attributes?["version"]?.Value;

                    var nodeInfoFactura = xmlComp.GetElementsByTagName("infoFactura")[0];
                    if (nodeInfoFactura != null) GetInfoFactura(comp.codDoc, comp, nodeInfoFactura);

                    GetDetails(comp, xmlComp.GetElementsByTagName("detalles")[0]);

                    break;
                case "03":
                    var nodeLiq = xmlComp.GetElementsByTagName("liquidacionCompra")[0];
                    comp.versionComp = nodeLiq?.Attributes?["version"]?.Value;
                    break;
                case "04":
                    var nodeNc = xmlComp.GetElementsByTagName("notaCredito")[0];
                    comp.versionComp = nodeNc?.Attributes?["version"]?.Value;

                    var nodeInfoNotaCredito = xmlComp.GetElementsByTagName("infoNotaCredito")[0];

                    if (nodeInfoNotaCredito != null) GetInfoNotaCredito(comp.codDoc, comp, nodeInfoNotaCredito);

                    GetDetails(comp, xmlComp.GetElementsByTagName("detalles")[0]);

                    break;
                case "05":
                    var nodeNd = xmlComp.GetElementsByTagName("notaDebito")[0];
                    comp.versionComp = nodeNd?.Attributes?["version"]?.Value;
                    break;
                case "06":
                    var nodeGr = xmlComp.GetElementsByTagName("guiaRemision")[0];
                    comp.versionComp = nodeGr?.Attributes?["version"]?.Value;
                    break;
                case "07":
                    var nodeRet = xmlComp.GetElementsByTagName("comprobanteRetencion")[0];
                    comp.versionComp = nodeRet?.Attributes?["version"]?.Value;

                    var nodeInfoCompRetencion = xmlComp.GetElementsByTagName("infoCompRetencion")[0];

                    GetInfoCompRetencion(comp.versionComp, comp, nodeInfoCompRetencion);

                    if (comp.versionComp == "2.0.0")
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
        ce.infoTributaria = new InfoTributaria
        {
            ambiente = nodeInfoTrib.SelectSingleNode("ambiente")?.InnerText,
            tipoEmision = nodeInfoTrib.SelectSingleNode("tipoEmision")?.InnerText,
            razonSocial = nodeInfoTrib.SelectSingleNode("razonSocial")?.InnerText,
            nombreComercial =
                nodeInfoTrib.SelectSingleNode("nombreComercial") == null
                    ? ""
                    : nodeInfoTrib.SelectSingleNode("nombreComercial")?.InnerText,
            ruc = nodeInfoTrib.SelectSingleNode("ruc")?.InnerText,
            claveAcceso = nodeInfoTrib.SelectSingleNode("claveAcceso")?.InnerText,
            codDoc = nodeInfoTrib.SelectSingleNode("codDoc")?.InnerText,
            estab = nodeInfoTrib.SelectSingleNode("estab")?.InnerText,
            ptoEmi = nodeInfoTrib.SelectSingleNode("ptoEmi")?.InnerText,
            secuencial = nodeInfoTrib.SelectSingleNode("secuencial")?.InnerText,
            dirMatriz = nodeInfoTrib.SelectSingleNode("dirMatriz")?.InnerText
        };
    }

    public void GetInfoNotaCredito(string codDoc, ComprobanteElectronico ce, XmlNode nodeInfoNotaCredito)
    {
        var infoFac = new InfoNotaCredito
        {
            fechaEmision = nodeInfoNotaCredito.SelectSingleNode("fechaEmision")?.InnerText,
            dirEstablecimiento = nodeInfoNotaCredito.SelectSingleNode("dirEstablecimiento") == null
                ? ""
                : nodeInfoNotaCredito.SelectSingleNode("dirEstablecimiento")?.InnerText,
            tipoIdentificacionComprador =
                nodeInfoNotaCredito.SelectSingleNode("tipoIdentificacionComprador")?.InnerText,
            razonSocialComprador = nodeInfoNotaCredito.SelectSingleNode("razonSocialComprador")?.InnerText,
            identificacionComprador = nodeInfoNotaCredito.SelectSingleNode("identificacionComprador")?.InnerText,
            contribuyenteEspecial = nodeInfoNotaCredito.SelectSingleNode("contribuyenteEspecial") == null
                ? ""
                : nodeInfoNotaCredito.SelectSingleNode("contribuyenteEspecial")?.InnerText,
            obligadoContabilidad = nodeInfoNotaCredito.SelectSingleNode("obligadoContabilidad") == null
                ? ""
                : nodeInfoNotaCredito.SelectSingleNode("obligadoContabilidad")?.InnerText,
            rise = nodeInfoNotaCredito.SelectSingleNode("rise") == null
                ? ""
                : nodeInfoNotaCredito.SelectSingleNode("rise")?.InnerText,
            codDocModificado = nodeInfoNotaCredito.SelectSingleNode("codDocModificado")?.InnerText,
            numDocModificado = nodeInfoNotaCredito.SelectSingleNode("numDocModificado")?.InnerText,
            fechaEmisionDocSustento = nodeInfoNotaCredito.SelectSingleNode("fechaEmisionDocSustento") == null
                ? ""
                : nodeInfoNotaCredito.SelectSingleNode("fechaEmisionDocSustento")?.InnerText,
            totalSinImpuestos = nodeInfoNotaCredito.SelectSingleNode("totalSinImpuestos")?.InnerText,
            valorModificacion = nodeInfoNotaCredito.SelectSingleNode("valorModificacion") == null
                ? ""
                : nodeInfoNotaCredito.SelectSingleNode("valorModificacion")?.InnerText,
            moneda = nodeInfoNotaCredito.SelectSingleNode("moneda") == null
                ? ""
                : nodeInfoNotaCredito.SelectSingleNode("moneda")?.InnerText,
            motivo = nodeInfoNotaCredito.SelectSingleNode("motivo") == null
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
                                  nombre = item.Attributes?.GetNamedItem("nombre")!.Value,
                                  valor = item.InnerText
                              })
            .ToList();

        comp.CreateAdditionalInfo(infoAdicionals);
    }

    private void GetInfoFactura(string codDoc, ComprobanteElectronico ce, XmlNode nodeInfoFactura)
    {
        var infoFac = new InfoFactura();
        infoFac.fechaEmision = nodeInfoFactura.SelectSingleNode("fechaEmision")?.InnerText;
        infoFac.dirEstablecimiento = nodeInfoFactura.SelectSingleNode("dirEstablecimiento") == null
            ? ""
            : nodeInfoFactura.SelectSingleNode("dirEstablecimiento")?.InnerText;
        infoFac.contribuyenteEspecial = nodeInfoFactura.SelectSingleNode("contribuyenteEspecial") == null
            ? ""
            : nodeInfoFactura.SelectSingleNode("contribuyenteEspecial")?.InnerText;
        infoFac.obligadoContabilidad = nodeInfoFactura.SelectSingleNode("obligadoContabilidad") == null
            ? ""
            : nodeInfoFactura.SelectSingleNode("obligadoContabilidad")?.InnerText;
        infoFac.tipoIdentificacionComprador =
            nodeInfoFactura.SelectSingleNode("tipoIdentificacionComprador")?.InnerText;
        infoFac.razonSocialComprador = nodeInfoFactura.SelectSingleNode("razonSocialComprador")?.InnerText;
        infoFac.identificacionComprador = nodeInfoFactura.SelectSingleNode("identificacionComprador")?.InnerText;
        infoFac.direccionComprador = nodeInfoFactura.SelectSingleNode("direccionComprador") == null
            ? ""
            : nodeInfoFactura.SelectSingleNode("direccionComprador")?.InnerText;
        infoFac.guiaRemision = nodeInfoFactura.SelectSingleNode("guiaRemision") == null
            ? ""
            : nodeInfoFactura.SelectSingleNode("guiaRemision")?.InnerText;
        infoFac.totalSinImpuestos = nodeInfoFactura.SelectSingleNode("totalSinImpuestos")?.InnerText;
        infoFac.totalDescuento = nodeInfoFactura.SelectSingleNode("totalDescuento")?.InnerText;
        infoFac.propina = nodeInfoFactura.SelectSingleNode("propina") == null
            ? "0.00"
            : nodeInfoFactura.SelectSingleNode("propina")?.InnerText;
        infoFac.importeTotal = nodeInfoFactura.SelectSingleNode("importeTotal")?.InnerText;
        infoFac.moneda = nodeInfoFactura.SelectSingleNode("moneda") == null
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
                                  codigo = item.SelectSingleNode("codigo")?.InnerText,
                                  codigoPorcentaje = item.SelectSingleNode("codigoPorcentaje")?.InnerText,
                                  descuentoAdicional = item.SelectSingleNode("descuentoAdicional") == null
                                      ? "0.00"
                                      : item.SelectSingleNode("descuentoAdicional")?.InnerText,
                                  baseImponible = item.SelectSingleNode("baseImponible")?.InnerText,
                                  valor = item.SelectSingleNode("valor")?.InnerText
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
                         formaPago = item.SelectSingleNode("formaPago")?.InnerText,
                         total = item.SelectSingleNode("total")?.InnerText,
                         plazo = item.SelectSingleNode("plazo") == null ? "" : item.SelectSingleNode("plazo")?.InnerText,
                         unidadTiempo = item.SelectSingleNode("unidadTiempo") == null
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
            var detail = new Detalle { idDetalle = idDetalle };
            if (comp.codDoc == "01")
                detail.codigoPrincipal = item.SelectSingleNode("codigoPrincipal") == null
                    ? ""
                    : item.SelectSingleNode("codigoPrincipal")?.InnerText;
            else
                detail.codigoPrincipal = item.SelectSingleNode("codigoInterno") == null
                    ? ""
                    : item.SelectSingleNode("codigoInterno")?.InnerText;

            detail.codigoAuxiliar = item.SelectSingleNode("codigoAuxiliar") == null
                ? ""
                : item.SelectSingleNode("codigoAuxiliar")?.InnerText;
            detail.descripcion = item.SelectSingleNode("descripcion")?.InnerText;
            detail.cantidad = item.SelectSingleNode("cantidad")?.InnerText;
            detail.precioUnitario = item.SelectSingleNode("precioUnitario")?.InnerText;
            detail.descuento = item.SelectSingleNode("descuento") == null
                ? ""
                : item.SelectSingleNode("descuento")?.InnerText;
            detail.precioTotalSinImpuesto = item.SelectSingleNode("precioTotalSinImpuesto")?.InnerText;
            detail.impuestos = item.SelectSingleNode("impuestos") == null
                ? ""
                : item.SelectNodes("impuestos")?[0]?.OuterXml;
            detail.detallesAdicionales = item.SelectSingleNode("detallesAdicionales") == null
                ? ""
                : item.SelectNodes("detallesAdicionales")?[0]?.OuterXml;
            impuestos.AddRange(from XmlElement taxes in item.SelectNodes("impuestos")
                               select new Impuesto
                               {
                                   idDetalle = idDetalle,
                                   codArticulo = detail.codigoPrincipal,
                                   codigo = taxes.GetElementsByTagName("codigo")[0]?.InnerText,
                                   codigoPorcentaje = taxes.GetElementsByTagName("codigoPorcentaje")[0]?.InnerText,
                                   tarifa = taxes.GetElementsByTagName("tarifa")[0]?.InnerText,
                                   baseImponible = taxes.GetElementsByTagName("baseImponible")[0]?.InnerText,
                                   valor = taxes.GetElementsByTagName("valor")[0]?.InnerText
                               });

            detalles.Add(detail);
            idDetalle++;
        }

        comp.CreateTaxes(impuestos);

        comp.CreateDetails(detalles);
    }

    private void GetInfoCompRetencion(string versionComp, ComprobanteElectronico ce, XmlNode nodeInfoCompRetencion)
    {
        if (nodeInfoCompRetencion != null)
        {
            var infoRet = new InfoCompRetencion
            {
                fechaEmision = nodeInfoCompRetencion.SelectSingleNode("fechaEmision")?.InnerText,
                dirEstablecimiento = nodeInfoCompRetencion.SelectSingleNode("dirEstablecimiento") == null
                    ? ""
                    : nodeInfoCompRetencion.SelectSingleNode("dirEstablecimiento")?.InnerText,
                contribuyenteEspecial = nodeInfoCompRetencion.SelectSingleNode("contribuyenteEspecial") == null
                    ? ""
                    : nodeInfoCompRetencion.SelectSingleNode("contribuyenteEspecial")?.InnerText,
                obligadoContabilidad = nodeInfoCompRetencion.SelectSingleNode("obligadoContabilidad")?.InnerText,
                tipoIdentificacionSujetoRetenido = nodeInfoCompRetencion.SelectSingleNode("tipoIdentificacionSujetoRetenido")?.InnerText,
                razonSocialSujetoRetenido = nodeInfoCompRetencion.SelectSingleNode("razonSocialSujetoRetenido")?.InnerText,
                identificacionSujetoRetenido = nodeInfoCompRetencion.SelectSingleNode("identificacionSujetoRetenido")?.InnerText,
                periodoFiscal = nodeInfoCompRetencion.SelectSingleNode("periodoFiscal") == null
                    ? ""
                    : nodeInfoCompRetencion.SelectSingleNode("periodoFiscal")?.InnerText
            };
            if (versionComp == "2.0.0")
            {
                infoRet.parteRel = nodeInfoCompRetencion.SelectSingleNode("parteRel")?.InnerText;
                infoRet.tipoSujetoRetenido = nodeInfoCompRetencion.SelectSingleNode("tipoSujetoRetenido") == null
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
                codigo = taxes.GetElementsByTagName("codigo")[0]?.InnerText,
                codigoRetencion = taxes.GetElementsByTagName("codigoRetencion")[0]?.InnerText,
                baseImponible = taxes.GetElementsByTagName("baseImponible")[0]?.InnerText,
                porcentajeRetener = taxes.GetElementsByTagName("porcentajeRetener")[0]?.InnerText,
                valorRetenido = taxes.GetElementsByTagName("valorRetenido")[0]?.InnerText,
                codDocSustento = taxes.GetElementsByTagName("codDocSustento")[0]?.InnerText,
                numDocSustento = taxes.GetElementsByTagName("numDocSustento")[0]?.InnerText,
                fechaEmisionDocSustento = taxes.GetElementsByTagName("fechaEmisionDocSustento")[0]?.InnerText
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
                codSustento = item.GetElementsByTagName("codSustento")[0]?.InnerText,
                codDocSustento = item.GetElementsByTagName("codDocSustento")[0]?.InnerText,
                numDocSustento = item.GetElementsByTagName("numDocSustento")[0]?.InnerText,
                fechaEmisionDocSustento = item.GetElementsByTagName("fechaEmisionDocSustento")[0]?.InnerText,
                numAutDocSustento = item.SelectSingleNode("numAutDocSustento") == null
                    ? ""
                    : item.GetElementsByTagName("numAutDocSustento")[0]?.InnerText,
                pagoLocExt = item.GetElementsByTagName("pagoLocExt")[0]?.InnerText,
                tipoRegi = item.SelectSingleNode("tipoRegi") == null
                    ? ""
                    : item.GetElementsByTagName("tipoRegi")[0]?.InnerText,
                paisEfecPago = item.SelectSingleNode("paisEfecPago") == null
                    ? ""
                    : item.GetElementsByTagName("paisEfecPago")[0]?.InnerText,
                aplicConvDobTrib = item.SelectSingleNode("aplicConvDobTrib") == null
                    ? ""
                    : item.GetElementsByTagName("aplicConvDobTrib")[0]?.InnerText,
                pagExtSujRetNorLeg = item.SelectSingleNode("pagExtSujRetNorLeg") == null
                    ? ""
                    : item.GetElementsByTagName("pagExtSujRetNorLeg")[0]?.InnerText,
                pagoRegFis = item.SelectSingleNode("pagoRegFis") == null
                    ? ""
                    : item.GetElementsByTagName("pagoRegFis")[0]?.InnerText,
                totalComprobantesReembolso = item.SelectSingleNode("totalComprobantesReembolso") == null
                    ? ""
                    : item.GetElementsByTagName("totalComprobantesReembolso")[0]?.InnerText,
                totalBaseImponibleReembolso = item.SelectSingleNode("totalBaseImponibleReembolso") == null
                    ? ""
                    : item.GetElementsByTagName("totalBaseImponibleReembolso")[0]?.InnerText,
                totalImpuestoReembolso = item.SelectSingleNode("totalImpuestoReembolso") == null
                    ? ""
                    : item.GetElementsByTagName("totalImpuestoReembolso")[0]?.InnerText,
                totalSinImpuestos = item.GetElementsByTagName("totalSinImpuestos")[0]?.InnerText,
                importeTotal = item.GetElementsByTagName("importeTotal")[0]?.InnerText
            };
            GetImpuestoDocSustento(docSustento, item.SelectSingleNode("impuestosDocSustento"));
            GetRetenciones(docSustento, item.SelectSingleNode("retenciones"), ce);
            if (docSustento.codDocSustento == "41") GetReembolsos(docSustento, item.SelectSingleNode("reembolsos"));

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
                             codImpuestoDocSustento = item.GetElementsByTagName("codImpuestoDocSustento")[0]?.InnerText,
                             codigoPorcentaje = item.GetElementsByTagName("codigoPorcentaje")[0]?.InnerText,
                             baseImponible = item.GetElementsByTagName("baseImponible")[0]?.InnerText,
                             tarifa = item.GetElementsByTagName("tarifa")[0]?.InnerText,
                             valorImpuesto = item.GetElementsByTagName("valorImpuesto")[0]?.InnerText
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
                codigo = item.GetElementsByTagName("codigo")[0]?.InnerText,
                codigoRetencion = item.GetElementsByTagName("codigoRetencion")[0]?.InnerText,
                baseImponible = item.GetElementsByTagName("baseImponible")[0]?.InnerText,
                porcentajeRetener = item.GetElementsByTagName("porcentajeRetener")[0]?.InnerText,
                valorRetenido = item.GetElementsByTagName("valorRetenido")[0]?.InnerText
            };
            retenciones.Add(retencion);

            var ir = new ImpuestoRetencion
            {
                codigo = item.GetElementsByTagName("codigo")[0]?.InnerText,
                codigoRetencion = item.GetElementsByTagName("codigoRetencion")[0]?.InnerText,
                baseImponible = item.GetElementsByTagName("baseImponible")[0]?.InnerText,
                porcentajeRetener = item.GetElementsByTagName("porcentajeRetener")[0]?.InnerText,
                valorRetenido = item.GetElementsByTagName("valorRetenido")[0]?.InnerText,
                codDocSustento = doc.codDocSustento,
                numDocSustento = doc.numDocSustento,
                fechaEmisionDocSustento = doc.fechaEmisionDocSustento
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
                tipoIdentificacionProveedorReembolso = item.GetElementsByTagName("tipoIdentificacionProveedorReembolso")[0]?.InnerText,
                identificacionProveedorReembolso = item.GetElementsByTagName("identificacionProveedorReembolso")[0]?.InnerText,
                codPaisPagoProveedorReembolso = item.GetElementsByTagName("codPaisPagoProveedorReembolso")[0]?.InnerText,
                tipoProveedorReembolso = item.GetElementsByTagName("tipoProveedorReembolso")[0]?.InnerText,
                codDocReembolso = item.GetElementsByTagName("codDocReembolso")[0]?.InnerText,
                estabDocReembolso = item.GetElementsByTagName("estabDocReembolso")[0]?.InnerText,
                ptoEmiDocReembolso = item.GetElementsByTagName("ptoEmiDocReembolso")[0]?.InnerText,
                secuencialDocReembolso = item.GetElementsByTagName("secuencialDocReembolso")[0]?.InnerText,
                fechaEmisionDocReembolso = item.GetElementsByTagName("fechaEmisionDocReembolso")[0]?.InnerText,
                numeroAutorizacionDocReemb = item.GetElementsByTagName("numeroAutorizacionDocReemb")[0]?.InnerText
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
                             codigo = item.GetElementsByTagName("codigo")[0]?.InnerText,
                             codigoPorcentaje = item.GetElementsByTagName("codigoPorcentaje")[0]?.InnerText,
                             tarifa = item.GetElementsByTagName("tarifa")[0]?.InnerText,
                             baseImponibleReembolso = item.GetElementsByTagName("baseImponibleReembolso")[0]?.InnerText,
                             impuestoReembolso = item.GetElementsByTagName("impuestoReembolso")[0]?.InnerText
                         }).ToList();

        reembolsoDetalles.CreateTax(impuestos);
    }

    private void GetRetencionPayments(DocSustento doc, XmlNode payments)
    {
        var pagos = (from XmlNode item in payments select new Pago { formaPago = item.SelectSingleNode("formaPago")?.InnerText, total = item.SelectSingleNode("total")?.InnerText }).ToList();

        doc.CreatePayments(pagos);
    }
}