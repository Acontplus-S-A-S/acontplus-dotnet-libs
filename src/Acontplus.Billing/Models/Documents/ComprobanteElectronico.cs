namespace Acontplus.Billing.Models.Documents;

public class ComprobanteElectronico
{
    public string VersionComp { get; set; } = string.Empty;
    public string CodDoc { get; set; } = string.Empty;
    public string FechaAutorizacion { get; set; } = string.Empty;
    public string NumeroAutorizacion { get; set; } = string.Empty;
    public InfoTributaria InfoTributaria { get; set; } = new();

    public InfoFactura? InfoFactura { get; set; }
    public List<Detalle>? Detalles { get; set; }
    public List<Impuesto>? Impuestos { get; set; }

    public InfoCompRetencion? InfoCompRetencion { get; set; }
    public List<ImpuestoRetencion>? ImpuestosRetencion { get; set; }
    public List<DocSustento>? DocSustentos { get; set; }

    public InfoNotaCredito? InfoNotaCredito { get; set; }

    public List<InfoAdicional>? InfoAdicional { get; set; }

    public void CreateInfoComp(string codDoc, object? obj)
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

    public void CreateDetails(object? obj)
    {
        Detalles = obj as List<Detalle>;
    }

    public void CreateTaxes(object? obj)
    {
        Impuestos = obj as List<Impuesto>;
    }

    public void CreateRetencionTaxes(object? obj)
    {
        ImpuestosRetencion = obj as List<ImpuestoRetencion>;
    }

    public void CreateDocSustentos(object? obj)
    {
        DocSustentos = obj as List<DocSustento>;
    }

    public void CreateAdditionalInfo(object? obj)
    {
        InfoAdicional = obj as List<InfoAdicional>;
    }
}

public class InfoTributaria
{
    public string Ambiente { get; set; } = string.Empty;
    public string TipoEmision { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string NombreComercial { get; set; } = string.Empty;
    public string Ruc { get; set; } = string.Empty;
    public string ClaveAcceso { get; set; } = string.Empty;
    public string CodDoc { get; set; } = string.Empty;
    public string Estab { get; set; } = string.Empty;
    public string PtoEmi { get; set; } = string.Empty;
    public string Secuencial { get; set; } = string.Empty;
    public string DirMatriz { get; set; } = string.Empty;
}

public class InfoFactura
{
    public string FechaEmision { get; set; } = string.Empty;
    public string DirEstablecimiento { get; set; } = string.Empty;
    public string ContribuyenteEspecial { get; set; } = string.Empty;
    public string ObligadoContabilidad { get; set; } = string.Empty;
    public string TipoIdentificacionComprador { get; set; } = string.Empty;
    public string RazonSocialComprador { get; set; } = string.Empty;
    public string IdentificacionComprador { get; set; } = string.Empty;
    public string DireccionComprador { get; set; } = string.Empty;
    public string GuiaRemision { get; set; } = string.Empty;
    public string TotalSinImpuestos { get; set; } = string.Empty;
    public string TotalDescuento { get; set; } = string.Empty;
    public string Propina { get; set; } = string.Empty;
    public string ImporteTotal { get; set; } = string.Empty;
    public string Moneda { get; set; } = string.Empty;
    public List<TotalImpuesto>? TotalImpuestos { get; set; }
    public List<Pago>? Pagos { get; set; }

    public void CreateTotalTaxes(object? obj)
    {
        TotalImpuestos = obj as List<TotalImpuesto>;
    }

