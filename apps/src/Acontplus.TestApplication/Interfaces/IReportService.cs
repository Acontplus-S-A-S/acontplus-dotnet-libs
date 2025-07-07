namespace Acontplus.TestApplication.Interfaces;

public interface IReportService
{
    Task<DataSet> GetParamsAsync(Dictionary<string, object> parameters);
    Task<DataSet> GetDataAsync(string spname, Dictionary<string, object> parameters, bool withTableNames = false);
}