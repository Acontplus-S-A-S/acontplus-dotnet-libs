namespace Acontplus.FactElect.Models.Documents;

public class ComprobanteElectronico
{
    public string VersionComp { get; set; }
    public string CodDoc { get; set; }
    public string FechaAutorizacion { get; set; }
    public string NumeroAutorizacion { get; set; }
    public InfoTributaria InfoTributaria { get; set; }

    public InfoFactura InfoFactura { get; set; }
    public List<Detalle> Detalles { get; set; }
    public List<Impuesto> Impuestos { get; set; }

    public InfoCompRetencion InfoCompRetencion { get; set; }
    public List<ImpuestoRetencion> ImpuestosRetencion { get; set; }
    public List<DocSustento> DocSustentos { get; set; }

    public InfoNotaCredito InfoNotaCredito { get; set; }

    public List<InfoAdicional> InfoAdicional { get; set; }

    public void CreateInfoComp(string codDoc, object obj)
    {
        switch (codDoc)
        {
            case "01":
                InfoFactura = obj as InfoFactura;
                break;
            case "03":
                break;
            case "04":
                InfoNotaCredito = obj as InfoNotaCredito;
                break;
            case "05":
                break;
            case "06":
                break;
            case "07":
                InfoCompRetencion = obj as InfoCompRetencion;
                break;
        }
    }

    public void CreateDetails(object obj)
    {
        Detalles = obj as List<Detalle>;
    }

    public void CreateTaxes(object obj)
    {
        Impuestos = obj as List<Impuesto>;
    }

    public void CreateRetencionTaxes(object obj)
    {
        ImpuestosRetencion = obj as List<ImpuestoRetencion>;
    }

    public void CreateDocSustentos(object obj)
    {
        DocSustentos = obj as List<DocSustento>;
    }

    public void CreateAdditionalInfo(object obj)
    {
        InfoAdicional = obj as List<InfoAdicional>;
    }
}

public class InfoTributaria
{
    public string Ambiente { get; set; }
    public string TipoEmision { get; set; }
    public string RazonSocial { get; set; }
    public string NombreComercial { get; set; }
    public string Ruc { get; set; }
    public string ClaveAcceso { get; set; }
    public string CodDoc { get; set; }
    public string Estab { get; set; }
    public string PtoEmi { get; set; }
    public string Secuencial { get; set; }
    public string DirMatriz { get; set; }
}

public class InfoFactura
{
    public string fechaEmision { get; set; }
    public string dirEstablecimiento { get; set; }
    public string contribuyenteEspecial { get; set; }
    public string obligadoContabilidad { get; set; }
    public string tipoIdentificacionComprador { get; set; }
    public string razonSocialComprador { get; set; }
    public string identificacionComprador { get; set; }
    public string direccionComprador { get; set; }
    public string guiaRemision { get; set; }
    public string totalSinImpuestos { get; set; }
    public string totalDescuento { get; set; }
    public string propina { get; set; }
    public string importeTotal { get; set; }
    public string moneda { get; set; }
    public List<TotalImpuesto> totalImpuestos { get; set; }
    public List<Pago> pagos { get; set; }

    public void CreateTotalTaxes(object obj)
    {
        totalImpuestos = obj as List<TotalImpuesto>;
    }

    public void CreatePayments(object obj)
    {
        pagos = obj as List<Pago>;
    }
}

public class TotalImpuesto
{
    public string codigo { get; set; }
    public string codigoPorcentaje { get; set; }
    public string descuentoAdicional { get; set; }
    public string baseImponible { get; set; }
    public string valor { get; set; }
}

public class Pago
{
    public string formaPago { get; set; }
    public string total { get; set; }
    public string plazo { get; set; }
    public string unidadTiempo { get; set; }
}

public class Detalle
{
    public int idDetalle { get; set; }
    public string codigoPrincipal { get; set; }
    public string codigoAuxiliar { get; set; }
    public string descripcion { get; set; }
    public string cantidad { get; set; }
    public string precioUnitario { get; set; }
    public string descuento { get; set; }
    public string precioTotalSinImpuesto { get; set; }
    public string detAdicionalNombre { get; set; }
    public string detAdicionalValor { get; set; }
    public string detallesAdicionales { get; set; }
    public string impuestos { get; set; }
}

public class DetalleFactura
{
    public int idProveedor { get; set; }
    public int idArticulo { get; set; }
    public int idMedida { get; set; }
    public string codigoPrincipal { get; set; }
    public string codigoAuxiliar { get; set; }
    public string descripcion { get; set; }
    public string cantidad { get; set; }
    public string precioUnitario { get; set; }
    public string descuento { get; set; }
    public string precioTotalSinImpuesto { get; set; }
    public string detAdicionalNombre { get; set; }
    public string detAdicionalValor { get; set; }
}

public class Impuesto
{
    public int idDetalle { get; set; }
    public string codArticulo { get; set; }
    public string codigo { get; set; }
    public string codigoPorcentaje { get; set; }
    public string tarifa { get; set; }
    public string baseImponible { get; set; }
    public string valor { get; set; }
}

