namespace Acontplus.FactElect.Interfaces.Services;

public interface ICookieService
{
    Task<Result<CookieResponse, DomainError>> GetAsync();
}