namespace Acontplus.TestHostApi.Services;

public interface IAtsService
{
    Task<ApiResponse> CheckValidationAsync(Dictionary<string, object> parameters);
    Task<DataSet> GetAsync(Dictionary<string, object> parameters);
}

public class AtsService(IUnitOfWork uow) : IAtsService
{
    private readonly IAdoRepository _adoRepository = uow.AdoRepository;
    private const string ModuleName = "FactElect.Ats_";

    public async Task<ApiResponse> CheckValidationAsync(Dictionary<string, object> parameters)
    {
        return await _adoRepository.QuerySingleOrDefaultAsync<ApiResponse>(
            $"{ModuleName}CheckValidation",
            parameters
        );
    }

    public async Task<DataSet> GetAsync(Dictionary<string, object> parameters)
    {
        var options = new CommandOptionsDto
        {
            CommandTimeout = 0
        };

        return await _adoRepository.GetDataSetAsync(
            $"{ModuleName}Get",
            parameters,
            options
        );
    }
}
