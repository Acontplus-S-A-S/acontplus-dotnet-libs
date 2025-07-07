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
    public string FechaEmision { get; set; }
    public string DirEstablecimiento { get; set; }
    public string ContribuyenteEspecial { get; set; }
    public string ObligadoContabilidad { get; set; }
    public string TipoIdentificacionComprador { get; set; }
    public string RazonSocialComprador { get; set; }
    public string IdentificacionComprador { get; set; }
    public string DireccionComprador { get; set; }
    public string GuiaRemision { get; set; }
    public string TotalSinImpuestos { get; set; }
    public string TotalDescuento { get; set; }
    public string Propina { get; set; }
    public string ImporteTotal { get; set; }
    public string Moneda { get; set; }
    public List<TotalImpuesto> TotalImpuestos { get; set; }
    public List<Pago> Pagos { get; set; }

    public void CreateTotalTaxes(object obj)
    {
        TotalImpuestos = obj as List<TotalImpuesto>;
    }

    public void CreatePayments(object obj)
    {
        Pagos = obj as List<Pago>;
    }
}

public class TotalImpuesto
{
    public string Codigo { get; set; }
    public string CodigoPorcentaje { get; set; }
    public string DescuentoAdicional { get; set; }
    public string BaseImponible { get; set; }
    public string Valor { get; set; }
}

public class Pago
{
    public string FormaPago { get; set; }
    public string Total { get; set; }
    public string Plazo { get; set; }
    public string UnidadTiempo { get; set; }
}

public class Detalle
{
    public int IdDetalle { get; set; }
    public string CodigoPrincipal { get; set; }
    public string CodigoAuxiliar { get; set; }
    public string Descripcion { get; set; }
    public string Cantidad { get; set; }
    public string PrecioUnitario { get; set; }
    public string Descuento { get; set; }
    public string PrecioTotalSinImpuesto { get; set; }
    public string DetAdicionalNombre { get; set; }
    public string DetAdicionalValor { get; set; }
    public string DetallesAdicionales { get; set; }
    public string Impuestos { get; set; }
}

public class DetalleFactura
{
    public int IdProveedor { get; set; }
    public int IdArticulo { get; set; }
    public int IdMedida { get; set; }
    public string CodigoPrincipal { get; set; }
    public string CodigoAuxiliar { get; set; }
    public string Descripcion { get; set; }
    public string Cantidad { get; set; }
    public string PrecioUnitario { get; set; }
    public string Descuento { get; set; }
    public string PrecioTotalSinImpuesto { get; set; }
    public string DetAdicionalNombre { get; set; }
    public string DetAdicionalValor { get; set; }
}

public class Impuesto
{
    public int IdDetalle { get; set; }
    public string CodArticulo { get; set; }
    public string Codigo { get; set; }
    public string CodigoPorcentaje { get; set; }
    public string Tarifa { get; set; }
    public string BaseImponible { get; set; }
    public string Valor { get; set; }
}

public class InfoNotaCredito
{
    public string FechaEmision { get; set; }
    public string DirEstablecimiento { get; set; }
    public string TipoIdentificacionComprador { get; set; }
    public string RazonSocialComprador { get; set; }
    public string IdentificacionComprador { get; set; }
    public string ContribuyenteEspecial { get; set; }
    public string ObligadoContabilidad { get; set; }
    public string Rise { get; set; }
    public string CodDocModificado { get; set; }
    public string NumDocModificado { get; set; }
    public string FechaEmisionDocSustento { get; set; }
    public string TotalSinImpuestos { get; set; }
    public string ValorModificacion { get; set; }
    public string Moneda { get; set; }
    public string Motivo { get; set; }
    public List<TotalImpuesto> TotalImpuestos { get; set; }

    public void CreateTotalTaxes(object obj)
    {
        TotalImpuestos = obj as List<TotalImpuesto>;
    }
}

public class InfoCompRetencion
{
    public string FechaEmision { get; set; }
    public string DirEstablecimiento { get; set; }
    public string ContribuyenteEspecial { get; set; }
    public string ObligadoContabilidad { get; set; }
    public string TipoIdentificacionSujetoRetenido { get; set; }
    public string RazonSocialSujetoRetenido { get; set; }
    public string IdentificacionSujetoRetenido { get; set; }

    public string PeriodoFiscal { get; set; }

