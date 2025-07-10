using Acontplus.Persistence.SqlServer.Validation;

namespace Acontplus.TestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrintController(IAdoRepository adoRepository) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Print(string json)
        {
            var parameters = new Dictionary<string, object>
            {
            { "userRoleId", 27 },
            { "json", SqlStringParam.Sanitize(json) },
            { "isMobileUserAgent", "false" }
        };
            //var response = await adoRepository.QuerySingleOrDefaultAsync<ApiResponse>("Config.Print_Get", parameters);

            return Ok();
        }
    }
}
