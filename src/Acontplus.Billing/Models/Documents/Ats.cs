namespace Acontplus.Billing.Models.Documents;

public record AtsHeader
{
    public string TipoIdInformante { get; init; } = string.Empty;
    public string IdInformante { get; init; } = string.Empty;
    public string RazonSocial { get; init; } = string.Empty;
    public string Anio { get; init; } = string.Empty;
    public string Mes { get; init; } = string.Empty;
    public string NumEstabRuc { get; init; } = string.Empty;
    public string TotalVentas { get; init; } = string.Empty;
    public string CodigoOperativo { get; init; } = string.Empty;
}

public record Purchase
{
    public string CodSustento { get; init; } = string.Empty;
    public string TpIdProv { get; init; } = string.Empty;
    public string IdProv { get; init; } = string.Empty;
    public string TipoComprobante { get; init; } = string.Empty;
    public string ParteRel { get; init; } = string.Empty;
    public string FechaRegistro { get; init; } = string.Empty;
    public string Establecimiento { get; init; } = string.Empty;
    public string PuntoEmision { get; init; } = string.Empty;
    public string Secuencial { get; init; } = string.Empty;
    public string FechaEmision { get; init; } = string.Empty;
    public string Autorizacion { get; init; } = string.Empty;
    public string BaseNoGraIva { get; init; } = string.Empty;
    public string BaseImponible { get; init; } = string.Empty;
    public string BaseImpGrav { get; init; } = string.Empty;
    public string BaseImpExe { get; init; } = string.Empty;
    public string MontoIce { get; init; } = string.Empty;
    public string MontoIva { get; init; } = string.Empty;
    public string ValRetBien10 { get; init; } = string.Empty;
    public string ValRetServ20 { get; init; } = string.Empty;
    public string ValorRetBienes { get; init; } = string.Empty;
    public string ValRetServ50 { get; init; } = string.Empty;
    public string ValorRetServicios { get; init; } = string.Empty;
    public string ValRetServ100 { get; init; } = string.Empty;
    public string TotbasesImpReemb { get; init; } = string.Empty;
    public string PagoLocExt { get; init; } = string.Empty;
    public string TipoRegi { get; init; } = string.Empty;
    public string DenopagoRegFis { get; init; } = string.Empty;
    public string PaisEfecPago { get; init; } = string.Empty;
    public string AplicConvDobTrib { get; init; } = string.Empty;
    public string PagExtSujRetNorLeg { get; init; } = string.Empty;
    public string FormaPago { get; init; } = string.Empty;
    public string DocModificado { get; init; }
    public string EstabModificado { get; init; }
    public string PtoEmiModificado { get; init; }
    public string SecModificado { get; init; }
    public string AutModificado { get; init; }
    public string EstabRetencion1 { get; init; }
    public string PtoEmiRetencion1 { get; init; }
    public string SecRetencion1 { get; init; }
    public string AutRetencion1 { get; init; }
    public string FechaEmiRet1 { get; init; }
    public string NroDocumento { get; init; } = string.Empty; // Used for linking with withholding taxes
}

public record WithholdingTax
{
    public string CodRetAir { get; init; } = string.Empty;
    public string BaseImpAir { get; init; } = string.Empty;
    public string PorcentajeAir { get; init; } = string.Empty;
    public string ValRetAir { get; init; } = string.Empty;
    public string NroDocumento { get; init; } = string.Empty; // Used for linking
    public string ClaveAcceso { get; init; } = string.Empty; // Used for linking (Autorizacion)
}

public record Sale
{
    public string TpIdCliente { get; init; } = string.Empty;
    public string IdCliente { get; init; } = string.Empty;
    public string ParteRelVtas { get; init; }
    public string TipoCliente { get; init; }
    public string DenoCli { get; init; }
    public string TipoComprobante { get; init; } = string.Empty;
    public string TipoEmision { get; init; } = string.Empty;
    public string NumeroComprobantes { get; init; } = string.Empty;
    public string BaseNoGraIva { get; init; } = string.Empty;
    public string BaseImponible { get; init; } = string.Empty;
    public string BaseImpGrav { get; init; } = string.Empty;
    public string MontoIva { get; init; } = string.Empty;
    public string TipoCompe { get; init; }
    public string MontoCompensacion { get; init; }
    public string MontoIce { get; init; } = string.Empty;
    public string ValorRetIva { get; init; } = string.Empty;
    public string ValorRetRenta { get; init; } = string.Empty;
    public string FormaPago { get; init; } = string.Empty;
}

public record EstablishmentSale
{
    public string CodEstab { get; init; } = string.Empty;
    public string VentasEstab { get; init; } = string.Empty;
    public string IvaComp { get; init; } = string.Empty;
}

public record CanceledDocument
{
    public string TipoComprobante { get; init; } = string.Empty;
    public string Establecimiento { get; init; } = string.Empty;
    public string PuntoEmision { get; init; } = string.Empty;
    public string SecuencialInicio { get; init; } = string.Empty;
    public string SecuencialFin { get; init; } = string.Empty;
    public string Autorizacion { get; init; } = string.Empty;
}

public record AtsData
{
    public AtsHeader Header { get; init; } = new();
    public IEnumerable<Purchase> Purchases { get; init; } = Enumerable.Empty<Purchase>();
    public IEnumerable<Sale> Sales { get; init; } = Enumerable.Empty<Sale>();
    public IEnumerable<WithholdingTax> WithholdingTaxes { get; init; } = Enumerable.Empty<WithholdingTax>();
    public IEnumerable<EstablishmentSale> EstablishmentSales { get; init; } = Enumerable.Empty<EstablishmentSale>();
    public IEnumerable<CanceledDocument> CanceledDocuments { get; init; } = Enumerable.Empty<CanceledDocument>();
}