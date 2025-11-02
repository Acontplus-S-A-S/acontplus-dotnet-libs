using Acontplus.Billing.Interfaces.Services;
using Acontplus.Billing.Models.Authentication;

namespace Acontplus.Billing.Services.Authentication;

public class CookieService : ICookieService
{
    public async Task<Result<CookieResponse, DomainError>> GetAsync()
    {
        var cookie = new CookieContainer();
        var generator = new Random();
        var numeroGenerado = generator.Next(0, 100000000).ToString("D6");
        using var client = new HttpClient(new HttpClientHandler
        {
            Credentials = CredentialCache.DefaultNetworkCredentials,
            UseCookies = true,
            CookieContainer = cookie
        });
        var request = new HttpRequestMessage
        {
            RequestUri =
                new Uri("https://srienlinea.sri.gob.ec/sri-captcha-servicio-internet/captcha/start/1?r=" +
                        numeroGenerado),
            Method = HttpMethod.Get
        };
        client.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:67.0) Gecko/20100101 Firefox/67.0");

        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return Result<CookieResponse, DomainError>.Failure(new DomainError
            {
                Code = "COOKIE_ERROR",
                Message = "No se pudo consultar la pagina"
            });

        var stream = await response.Content.ReadAsStreamAsync();
        using var streamReader = new StreamReader(stream);
        return Result<CookieResponse, DomainError>.Success(new CookieResponse
        { Cookie = cookie, Captcha = HttpUtility.HtmlDecode(await streamReader.ReadToEndAsync()) });
    }
}