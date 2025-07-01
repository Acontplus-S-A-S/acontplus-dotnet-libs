using Acontplus.Utilities.Extensions;

namespace Acontplus.TestHostApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController(IUsuarioService usuarioService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UsuarioDto usuarioDto)
        {
            var usuario = ObjectMapper.Map<UsuarioDto, Usuario>(usuarioDto);
            var serialized = DataConverters.SerializeObjectCustom<Usuario>(usuario);


            return await usuarioService.AddAsync(usuario).ToActionResultAsync();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UsuarioDto usuarioDto)
        {
            var usuario = ObjectMapper.Map<UsuarioDto, Usuario>(usuarioDto);

            return Ok(await usuarioService.UpdateAsync(id, usuario));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Put(int id)
        {

            return Ok(await usuarioService.DeleteAsync(id));
        }

        [HttpGet]
        [ProducesResponseType<ApiResponse<PagedResult<UsuarioDto>>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiResponse>(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUsers(
            [FromQuery] PaginationDto pagination,
            [FromServices] IUsuarioService usuarioService,
            [FromServices] ILogger<UsuarioController> logger)
        {
            return await usuarioService.GetPaginatedUsersAsync(pagination).ToActionResultAsync();
        }
        [HttpGet("ado")]
        public async Task<IActionResult> GetUsersAdo(
            [FromServices] IUsuarioService usuarioService,
            [FromServices] ILogger<UsuarioController> logger)
        {
            return await usuarioService.GetLegacySpResponseAsync().ToActionResultAsync();
        }

        private string CreatePageLink(PaginationDto pagination, int page)
        {
            return $"?page={page}&pageSize={pagination.PageSize}";
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var correlationId = HttpContext.TraceIdentifier;
            return await usuarioService.DeleteAsync(id).ToActionResultAsync(correlationId);
        }
    }
}
