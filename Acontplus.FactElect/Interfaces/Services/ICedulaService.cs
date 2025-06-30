using Acontplus.FactElect.Models.Validation;

namespace Acontplus.FactElect.Interfaces.Services;

public interface ICedulaService
{
    Task<CedulaModel> GetCedulaSriAsync(string numeroCedula);
}