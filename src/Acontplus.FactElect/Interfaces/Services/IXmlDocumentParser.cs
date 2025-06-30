namespace Acontplus.FactElect.Interfaces.Services;

public interface IXmlDocumentParser<T> where T : class
{
    bool TryParse(XmlDocument xmlDocument, out T result, out string errorMessage);
}