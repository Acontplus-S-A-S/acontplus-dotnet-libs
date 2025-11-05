using Acontplus.Utilities.Mapping;

namespace Acontplus.TestApi.Controllers.Business
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController(IUsuarioService usuarioService) : ControllerBase
    {
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUsuario(int id)
        {
            var result = await usuarioService.GetByIdAsync(id);
            return result.ToGetActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUsuario([FromBody] UsuarioDto usuarioDto)
        {
            var usuario = ObjectMapper.Map<UsuarioDto, Usuario>(usuarioDto);
            var result = await usuarioService.AddAsync(usuario);
            if (result.IsSuccess && result.Value is not null)
            {
                var locationUri = $"/api/Usuario/{result.Value.Id}";
                // Custom message example
                return ApiResponse<Usuario>.Success(result.Value, new ApiResponseOptions { Message = "Usuario creado exitosamente." })
                    .ToActionResult();
            }
            return result.ToActionResult();
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUsuario(int id, [FromBody] UsuarioDto usuarioDto)
        {
            var usuario = ObjectMapper.Map<UsuarioDto, Usuario>(usuarioDto);
            var result = await usuarioService.UpdateAsync(id, usuario);
            return result.IsSuccess
                ? ApiResponse<Usuario>.Success(result.Value, new ApiResponseOptions { Message = "Usuario actualizado correctamente." }).ToActionResult()
                : result.ToActionResult();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var result = await usuarioService.DeleteAsync(id);
            return result.ToDeleteActionResult();
        }

        [HttpGet]
        [ProducesResponseType<ApiResponse<PagedResult<UsuarioDto>>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiResponse>(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUsuarios([FromQuery] PaginationDto pagination)
        {
            var result = await usuarioService.GetPaginatedUsersAsync(pagination);
            return result.ToGetActionResult();
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportUsuarios([FromBody] List<UsuarioDto> dtos)
        {
            var result = await usuarioService.ImportUsuariosAsync(dtos);
            return result.ToActionResult();
        }

        [HttpGet("ado")]
        public async Task<IActionResult> GetUsersAdo()
        {
            var result = await usuarioService.GetLegacySpResponseAsync();
            return result.ToGetActionResult();
        }

        [HttpGet("get-dynamic")]
        public async Task<IActionResult> GetDynamicUsers()
        {
            var result = await usuarioService.GetDynamicUserListAsync();
            return result.ToGetActionResult();
        }

        private string CreatePageLink(PaginationDto pagination, int page)
        {
            return $"?page={page}&pageSize={pagination.PageSize}";
        }
    }
}