public class InfoNotaCredito
{
    public string fechaEmision { get; set; }
    public string dirEstablecimiento { get; set; }
    public string tipoIdentificacionComprador { get; set; }
    public string razonSocialComprador { get; set; }
    public string identificacionComprador { get; set; }
    public string contribuyenteEspecial { get; set; }
    public string obligadoContabilidad { get; set; }
    public string rise { get; set; }
    public string codDocModificado { get; set; }
    public string numDocModificado { get; set; }
    public string fechaEmisionDocSustento { get; set; }
    public string totalSinImpuestos { get; set; }
    public string valorModificacion { get; set; }
    public string moneda { get; set; }
    public string motivo { get; set; }
    public List<TotalImpuesto> totalImpuestos { get; set; }

    public void CreateTotalTaxes(object obj)
    {
        totalImpuestos = obj as List<TotalImpuesto>;
    }
}

public class InfoCompRetencion
{
    public string fechaEmision { get; set; }
    public string dirEstablecimiento { get; set; }
    public string contribuyenteEspecial { get; set; }
    public string obligadoContabilidad { get; set; }
    public string tipoIdentificacionSujetoRetenido { get; set; }
    public string razonSocialSujetoRetenido { get; set; }
    public string identificacionSujetoRetenido { get; set; }

    public string periodoFiscal { get; set; }

    //version ATS
    public string tipoSujetoRetenido { get; set; }
    public string parteRel { get; set; }
}

public class ImpuestoRetencion
{
    public string codigo { get; set; }
    public string codigoRetencion { get; set; }
    public string baseImponible { get; set; }
    public string porcentajeRetener { get; set; }
    public string valorRetenido { get; set; }
    public string codDocSustento { get; set; }
    public string numDocSustento { get; set; }
    public string fechaEmisionDocSustento { get; set; }
}

public class DocSustento
{
    public string codSustento { get; set; }
    public string codDocSustento { get; set; }
    public string numDocSustento { get; set; }
    public string fechaEmisionDocSustento { get; set; }
    public string fechaRegistroContable { get; set; }
    public string numAutDocSustento { get; set; }
    public string pagoLocExt { get; set; }
    public string tipoRegi { get; set; }
    public string paisEfecPago { get; set; }
    public string aplicConvDobTrib { get; set; }
    public string pagExtSujRetNorLeg { get; set; }
    public string pagoRegFis { get; set; }
    public string totalComprobantesReembolso { get; set; }
    public string totalBaseImponibleReembolso { get; set; }
    public string totalImpuestoReembolso { get; set; }
    public string totalSinImpuestos { get; set; }
    public string importeTotal { get; set; }
    public List<ImpuestoDocSustento> impuestos { get; set; }
    public List<Retencion> retenciones { get; set; }
    public List<ReembolsoDetalle> reembolsos { get; set; }
    public List<Pago> pagos { get; set; }

    public void CreateTax(object obj)
    {
        impuestos = obj as List<ImpuestoDocSustento>;
    }

    public void CreateRetencion(object obj)
    {
        retenciones = obj as List<Retencion>;
    }

    public void CreateReembolsos(object obj)
    {
        reembolsos = obj as List<ReembolsoDetalle>;
    }

    public void CreatePayments(object obj)
    {
        pagos = obj as List<Pago>;
    }
}

public class ImpuestoDocSustento
{
    public string codImpuestoDocSustento { get; set; }
    public string codigoPorcentaje { get; set; }
    public string baseImponible { get; set; }
    public string tarifa { get; set; }
    public string valorImpuesto { get; set; }
}

public class Retencion
{
    public string codigo { get; set; }
    public string codigoRetencion { get; set; }
    public string baseImponible { get; set; }
    public string porcentajeRetener { get; set; }
    public string valorRetenido { get; set; }
    public List<Dividendo> dividendos { get; set; }
    public List<CompraCajBanano> bananasBox { get; set; }

    public void CreateDividendo(object obj)
    {
        dividendos = obj as List<Dividendo>;
    }

    public void CreateBananaBox(object obj)
    {
        bananasBox = obj as List<CompraCajBanano>;
    }
}

public class Dividendo
{
    public string fechaPagoDiv { get; set; }
    public string imRentaSoc { get; set; }
    public string ejerFisUtDiv { get; set; }
}

public class CompraCajBanano
{
    public string numCajBan { get; set; }
    public string precCajBan { get; set; }
    public string NumCajBan { get; set; }
}

public class ReembolsoDetalle
{
    public string tipoIdentificacionProveedorReembolso { get; set; }
    public string identificacionProveedorReembolso { get; set; }
    public string codPaisPagoProveedorReembolso { get; set; }
    public string tipoProveedorReembolso { get; set; }
    public string codDocReembolso { get; set; }
    public string estabDocReembolso { get; set; }
    public string ptoEmiDocReembolso { get; set; }
    public string secuencialDocReembolso { get; set; }
    public string fechaEmisionDocReembolso { get; set; }
    public string numeroAutorizacionDocReemb { get; set; }
    public List<DetalleImpuesto> impuestosReembolso { get; set; }

    public void CreateTax(object obj)
    {
        impuestosReembolso = obj as List<DetalleImpuesto>;
    }
}

public class DetalleImpuesto
{
    public string codigo { get; set; }
    public string codigoPorcentaje { get; set; }
    public string tarifa { get; set; }
    public string baseImponibleReembolso { get; set; }
    public string impuestoReembolso { get; set; }
}

public class InfoAdicional
{
    public string? Nombre { get; set; }
    public string? Valor { get; set; }
}