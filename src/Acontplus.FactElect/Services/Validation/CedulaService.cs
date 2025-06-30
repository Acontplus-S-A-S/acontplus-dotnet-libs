using Acontplus.FactElect.Interfaces.Services;
using Acontplus.FactElect.Models.Authentication;
using Acontplus.FactElect.Models.Validation;

namespace Acontplus.FactElect.Services.Validation;

public class CedulaService(IServiceProvider serviceProvider) : ICedulaService
{
    public async Task<CedulaModel> GetCedulaSriAsync(string numeroCedula)
    {
        if (!await CheckExistenceAsync(numeroCedula)) return new CedulaModel { error = "Cedula No existe" };

        using var scope = serviceProvider.CreateScope();
        var cookieService = scope.ServiceProvider.GetRequiredService<ICookieService>();

        var cookieContainer = await cookieService.GetAsync();

        if (cookieContainer == null) return new CedulaModel { error = "No se pudo consultar la pagina" };
        var captchaService = scope.ServiceProvider.GetRequiredService<ICaptchaService>();

        var htmlResponse = await captchaService.ValidateAsync(cookieContainer.Html, cookieContainer.Cookie);

        return htmlResponse == null
            ? new CedulaModel { error = "No se pudo validar el captcha" }
            : await GetCedulaSriAsync(numeroCedula, cookieContainer.Cookie, htmlResponse);
    }

    private async Task<bool> CheckExistenceAsync(string cedula)
    {
        using var client =
            new HttpClient(new HttpClientHandler { Credentials = CredentialCache.DefaultNetworkCredentials });
        var request = new HttpRequestMessage
        {
            RequestUri =
                new Uri(
                    "https://srienlinea.sri.gob.ec/sri-registro-civil-servicio-internet/rest/DatosRegistroCivil/existeNumeroIdentificacion?numeroIdentificacion=" +
                    cedula),
            Method = HttpMethod.Get
        };
        client.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:67.0) Gecko/20100101 Firefox/67.0");

        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode) return false;

        var stream = await response.Content.ReadAsStreamAsync();
        using var sr = new StreamReader(stream);
        var html = HttpUtility.HtmlDecode(await sr.ReadToEndAsync());
        return html == "true";
    }

    private async Task<CedulaModel> GetCedulaSriAsync(string numeroCedula, CookieContainer cookies, string html)
    {
        var personaDatosError = new CedulaModel();
        var objToken = JsonConvert.DeserializeObject<TokenSri>(html);

        var strtoken = objToken.mensaje;
        if (string.IsNullOrEmpty(html))
        {
            personaDatosError.error = " No existe contribuyente con ese Ruc";
            return personaDatosError;
        }

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
                    "https://srienlinea.sri.gob.ec/sri-registro-civil-servicio-internet/rest/DatosRegistroCivil/obtenerDatosCompletosPorNumeroIdentificacionConToken?numeroIdentificacion=" +
                    numeroCedula),
            Method = HttpMethod.Get
        };
        client.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:67.0) Gecko/20100101 Firefox/67.0");
        const string contentType = "application/json";
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
        client.DefaultRequestHeaders.Add("Authorization", strtoken);
        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode) throw new Exception();

        var stream = await response.Content.ReadAsStreamAsync();
        using var sr = new StreamReader(stream);
        html = HttpUtility.HtmlDecode(await sr.ReadToEndAsync());
        html = html.Replace("[", "");
        html = html.Replace("]", "");
        personaDatosError = JsonConvert.DeserializeObject<CedulaModel>(html);
        if (personaDatosError != null) return personaDatosError;

        personaDatosError = new CedulaModel { error = " No existe contribuyente con ese Ruc" };
        return personaDatosError;
    }
}