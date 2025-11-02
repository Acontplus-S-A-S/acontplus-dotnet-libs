using Acontplus.Billing.Models.Documents;

namespace Acontplus.Billing.Interfaces.Services;

public interface IDocumentConverter
{
    string CreateHtml(ComprobanteElectronico comprobanteElectronico);
}