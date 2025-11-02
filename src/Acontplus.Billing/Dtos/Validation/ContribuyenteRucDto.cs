using System.ComponentModel.DataAnnotations;

namespace Acontplus.Billing.Dtos.Validation;

// Date information for taxpayer
public record InformacionFechasContribuyenteDto
{
    public required string FechaInicioActividades { get; set; }
    public required string FechaCese { get; set; }
    public required string FechaReinicioActividades { get; set; }
    public required string FechaActualizacion { get; set; }
}

// Main RUC (taxpayer) information
public record ContribuyenteRucDto
{
    [Required] public required string NumeroRuc { get; set; }

    [Required] public required string RazonSocial { get; set; }

    [Required] public required string EstadoContribuyenteRuc { get; set; }

    [Required] public required string ActividadEconomicaPrincipal { get; set; }

    [Required] public required string TipoContribuyente { get; set; }

    [Required] public required string Regimen { get; set; }

    [Required] public required string Categoria { get; set; }

    [Required] public required string ObligadoLlevarContabilidad { get; set; }

    [Required] public required string AgenteRetencion { get; set; }

    [Required] public required string ContribuyenteEspecial { get; set; }

    [Required] public required InformacionFechasContribuyenteDto InformacionFechasContribuyente { get; set; }

    public object[]? RepresentantesLegales { get; set; }

    public string? MotivoCancelacionSuspension { get; set; }

    [Required] public required string ContribuyenteFantasma { get; set; }

    [Required] public required string TransaccionesInexistente { get; set; }

    //Custom properties
    public string? NombreComercial { get; set; }
    public string? Direccion { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
}

// Establishment information
public record EstablecimientoDto
{
    public string? NombreFantasiaComercial { get; set; }

    [Required] public required string TipoEstablecimiento { get; set; }

    [Required] public required string DireccionCompleta { get; set; }

    [Required] public required string Estado { get; set; }

    [Required] public required string NumeroEstablecimiento { get; set; }

    [Required] public required string Matriz { get; set; }
}

// Combined response DTO
public record ContribuyenteCompleteDto
{
    [Required] public required ContribuyenteRucDto Contribuyente { get; set; }

    [Required] public required List<EstablecimientoDto> Establecimientos { get; set; }
}