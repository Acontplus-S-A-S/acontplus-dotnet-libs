namespace Acontplus.Core.Domain.Enums;

public enum SriDocument
{
    [Description("Factura")] Factura = 1,
    [Description("Liquidación de Compra")] Liquidacion = 3,
    [Description("Nota de Crédito")] NotaCredito = 4,
    [Description("Nota de Débito")] NotaDebito = 5,
    [Description("Guía de Remisión")] GuiaRemision = 6,
    [Description("Retención")] Retencion = 7
}
