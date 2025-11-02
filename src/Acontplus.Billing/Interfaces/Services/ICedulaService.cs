using Acontplus.Billing.Dtos.Validation;

namespace Acontplus.Billing.Interfaces.Services;

public interface ICedulaService
{
    Task<Result<ContribuyenteCedulaDto, DomainErrors>> GetCedulaSriAsync(string cedula);
}