namespace Acontplus.FactElect.Services.Validation;

public class CedulaService(IServiceProvider serviceProvider) : ICedulaService
{
    public async Task<Result<ContribuyenteCedulaDto, DomainErrors>> GetCedulaSriAsync(string cedula)
    {
        var errors = new List<DomainError>();

        if (string.IsNullOrWhiteSpace(cedula))
        {
            errors.Add(DomainError.Validation("CEDULA_REQUIRED", "Cédula es requerido"));
        }
        else if (cedula.Length != 10)
        {
            errors.Add(DomainError.Validation("CEDULA_INVALID_LENGTH", "Cédula debe tener 10 dígitos"));
        }

        if (errors.Count > 0)
        {
            return Result<ContribuyenteCedulaDto, DomainErrors>.Failure(DomainErrors.Multiple(errors));
        }

        var checkExistence = await CheckExistenceAsync(cedula);
        if (!checkExistence.IsSuccess)
        {
            return Result<ContribuyenteCedulaDto, DomainErrors>.Failure(checkExistence.Error);
        }

        using var scope = serviceProvider.CreateScope();
        var cookieService = scope.ServiceProvider.GetRequiredService<ICookieService>();

        var cookieContainer = await cookieService.GetAsync();

        if (!cookieContainer.IsSuccess)
        {
            return Result<ContribuyenteCedulaDto, DomainErrors>.Failure(cookieContainer.Error);
        }

        var captchaService = scope.ServiceProvider.GetRequiredService<ICaptchaService>();

        var captchaResponse =
            await captchaService.ValidateAsync(cookieContainer.Value.Captcha, cookieContainer.Value.Cookie);

        if (!captchaResponse.IsSuccess)
        {
            return Result<ContribuyenteCedulaDto, DomainErrors>.Failure(captchaResponse.Error);
        }

        var response = await GetCedulaSriAsync(cedula, cookieContainer.Value.Cookie, captchaResponse.Value);
        return !response.IsSuccess
            ? Result<ContribuyenteCedulaDto, DomainErrors>.Failure(response.Error)
            : Result<ContribuyenteCedulaDto, DomainErrors>.Success(response.Value);
    }

    private async Task<Result<bool, DomainError>> CheckExistenceAsync(string cedula)
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
        if (!response.IsSuccessStatusCode)
            return Result<bool, DomainError>.Failure(new DomainError
            {
                Code = "CEDULA_CHECK_ERROR",
                Message = "No se pudo consultar la existencia de la cédula"
            });

        var stream = await response.Content.ReadAsStreamAsync();
        using var sr = new StreamReader(stream);
        var html = HttpUtility.HtmlDecode(await sr.ReadToEndAsync());
        if (html == "true")
        {
            return Result<bool, DomainError>.Success(true);
        }

        return Result<bool, DomainError>.Failure(new DomainError
        {
            Code = "CEDULA_NOT_FOUND",
            Message = "No existe contribuyente con esa cédula"
        });
    }

    private async Task<Result<ContribuyenteCedulaDto, DomainError>> GetCedulaSriAsync(string cedula, CookieContainer cookies,
        string captcha)
    {
        var captchaDeserialized = JsonExtensions.DeserializeModern<TokenSri>(captcha);

        var tokenSri = captchaDeserialized.Mensaje;

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
                    cedula),
            Method = HttpMethod.Get
        };
        client.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:67.0) Gecko/20100101 Firefox/67.0");
        const string contentType = "application/json";
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
        client.DefaultRequestHeaders.Add("Authorization", tokenSri);
        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode) throw new Exception();

        var stream = await response.Content.ReadAsStreamAsync();
        using var streamReader = new StreamReader(stream);
        var sriResponse = HttpUtility.HtmlDecode(await streamReader.ReadToEndAsync());
        sriResponse = sriResponse.Replace("[", "");
        sriResponse = sriResponse.Replace("]", "");
        var result = JsonExtensions.DeserializeModern<ContribuyenteCedulaDto>(sriResponse);
        if (result != null) return Result<ContribuyenteCedulaDto, DomainError>.Success(result);
        return Result<ContribuyenteCedulaDto, DomainError>.Failure(new DomainError
        {
            Code = "CEDULA_NOT_FOUND",
            Message = "No existe contribuyente con esa cédula"
        });
    }
}