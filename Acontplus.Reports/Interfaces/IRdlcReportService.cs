namespace Acontplus.Reports.Interfaces
{
    public interface IRdlcReportService
    {
        public ReportResponse GetReport(DataSet parameters, DataSet data, bool externalDirectory = false);
        public Task<ReportResponse> GetErrorAsync();
    }
}
