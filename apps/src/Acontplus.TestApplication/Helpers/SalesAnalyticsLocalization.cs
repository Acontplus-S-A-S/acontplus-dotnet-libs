namespace Acontplus.TestApplication.Helpers;

/// <summary>
/// Localization provider for sales analytics labels
/// Application-level concern, not library-level
/// </summary>
public static class SalesAnalyticsLocalization
{
    public static Dictionary<string, string> GetLabels(string language) => language.ToLowerInvariant() switch
    {
        "es" => GetSpanishLabels(),
        "en" => GetEnglishLabels(),
        _ => GetEnglishLabels()
    };

    private static Dictionary<string, string> GetSpanishLabels() => new()
    {
        // Base dashboard properties
        { "TotalTransactions", "Total de Transacciones" },
        { "CompletedTransactions", "Transacciones Completadas" },
        { "TotalRevenue", "Ingresos Totales" },
        { "NetRevenue", "Ingresos Netos" },
        { "GrowthRate", "Tasa de Crecimiento" },
        { "CompletionRate", "Tasa de Finalización" },
        { "UniqueEntities", "Clientes Únicos" },
        { "AverageTransactionValue", "Valor Promedio de Transacción" },

        // Sales-specific properties
        { "AverageOrderValue", "Valor Promedio de Orden" },
        { "TotalDiscounts", "Descuentos Totales" },
        { "DiscountPercentage", "Porcentaje de Descuento" },
        { "CancelledOrders", "Órdenes Canceladas" },
        { "CancellationRate", "Tasa de Cancelación" },
        { "CashSales", "Ventas en Efectivo" },
        { "CreditCardSales", "Ventas con Tarjeta" },
        { "TransferSales", "Ventas por Transferencia" },
        { "NewCustomers", "Clientes Nuevos" },
        { "ReturningCustomers", "Clientes Recurrentes" },
        { "CustomerRetentionRate", "Tasa de Retención de Clientes" },

        // Real-time properties
        { "ActiveSales", "Ventas Activas" },
        { "PendingPayments", "Pagos Pendientes" },
        { "CurrentHourRevenue", "Ingresos Hora Actual" },
        { "CurrentDayRevenue", "Ingresos Día Actual" },
        { "AverageSaleTime", "Tiempo Promedio de Venta" },
        { "SalesPerHour", "Ventas por Hora" },
        { "PeakHourRevenue", "Ingresos Hora Pico" },

        // Chart labels
        { "Chart.Title.SalesDashboard", "Panel de Ventas" },
        { "Chart.Axis.Revenue", "Ingresos ($)" },
        { "Chart.Axis.Date", "Fecha" },
        { "Chart.Axis.Quantity", "Cantidad" },
        { "Chart.Legend.Current", "Actual" },
        { "Chart.Legend.Previous", "Anterior" }
    };

    private static Dictionary<string, string> GetEnglishLabels() => new()
    {
        // Base dashboard properties
        { "TotalTransactions", "Total Transactions" },
        { "CompletedTransactions", "Completed Transactions" },
        { "TotalRevenue", "Total Revenue" },
        { "NetRevenue", "Net Revenue" },
        { "GrowthRate", "Growth Rate" },
        { "CompletionRate", "Completion Rate" },
        { "UniqueEntities", "Unique Customers" },
        { "AverageTransactionValue", "Average Transaction Value" },

        // Sales-specific properties
        { "AverageOrderValue", "Average Order Value" },
        { "TotalDiscounts", "Total Discounts" },
        { "DiscountPercentage", "Discount Percentage" },
        { "CancelledOrders", "Cancelled Orders" },
        { "CancellationRate", "Cancellation Rate" },
        { "CashSales", "Cash Sales" },
        { "CreditCardSales", "Credit Card Sales" },
        { "TransferSales", "Transfer Sales" },
        { "NewCustomers", "New Customers" },
        { "ReturningCustomers", "Returning Customers" },
        { "CustomerRetentionRate", "Customer Retention Rate" },

        // Real-time properties
        { "ActiveSales", "Active Sales" },
        { "PendingPayments", "Pending Payments" },
        { "CurrentHourRevenue", "Current Hour Revenue" },
        { "CurrentDayRevenue", "Current Day Revenue" },
        { "AverageSaleTime", "Average Sale Time" },
        { "SalesPerHour", "Sales Per Hour" },
        { "PeakHourRevenue", "Peak Hour Revenue" },

        // Chart labels
        { "Chart.Title.SalesDashboard", "Sales Dashboard" },
        { "Chart.Axis.Revenue", "Revenue ($)" },
        { "Chart.Axis.Date", "Date" },
        { "Chart.Axis.Quantity", "Quantity" },
        { "Chart.Legend.Current", "Current" },
        { "Chart.Legend.Previous", "Previous" }
    };
}
