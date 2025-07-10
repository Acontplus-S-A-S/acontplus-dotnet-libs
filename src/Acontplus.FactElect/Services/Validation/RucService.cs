using System.Text.Json;

namespace Acontplus.FactElect.Services.Validation;

public class RucService(IServiceProvider serviceProvider) : IRucService
{
    public async Task<Result<ContribuyenteCompleteDto, DomainErrors>> GetRucSriAsync(string ruc)
    {
        var errors = new List<DomainError>();

        if (string.IsNullOrWhiteSpace(ruc))
        {
            errors.Add(DomainError.Validation("RUC_REQUIRED", "RUC es requerido"));
        }
        else if (ruc.Length != 13)
        {
            errors.Add(DomainError.Validation("RUC_INVALID_LENGTH", "RUC debe tener 13 dígitos"));
        }

        if (errors.Count > 0)
        {
            return Result<ContribuyenteCompleteDto, DomainErrors>.Failure(DomainErrors.Multiple(errors));
        }

        var checkExistenceResult = await CheckExistenceAsync(ruc);
        if (!checkExistenceResult.IsSuccess)
        {
            return Result<ContribuyenteCompleteDto, DomainErrors>.Failure(checkExistenceResult.Error);
        }

        using var scope = serviceProvider.CreateScope();
        var cookieService = scope.ServiceProvider.GetRequiredService<ICookieService>();

        var cookieResponse = await cookieService.GetAsync();

        if (!cookieResponse.IsSuccess)
        {
            return Result<ContribuyenteCompleteDto, DomainErrors>.Failure(cookieResponse.Error);
        }

        var captchaService = scope.ServiceProvider.GetRequiredService<ICaptchaService>();

        var captchaResponse =
            await captchaService.ValidateAsync(cookieResponse.Value.Captcha, cookieResponse.Value.Cookie);

        if (!captchaResponse.IsSuccess)
        {
            return Result<ContribuyenteCompleteDto, DomainErrors>.Failure(captchaResponse.Error);
        }

        if (errors.Count != 0)
        {
            return Result<ContribuyenteCompleteDto, DomainErrors>.Failure(DomainErrors.Multiple(errors));
        }

        var response = await GetRucSriAsync(ruc, cookieResponse.Value.Cookie, captchaResponse.Value);

        return !response.IsSuccess
            ? Result<ContribuyenteCompleteDto, DomainErrors>.Failure(response.Error)
            : Result<ContribuyenteCompleteDto, DomainErrors>.Success(response.Value);
    }

    private async Task<Result<bool, DomainError>> CheckExistenceAsync(string ruc)
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
        if (!response.IsSuccessStatusCode)
            return Result<bool, DomainError>.Failure(new DomainError
            {
                Code = "RUC_CHECK_ERROR",
                Message = "Error al verificar la existencia del RUC"
            });

        var stream = await response.Content.ReadAsStreamAsync();
        using var sr = new StreamReader(stream);
        var htmlDecode = HttpUtility.HtmlDecode(await sr.ReadToEndAsync());
        if (htmlDecode == "true")
        {
            return Result<bool, DomainError>.Success(true);
        }

