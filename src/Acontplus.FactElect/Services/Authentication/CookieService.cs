using Acontplus.FactElect.Interfaces.Services;
using Acontplus.FactElect.Models.Authentication;

namespace Acontplus.FactElect.Services.Authentication;

public class CookieService : ICookieService
{
    public async Task<CookieResponse> GetAsync()
    {
        var cookies = new CookieContainer();
        var generator = new Random();
        var numeroGenerado = generator.Next(0, 100000000).ToString("D6");
        using var client = new HttpClient(new HttpClientHandler
        {
            Credentials = CredentialCache.DefaultNetworkCredentials,
            UseCookies = true,
            CookieContainer = cookies
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

        if (!response.IsSuccessStatusCode) return null;

        var stream = await response.Content.ReadAsStreamAsync();
        using var sr = new StreamReader(stream);
        return new CookieResponse { Cookie = cookies, Html = HttpUtility.HtmlDecode(await sr.ReadToEndAsync()) };
    }
}