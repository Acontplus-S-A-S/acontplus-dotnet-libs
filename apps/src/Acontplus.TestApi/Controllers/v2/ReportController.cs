using Acontplus.Persistence.SqlServer.Utilities;
using Acontplus.Utilities.Data;

namespace Acontplus.TestApi.Controllers.v2;

[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class ReportController(
    IReportService reportService,
    IRdlcReportService rdlcReportService,
    IEmailService emailService,
    IConfiguration configuration) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var cantidad = Convert.ToInt32(configuration.GetSection("cantidad").Value);
        var dt = await emailService.GetAsync(cantidad);
        var dataRow = dt.Rows[0];

        var mainParams = JsonConvert.DeserializeObject<Notification>(dataRow.Field<string>("params"));
        var emailData = JsonConvert.DeserializeObject<EmailModel>(dataRow.Field<string>("emailData"));


        if (mainParams.hasFile)
        {
            var reportFile = new FileModel();
            await HandleReportGeneration(mainParams, reportFile);
            var files = new List<FileModel> { reportFile };
            emailData.Files = files;
        }

        return File(emailData.Files[0].Content, "application/pdf", emailData.Files[0].FileName);
    }

    private async Task HandleReportGeneration(Notification mainParams, FileModel reportFile)
    {
        var dsParams = await reportService.GetParamsAsync(new Dictionary<string, object>
        {
            { "json", SqlStringParam.Sanitize(DataConverters.SerializeDictionary(mainParams.reportParams)) }
        });

        if (dsParams.Tables["ReportProps"].Rows.Count == 0) throw new Exception("Report data not found");

        var storedProcedure = dsParams.Tables["ReportProps"].Rows[0].Field<string>("storedProcedure");
        var dataParams = new Dictionary<string, object>
        {
            ["json"] = DataConverters.SerializeDictionary(mainParams.spParams)
        };
        var ds = await reportService.GetDataAsync(storedProcedure, dataParams, mainParams.withTableNames);

        if (ds.Tables[0].Rows.Count == 0) throw new Exception("No report data found");

        var response = rdlcReportService.GetReport(dsParams, ds, true);

        if (response == null) throw new Exception("Error in RDLC generation");

        FileModelBuilder.Create(response.FileContents, response.ContentType, response.FileDownloadName);
    }
}
