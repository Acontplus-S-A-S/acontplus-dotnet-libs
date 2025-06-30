using Acontplus.FactElect.Interfaces.Services;
using Acontplus.FactElect.Models.Authentication;
using Acontplus.FactElect.Models.Validation;

namespace Acontplus.FactElect.Services.Validation;

public class RucService(IServiceProvider serviceProvider) : IRucService
{
    public async Task<RucModel> GetRucSriAsync(string ruc)
    {
        if (!await CheckExistenceAsync(ruc)) return new RucModel { error = "Ruc no exist" };
        using var scope = serviceProvider.CreateScope();
        var cookieService = scope.ServiceProvider.GetRequiredService<ICookieService>();

        var cookieContainer = await cookieService.GetAsync();

        if (cookieContainer == null) return new RucModel { error = "No se pudo consultar la pagina" };

        var captchaService = scope.ServiceProvider.GetRequiredService<ICaptchaService>();

        var htmlResponse = await captchaService.ValidateAsync(cookieContainer.Html, cookieContainer.Cookie);

        if (htmlResponse == null) return new RucModel { error = "No se pudo validar el captcha" };

        var response = await GetRucSriAsync(ruc, cookieContainer.Cookie, htmlResponse);

        if (response.establecimientos.Count <= 0) return response;

        response.direccion = response.establecimientos[0].direccionCompleta;
        if (response.establecimientos[0].nombreFantasiaComercial != null)
            response.nombreComercial = response.establecimientos[0].nombreFantasiaComercial.ToString();
        //_response.email = "";
        //_response.telefono = "";

        return response;
    }

    private async Task<bool> CheckExistenceAsync(string ruc)
    {
        using var client =
            new HttpClient(new HttpClientHandler { Credentials = CredentialCache.DefaultNetworkCredentials });
        var request = new HttpRequestMessage
        {
            RequestUri =
                new Uri(
                    "https://srienlinea.sri.gob.ec/sri-catastro-sujeto-servicio-internet/rest/ConsolidadoContribuyente/existePorNumeroRuc?numeroRuc=" +
                    ruc),
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

    private async Task<RucModel> GetRucSriAsync(string numeroRuc, CookieContainer cookies, string html)
    {
        var personaDatosError = new RucModel();
        var objToken = JsonConvert.DeserializeObject<TokenSri>(html);

        var strtoken = objToken.mensaje;
        if (string.IsNullOrEmpty(html))
        {
            personaDatosError.error = " No existe contribuyente con ese Ruc";
            return personaDatosError;
        }

        using (var client = new HttpClient(new HttpClientHandler
        {
            Credentials = CredentialCache.DefaultNetworkCredentials,
            UseCookies = true,
            CookieContainer = cookies
        }))
        {
            var request = new HttpRequestMessage
            {
                RequestUri =
                    new Uri(
                        "https://srienlinea.sri.gob.ec/sri-catastro-sujeto-servicio-internet/rest/ConsolidadoContribuyente/obtenerPorNumerosRuc?&ruc=" +
                        numeroRuc),
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
            var respSri = JsonConvert.DeserializeObject<List<RucModel>>(html);
            personaDatosError = respSri[0];
            if (personaDatosError == null)
            {
                personaDatosError = new RucModel { error = " No existe contribuyente con ese Ruc" };
                return personaDatosError;
            }
        }

        using (var client = new HttpClient(new HttpClientHandler
        {
            Credentials = CredentialCache.DefaultNetworkCredentials,
            UseCookies = true,
            CookieContainer = cookies
        }))
        {
            var request = new HttpRequestMessage
            {
                RequestUri =
                    new Uri(
                        "https://srienlinea.sri.gob.ec/sri-catastro-sujeto-servicio-internet/rest/Establecimiento/consultarPorNumeroRuc?numeroRuc=" +
                        numeroRuc),
                Method = HttpMethod.Get
            };
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:67.0) Gecko/20100101 Firefox/67.0");
            const string contentType = "application/json";
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
            client.DefaultRequestHeaders.Add("Authorization", strtoken);
            var response = client.SendAsync(request).Result;
            if (!response.IsSuccessStatusCode) throw new Exception();

            var streamTask = response.Content.ReadAsStreamAsync();
            var stream = streamTask.Result;
            var sr = new StreamReader(stream);
            html = HttpUtility.HtmlDecode(await sr.ReadToEndAsync());
            var personaEstablecimientos = JsonConvert.DeserializeObject<List<Establecimiento>>(html);

            if (personaEstablecimientos.Count > 0) personaDatosError.establecimientos = personaEstablecimientos;
        }

        return personaDatosError;
    }
}
