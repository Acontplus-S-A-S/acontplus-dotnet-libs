namespace Acontplus.TestApplication.Interfaces;

public interface IAtsService
{
    Task<SpResponse> CheckValidationAsync(Dictionary<string, object> parameters);
    Task<DataSet> GetAsync(Dictionary<string, object> parameters);
}
