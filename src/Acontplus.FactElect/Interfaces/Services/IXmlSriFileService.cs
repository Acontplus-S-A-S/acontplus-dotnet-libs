namespace Acontplus.FactElect.Interfaces.Services;

public interface IXmlSriFileService
{
    Task<XmlSriFileModel> GetAsync(IFormFile file);
}