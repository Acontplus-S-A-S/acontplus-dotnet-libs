using Acontplus.Core.DTOs.Responses;
using System.Data;

namespace Acontplus.TestApplication.Interfaces;

public interface IAtsService
{
    Task<LegacySpResponse> CheckValidationAsync(Dictionary<string, object> parameters);
    Task<DataSet> GetAsync(Dictionary<string, object> parameters);
}