    public void CreatePayments(object? obj)
    {
        Pagos = obj as List<Pago>;
    }
}

public class TotalImpuesto
{
    public string Codigo { get; set; } = string.Empty;
    public string CodigoPorcentaje { get; set; } = string.Empty;
    public string DescuentoAdicional { get; set; } = string.Empty;
    public string BaseImponible { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
}

public class Pago
{
    public string FormaPago { get; set; } = string.Empty;
    public string Total { get; set; } = string.Empty;
    public string Plazo { get; set; } = string.Empty;
    public string UnidadTiempo { get; set; } = string.Empty;
}

public class Detalle
{
    public int IdDetalle { get; set; }
    public string CodigoPrincipal { get; set; } = string.Empty;
    public string CodigoAuxiliar { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Cantidad { get; set; } = string.Empty;
    public string PrecioUnitario { get; set; } = string.Empty;
    public string Descuento { get; set; } = string.Empty;
    public string PrecioTotalSinImpuesto { get; set; } = string.Empty;
    public string DetAdicionalNombre { get; set; } = string.Empty;
    public string DetAdicionalValor { get; set; } = string.Empty;
    public string DetallesAdicionales { get; set; } = string.Empty;
    public string Impuestos { get; set; } = string.Empty;
}

public class DetalleFactura
{
    public int IdProveedor { get; set; }
    public int IdArticulo { get; set; }
    public int IdMedida { get; set; }
    public string CodigoPrincipal { get; set; } = string.Empty;
    public string CodigoAuxiliar { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Cantidad { get; set; } = string.Empty;
    public string PrecioUnitario { get; set; } = string.Empty;
    public string Descuento { get; set; } = string.Empty;
    public string PrecioTotalSinImpuesto { get; set; } = string.Empty;
    public string DetAdicionalNombre { get; set; } = string.Empty;
    public string DetAdicionalValor { get; set; } = string.Empty;
}

public class Impuesto
{
    public int IdDetalle { get; set; }
    public string CodArticulo { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public string CodigoPorcentaje { get; set; } = string.Empty;
    public string Tarifa { get; set; } = string.Empty;
    public string BaseImponible { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
}

public class InfoNotaCredito
{
    public string FechaEmision { get; set; } = string.Empty;
    public string DirEstablecimiento { get; set; } = string.Empty;
    public string TipoIdentificacionComprador { get; set; } = string.Empty;
    public string RazonSocialComprador { get; set; } = string.Empty;
    public string IdentificacionComprador { get; set; } = string.Empty;
    public string ContribuyenteEspecial { get; set; } = string.Empty;
    public string ObligadoContabilidad { get; set; } = string.Empty;
    public string Rise { get; set; } = string.Empty;
    public string CodDocModificado { get; set; } = string.Empty;
    public string NumDocModificado { get; set; } = string.Empty;
    public string FechaEmisionDocSustento { get; set; } = string.Empty;
    public string TotalSinImpuestos { get; set; } = string.Empty;
    public string ValorModificacion { get; set; } = string.Empty;
    public string Moneda { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
    public List<TotalImpuesto>? TotalImpuestos { get; set; }

    public void CreateTotalTaxes(object? obj)
    {
        TotalImpuestos = obj as List<TotalImpuesto>;
    }
}

public class InfoCompRetencion
{
    public string FechaEmision { get; set; } = string.Empty;
    public string DirEstablecimiento { get; set; } = string.Empty;
    public string ContribuyenteEspecial { get; set; } = string.Empty;
    public string ObligadoContabilidad { get; set; } = string.Empty;
    public string TipoIdentificacionSujetoRetenido { get; set; } = string.Empty;
    public string RazonSocialSujetoRetenido { get; set; } = string.Empty;
    public string IdentificacionSujetoRetenido { get; set; } = string.Empty;
    public string PeriodoFiscal { get; set; } = string.Empty;

    //version ATS
    public string TipoSujetoRetenido { get; set; } = string.Empty;
    public string ParteRel { get; set; } = string.Empty;
}

public class ImpuestoRetencion
{
    public string? Codigo { get; set; }
    public string CodigoRetencion { get; set; } = string.Empty;
    public string BaseImponible { get; set; } = string.Empty;
    public string PorcentajeRetener { get; set; } = string.Empty;
    public string ValorRetenido { get; set; } = string.Empty;
    public string CodDocSustento { get; set; } = string.Empty;
    public string NumDocSustento { get; set; } = string.Empty;
    public string FechaEmisionDocSustento { get; set; } = string.Empty;
}

public class DocSustento
{
    public string CodSustento { get; set; } = string.Empty;
    public string CodDocSustento { get; set; } = string.Empty;
    public string NumDocSustento { get; set; } = string.Empty;
    public string FechaEmisionDocSustento { get; set; } = string.Empty;
    public string FechaRegistroContable { get; set; } = string.Empty;
    public string NumAutDocSustento { get; set; } = string.Empty;
    public string PagoLocExt { get; set; } = string.Empty;
    public string TipoRegi { get; set; } = string.Empty;
    public string PaisEfecPago { get; set; } = string.Empty;
    public string AplicConvDobTrib { get; set; } = string.Empty;
    public string PagExtSujRetNorLeg { get; set; } = string.Empty;
    public string PagoRegFis { get; set; } = string.Empty;
    public string TotalComprobantesReembolso { get; set; } = string.Empty;
    public string TotalBaseImponibleReembolso { get; set; } = string.Empty;
    public string TotalImpuestoReembolso { get; set; } = string.Empty;
    public string TotalSinImpuestos { get; set; } = string.Empty;
    public string ImporteTotal { get; set; } = string.Empty;
    public List<ImpuestoDocSustento>? Impuestos { get; set; }
    public List<Retencion>? Retenciones { get; set; }
    public List<ReembolsoDetalle>? Reembolsos { get; set; }
    public List<Pago>? Pagos { get; set; }

    public void CreateTax(object? obj)
    {
        Impuestos = obj as List<ImpuestoDocSustento>;
    }

    public void CreateRetencion(object? obj)
    {
        Retenciones = obj as List<Retencion>;
    }

    public void CreateReembolsos(object? obj)
    {
        Reembolsos = obj as List<ReembolsoDetalle>;
    }

    public void CreatePayments(object? obj)
    {
        Pagos = obj as List<Pago>;
    }
}

public class ImpuestoDocSustento
{
    public string CodImpuestoDocSustento { get; set; } = string.Empty;
    public string CodigoPorcentaje { get; set; } = string.Empty;
    public string BaseImponible { get; set; } = string.Empty;
    public string Tarifa { get; set; } = string.Empty;
    public string ValorImpuesto { get; set; } = string.Empty;
}

public class Retencion
{
    public string Codigo { get; set; } = string.Empty;
    public string CodigoRetencion { get; set; } = string.Empty;
    public string BaseImponible { get; set; } = string.Empty;
    public string PorcentajeRetener { get; set; } = string.Empty;
    public string ValorRetenido { get; set; } = string.Empty;
    public List<Dividendo>? Dividendos { get; set; }
    public List<CompraCajBanano>? BananasBox { get; set; }

    public void CreateDividendo(object? obj)
    {
        Dividendos = obj as List<Dividendo>;
    }

    public void CreateBananaBox(object? obj)
    {
        BananasBox = obj as List<CompraCajBanano>;
    }
}

public class Dividendo
{
    public string FechaPagoDiv { get; set; } = string.Empty;
    public string ImRentaSoc { get; set; } = string.Empty;
    public string EjerFisUtDiv { get; set; } = string.Empty;
}

public class CompraCajBanano
{
    public string NumCajBan { get; set; } = string.Empty;
    public string PrecCajBan { get; set; } = string.Empty;
}

public class ReembolsoDetalle
{
    public string TipoIdentificacionProveedorReembolso { get; set; } = string.Empty;
    public string IdentificacionProveedorReembolso { get; set; } = string.Empty;
    public string CodPaisPagoProveedorReembolso { get; set; } = string.Empty;
    public string TipoProveedorReembolso { get; set; } = string.Empty;
    public string CodDocReembolso { get; set; } = string.Empty;
    public string EstabDocReembolso { get; set; } = string.Empty;
    public string PtoEmiDocReembolso { get; set; } = string.Empty;
    public string SecuencialDocReembolso { get; set; } = string.Empty;
    public string FechaEmisionDocReembolso { get; set; } = string.Empty;
    public string NumeroAutorizacionDocReemb { get; set; } = string.Empty;
    public List<DetalleImpuesto>? ImpuestosReembolso { get; set; }

    public void CreateTax(object? obj)
    {
        ImpuestosReembolso = obj as List<DetalleImpuesto>;
    }
}

public class DetalleImpuesto
{
    public string Codigo { get; set; } = string.Empty;
    public string CodigoPorcentaje { get; set; } = string.Empty;
    public string Tarifa { get; set; } = string.Empty;
    public string BaseImponibleReembolso { get; set; } = string.Empty;
    public string ImpuestoReembolso { get; set; } = string.Empty;
}

public class InfoAdicional
{
    public string? Nombre { get; set; }
    public string? Valor { get; set; }
}
