namespace Acontplus.FactElect.Models.Documents;

public class XmlSriFileModel
{
    public string CodDoc { get; set; }
    public string ClaveAcceso { get; set; }
    public string FechaEmision { get; set; }
    public string VersionComp { get; set; }
    public XmlDocument XmlSri { get; set; }
    public XmlDocument XmlComprobante { get; set; }
}