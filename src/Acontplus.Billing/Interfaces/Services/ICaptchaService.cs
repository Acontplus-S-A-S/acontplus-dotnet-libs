namespace Acontplus.Billing.Interfaces.Services;

public interface ICaptchaService
{
    Task<Result<string, DomainError>> ValidateAsync(string captcha, CookieContainer cookies);
}