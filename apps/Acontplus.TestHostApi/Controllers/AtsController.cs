namespace Acontplus.TestHostApi.Controllers;

public class AtsController(IAtsService atsService, IAtsXmlService atsXmlService) : BaseApiController
{
    // private readonly AtsXml _atsXml = new();

    [HttpGet("download")]
    public async Task<IActionResult> Download(string json)
    {
        var parameters = new Dictionary<string, object>
        {
            { "userRoleId", 428 }, { "json", SqlStringParam.Sanitize(json) }
        };
        var mapper = new AtsDataSetMapper();

        var ds = await atsService.GetAsync(parameters);
        var atsData = mapper.MapDataSetToAtsData(ds);
        var xmlBytes = await atsXmlService.CreateAtsXmlAsync(atsData);
        var fileName = "ATS" + "_" + atsData.Header.IdInformante
                       + "_" + atsData.Header.NumEstabRuc
                       + "_" + atsData.Header.Anio
                       + "_" + atsData.Header.Mes;

        // if (ds.Tables[0].Columns.Contains("code")) return BadRequest(ds.Tables[0].Rows[0].Field<string>("message"));
        // byte[]? xmlBytes = null;
        // if (!_atsXml.Create(ref ds, ref xmlBytes)) return BadRequest("No se pudo generar el ats");
        // var fileName = "ATS_" + ds.Tables["header"]?.Rows[0]["IdInformante"]
        //                       + "_" + ds.Tables["header"]?.Rows[0]["numEstabRuc"]
        //                       + "_" + ds.Tables["header"]?.Rows[0]["anio"] + "_"
        //                       + ds.Tables["header"]?.Rows[0]["mes"];

        return File(xmlBytes, "text/xml", fileName + ".xml");
    }
}