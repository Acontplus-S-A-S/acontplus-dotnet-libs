using Acontplus.FactElect.Models.Authentication;

namespace Acontplus.FactElect.Interfaces.Services;

public interface ICookieService
{
    Task<CookieResponse> GetAsync();
}