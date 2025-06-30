using Acontplus.FactElect.Models.Responses;

namespace Acontplus.FactElect.Interfaces.Services;

public interface IWebServiceSri
{
    public Task<ResponseSri> AuthorizationAsync(string claveAcceso, string url);
    public Task<ResponseSri> AuthorizationLoteAsync(string claveAcceso, string url);
    public Task<ResponseSri> CheckExistenceAsync(string claveAcceso, string url);
    public Task<string> GetXmlAsync(string claveAcceso, string url);
    public Task<ResponseSri> ReceptionAsync(string xmlSigned, string url);
}