namespace Acontplus.FactElect.Interfaces.Services;

public interface IRucService
{
    Task<Result<ContribuyenteCompleteDto, DomainErrors>> GetRucSriAsync(string idCard);
}