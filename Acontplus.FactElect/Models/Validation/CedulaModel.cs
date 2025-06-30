namespace Acontplus.FactElect.Models.Validation;

public class CedulaModel
{
    public string identificacion { get; set; }
    public string nombreCompleto { get; set; }
    public object fechaDefuncion { get; set; }
    public string error { get; set; }
    public bool networkError { get; set; }
    public string direccion { get; set; }
    public string email { get; set; }
    public string telefono { get; set; }
}