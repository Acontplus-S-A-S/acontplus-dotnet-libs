namespace Acontplus.FactElect.Models.Validation;

public class ClasificacionMiPyme
{
    public string clasificacionMiPyme;
}

public class Establecimiento
{
    public object nombreFantasiaComercial { get; set; }
    public string tipoEstablecimiento { get; set; }
    public string direccionCompleta { get; set; }
    public string estado { get; set; }
    public string numeroEstablecimiento { get; set; }
    public string matriz { get; set; }
}

public class InformacionFechasContribuyente
{
    public string fechaInicioActividades { get; set; }
    public string fechaCese { get; set; }
    public string fechaReinicioActividades { get; set; }
    public string fechaActualizacion { get; set; }
}

public class RepresentantesLegales
{
    public string identificacion { get; set; }
    public string nombre { get; set; }
}

public class RucModel
{
    public string numeroRuc { get; set; }
    public string razonSocial { get; set; }
    public string estadoContribuyenteRuc { get; set; }
    public string actividadEconomicaPrincipal { get; set; }
    public string tipoContribuyente { get; set; }
    public string regimen { get; set; }
    public object categoria { get; set; }
    public string obligadoLlevarContabilidad { get; set; }
    public string agenteRetencion { get; set; }
    public string contribuyenteEspecial { get; set; }
    public InformacionFechasContribuyente informacionFechasContribuyente { get; set; }
    public List<RepresentantesLegales> representantesLegales { get; set; }
    public object motivoCancelacionSuspension { get; set; }
    public string contribuyenteFantasma { get; set; }
    public string transaccionesInexistente { get; set; }
    public ClasificacionMiPyme clasificacionMiPyme { get; set; }

    public List<Establecimiento> establecimientos { get; set; }

    //custom
    public string error { get; set; }
    public bool networkError { get; set; }
    public string nombreComercial { get; set; }
    public string direccion { get; set; }
    public string email { get; set; }
    public string telefono { get; set; }
}