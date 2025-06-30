using Acontplus.FactElect.Models.Documents;

namespace Acontplus.FactElect.Interfaces.Services;

public interface IDocumentConverter
{
    string CreateHtml(ComprobanteElectronico comprobanteElectronico);
}