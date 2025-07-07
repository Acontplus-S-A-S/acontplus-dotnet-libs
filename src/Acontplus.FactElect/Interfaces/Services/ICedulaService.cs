namespace Acontplus.FactElect.Interfaces.Services;

public interface ICedulaService
{
    Task<Result<ContribuyenteCedulaDto, DomainErrors>> GetCedulaSriAsync(string cedula);
}