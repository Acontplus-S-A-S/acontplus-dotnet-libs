using Acontplus.FactElect.Models.Validation;

namespace Acontplus.FactElect.Interfaces.Services;

public interface IRucService
{
    Task<RucModel> GetRucSriAsync(string ruc);
}