        return Result<bool, DomainError>.Failure(new DomainError
        {
            Code = "RUC_NOT_FOUND",
            Message = "RUC No existe"
        });
    }

    private async Task<Result<ContribuyenteCompleteDto, DomainError>> GetRucSriAsync(string ruc,
        CookieContainer cookieContainer,
        string captcha)
    {
        var captchaDeserialized = JsonExtensions.DeserializeModern<TokenSri>(captcha);

        var tokenSri = captchaDeserialized.mensaje;

        using var client = new HttpClient(new HttpClientHandler
        {
            Credentials = CredentialCache.DefaultNetworkCredentials,
            UseCookies = true,
            CookieContainer = cookieContainer
        });
        var request = new HttpRequestMessage
        {
            RequestUri =
                new Uri(
                    "https://srienlinea.sri.gob.ec/sri-catastro-sujeto-servicio-internet/rest/ConsolidadoContribuyente/obtenerPorNumerosRuc?&ruc=" +
                    ruc),
            Method = HttpMethod.Get
        };
        client.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:67.0) Gecko/20100101 Firefox/67.0");
        const string contentType = "application/json";
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
        client.DefaultRequestHeaders.Add("Authorization", tokenSri);
        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            return Result<ContribuyenteCompleteDto, DomainError>.Failure(new DomainError
            {
                Code = "RUC_FETCH_ERROR",
                Message = "Error al obtener los datos del RUC"
            });

        var stream = await response.Content.ReadAsStreamAsync();
        using var streamReader = new StreamReader(stream);
        var sriResponse = HttpUtility.HtmlDecode(await streamReader.ReadToEndAsync());
        var rucs = JsonExtensions.DeserializeModern<List<ContribuyenteRucDto>>(sriResponse);
        if (rucs.Count == 0 || rucs[0].NumeroRuc != ruc)
        {
            return Result<ContribuyenteCompleteDto, DomainError>.Failure(new DomainError
            {
                Code = "RUC_NOT_FOUND",
                Message = "No existe contribuyente con ese RUC"
            });
        }


        var consolidatedRuc = await GetRucSriWithEstablishmentsAsync(rucs[0], cookieContainer, tokenSri);
        return !consolidatedRuc.IsSuccess
            ? Result<ContribuyenteCompleteDto, DomainError>.Failure(consolidatedRuc.Error)
            : Result<ContribuyenteCompleteDto, DomainError>.Success(consolidatedRuc.Value);
    }

    private async Task<Result<ContribuyenteCompleteDto, DomainError>> GetRucSriWithEstablishmentsAsync(
        ContribuyenteRucDto contribuyenteRucDto,
        CookieContainer cookieContainer,
        string tokenSri)
    {
        using var client = new HttpClient(new HttpClientHandler
        {
            Credentials = CredentialCache.DefaultNetworkCredentials,
            UseCookies = true,
            CookieContainer = cookieContainer
        });
        var request = new HttpRequestMessage
        {
            RequestUri =
                new Uri(
                    "https://srienlinea.sri.gob.ec/sri-catastro-sujeto-servicio-internet/rest/Establecimiento/consultarPorNumeroRuc?numeroRuc=" +
                    contribuyenteRucDto.NumeroRuc),
            Method = HttpMethod.Get
        };
        client.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:67.0) Gecko/20100101 Firefox/67.0");
        const string contentType = "application/json";
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
        client.DefaultRequestHeaders.Add("Authorization", tokenSri);
        var sriResponse = client.SendAsync(request).Result;
        if (!sriResponse.IsSuccessStatusCode)
            return Result<ContribuyenteCompleteDto, DomainError>.Success(new ContribuyenteCompleteDto()
            {
                Contribuyente = contribuyenteRucDto,
                Establecimientos = new List<EstablecimientoDto>()
            });

        var stream = await sriResponse.Content.ReadAsStreamAsync();
        using var streamReader = new StreamReader(stream);
        var serializedEstabs = HttpUtility.HtmlDecode(await streamReader.ReadToEndAsync());
        var establecimientos = JsonExtensions.DeserializeModern<List<EstablecimientoDto>>(serializedEstabs);

        var response = new ContribuyenteCompleteDto
        {
            Contribuyente = contribuyenteRucDto,
            Establecimientos = new List<EstablecimientoDto>()
        };
        if (establecimientos.Count > 0) response.Establecimientos = establecimientos;

        response.Contribuyente.Direccion = response.Establecimientos[0].DireccionCompleta;
        if (response.Establecimientos[0].NombreFantasiaComercial != null)
            response.Contribuyente.NombreComercial =
                response.Establecimientos[0].NombreFantasiaComercial;

        return Result<ContribuyenteCompleteDto, DomainError>.Success(response);
    }
}