    //version ATS
    public string TipoSujetoRetenido { get; set; }
    public string ParteRel { get; set; }
}

public class ImpuestoRetencion
{
    public string? Codigo { get; set; }
    public string CodigoRetencion { get; set; }
    public string BaseImponible { get; set; }
    public string PorcentajeRetener { get; set; }
    public string ValorRetenido { get; set; }
    public string CodDocSustento { get; set; }
    public string NumDocSustento { get; set; }
    public string FechaEmisionDocSustento { get; set; }
}

public class DocSustento
{
    public string CodSustento { get; set; }
    public string CodDocSustento { get; set; }
    public string NumDocSustento { get; set; }
    public string FechaEmisionDocSustento { get; set; }
    public string FechaRegistroContable { get; set; }
    public string NumAutDocSustento { get; set; }
    public string PagoLocExt { get; set; }
    public string TipoRegi { get; set; }
    public string PaisEfecPago { get; set; }
    public string AplicConvDobTrib { get; set; }
    public string PagExtSujRetNorLeg { get; set; }
    public string PagoRegFis { get; set; }
    public string TotalComprobantesReembolso { get; set; }
    public string TotalBaseImponibleReembolso { get; set; }
    public string TotalImpuestoReembolso { get; set; }
    public string TotalSinImpuestos { get; set; }
    public string ImporteTotal { get; set; }
    public List<ImpuestoDocSustento> Impuestos { get; set; }
    public List<Retencion> Retenciones { get; set; }
    public List<ReembolsoDetalle> Reembolsos { get; set; }
    public List<Pago> Pagos { get; set; }

    public void CreateTax(object obj)
    {
        Impuestos = obj as List<ImpuestoDocSustento>;
    }

    public void CreateRetencion(object obj)
    {
        Retenciones = obj as List<Retencion>;
    }

    public void CreateReembolsos(object obj)
    {
        Reembolsos = obj as List<ReembolsoDetalle>;
    }

    public void CreatePayments(object obj)
    {
        Pagos = obj as List<Pago>;
    }
}

public class ImpuestoDocSustento
{
    public string CodImpuestoDocSustento { get; set; }
    public string CodigoPorcentaje { get; set; }
    public string BaseImponible { get; set; }
    public string Tarifa { get; set; }
    public string ValorImpuesto { get; set; }
}

public class Retencion
{
    public string Codigo { get; set; }
    public string CodigoRetencion { get; set; }
    public string BaseImponible { get; set; }
    public string PorcentajeRetener { get; set; }
    public string ValorRetenido { get; set; }
    public List<Dividendo> Dividendos { get; set; }
    public List<CompraCajBanano> BananasBox { get; set; }

    public void CreateDividendo(object obj)
    {
        Dividendos = obj as List<Dividendo>;
    }

    public void CreateBananaBox(object obj)
    {
        BananasBox = obj as List<CompraCajBanano>;
    }
}

public class Dividendo
{
    public string FechaPagoDiv { get; set; }
    public string ImRentaSoc { get; set; }
    public string EjerFisUtDiv { get; set; }
}

public class CompraCajBanano
{
    public string NumCajBan { get; set; }
    public string PrecCajBan { get; set; }
}

public class ReembolsoDetalle
{
    public string TipoIdentificacionProveedorReembolso { get; set; }
    public string IdentificacionProveedorReembolso { get; set; }
    public string CodPaisPagoProveedorReembolso { get; set; }
    public string TipoProveedorReembolso { get; set; }
    public string CodDocReembolso { get; set; }
    public string EstabDocReembolso { get; set; }
    public string PtoEmiDocReembolso { get; set; }
    public string SecuencialDocReembolso { get; set; }
    public string FechaEmisionDocReembolso { get; set; }
    public string NumeroAutorizacionDocReemb { get; set; }
    public List<DetalleImpuesto> ImpuestosReembolso { get; set; }

    public void CreateTax(object obj)
    {
        ImpuestosReembolso = obj as List<DetalleImpuesto>;
    }
}

public class DetalleImpuesto
{
    public string Codigo { get; set; }
    public string CodigoPorcentaje { get; set; }
    public string Tarifa { get; set; }
    public string BaseImponibleReembolso { get; set; }
    public string ImpuestoReembolso { get; set; }
}

public class InfoAdicional
{
    public string? Nombre { get; set; }
    public string? Valor { get; set; }
}