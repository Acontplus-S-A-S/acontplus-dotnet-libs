namespace Acontplus.FactElect.Dtos.Validation;

public record ContribuyenteCedulaDto
{
    public required string Identificacion { get; set; }
    public required string NombreCompleto { get; set; }
    public object? FechaDefuncion { get; set; }
    // Custom properties
    public string? Direccion { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
}