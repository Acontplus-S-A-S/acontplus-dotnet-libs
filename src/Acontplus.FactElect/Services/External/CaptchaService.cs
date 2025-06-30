using Acontplus.FactElect.Interfaces.Services;
using Acontplus.FactElect.Models.Validation;

namespace Acontplus.FactElect.Services.External;

public class CaptchaService : ICaptchaService
{
    public async Task<string> ValidateAsync(string html, CookieContainer cookies)
    {
        var captchaImage = JsonConvert.DeserializeObject<CaptchaImage>(html);
        var captcha = captchaImage.values[0];

        using var client = new HttpClient(new HttpClientHandler
        {
            Credentials = CredentialCache.DefaultNetworkCredentials,
            UseCookies = true,
            CookieContainer = cookies
        });
        var request = new HttpRequestMessage
        {
            RequestUri =
                new Uri(
                    "https://srienlinea.sri.gob.ec/sri-captcha-servicio-internet/rest/ValidacionCaptcha/validarCaptcha/" +
                    captcha + "?emitirToken=true"),
            Method = HttpMethod.Get
        };
        client.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:67.0) Gecko/20100101 Firefox/67.0");
        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;

        var stream = await response.Content.ReadAsStreamAsync();
        using var sr = new StreamReader(stream);
        return HttpUtility.HtmlDecode(await sr.ReadToEndAsync());
    }
}