namespace Acontplus.Reports.Interfaces;

public interface IRdlcPrinterService
{
    public bool Print(RdlcPrinter rdlcPrinter, RdlcPrintRequest printRequest);
}
