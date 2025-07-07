namespace Acontplus.FactElect.Services.External;

public class CaptchaService : ICaptchaService
{
    public async Task<Result<string, DomainError>> ValidateAsync(string captcha, CookieContainer cookies)
    {
        var captchaImage = JsonConvert.DeserializeObject<CaptchaImageDto>(captcha);
        var captchaImageValue = captchaImage?.Values[0];

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
                    captchaImageValue + "?emitirToken=true"),
            Method = HttpMethod.Get
        };
        client.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:67.0) Gecko/20100101 Firefox/67.0");
        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            return Result<string, DomainError>.Failure(new DomainError
            {
                Code = "CAPTCHA_ERROR",
                Message = "No se pudo validar el captcha"
            });

        var stream = await response.Content.ReadAsStreamAsync();
        using var sr = new StreamReader(stream);
        return Result<string, DomainError>.Success(HttpUtility.HtmlDecode(await sr.ReadToEndAsync()));
        ;
    }
}