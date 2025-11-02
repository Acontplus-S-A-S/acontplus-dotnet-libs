using Acontplus.Billing.Models.Authentication;

namespace Acontplus.Billing.Interfaces.Services;

public interface ICookieService
{
    Task<Result<CookieResponse, DomainError>> GetAsync();
}