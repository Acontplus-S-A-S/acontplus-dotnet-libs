using Acontplus.Billing.Dtos.Validation;

namespace Acontplus.Billing.Interfaces.Services;

public interface IRucService
{
    Task<Result<ContribuyenteCompleteDto, DomainErrors>> GetRucSriAsync(string idCard);
}