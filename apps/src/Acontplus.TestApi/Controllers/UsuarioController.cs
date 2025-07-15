using Acontplus.Services.Extensions;
using Acontplus.Utilities.Mapping;

namespace Acontplus.TestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController(IUsuarioService usuarioService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UsuarioDto usuarioDto)
        {
            var usuario = ObjectMapper.Map<UsuarioDto, Usuario>(usuarioDto);
            var result = await usuarioService.AddAsync(usuario);
            // Return 201 Created if successful, otherwise error
            // Assume new user's ID is available in result.Value.Id if successful
            if (result.IsSuccess && result.Value is not null)
            {
                var locationUri = $"/api/Usuario/{result.Value.Id}";
                return result.ToCreatedActionResult(locationUri);
            }
            // For error or null, use generic action result
            return result.ToActionResult();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UsuarioDto usuarioDto)
        {
            var usuario = ObjectMapper.Map<UsuarioDto, Usuario>(usuarioDto);
            var result = await usuarioService.UpdateAsync(id, usuario);
            // Return 200 OK if updated, 204 NoContent if no data, or error
            return result.ToPutActionResult();
        }

        // [HttpDelete("{id}")]
        // public async Task<IActionResult> Put(int id)
        // {
        //     return Ok(await usuarioService.DeleteAsync(id));
        // }

        [HttpGet]
        [ProducesResponseType<ApiResponse<PagedResult<UsuarioDto>>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiResponse>(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUsers(
            [FromQuery] PaginationDto pagination,
            [FromServices] IUsuarioService usuarioService,
            [FromServices] ILogger<UsuarioController> logger)
        {
            var isMobileRequest = HttpContext.GetIsMobileRequest();
            var result = await usuarioService.GetPaginatedUsersAsync(pagination);
            // Return 200 OK if data, 204 NoContent if empty, or error
            return result.ToGetActionResult();
        }

        [HttpGet("ado")]
        public async Task<IActionResult> GetUsersAdo(
            [FromServices] IUsuarioService usuarioService,
            [FromServices] ILogger<UsuarioController> logger)
        {
            var result = await usuarioService.GetLegacySpResponseAsync();
            return result.ToGetActionResult();
        }

        [HttpGet("get-dynamic")]
        public async Task<IActionResult> GetDynamicUsers(
            [FromServices] IUsuarioService usuarioService,
            [FromServices] ILogger<UsuarioController> logger)
        {
            var result = await usuarioService.GetDynamicUserListAsync();
            return result.ToGetActionResult();
        }

        private string CreatePageLink(PaginationDto pagination, int page)
        {
            return $"?page={page}&pageSize={pagination.PageSize}";
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var correlationId = HttpContext.TraceIdentifier;
            var result = await usuarioService.DeleteAsync(id);
            return result.ToDeleteActionResult();
        }
    }
}