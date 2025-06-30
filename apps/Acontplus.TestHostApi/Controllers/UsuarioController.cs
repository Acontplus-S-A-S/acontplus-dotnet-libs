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


            return Ok(await usuarioService.AddAsync(usuario));
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
        public async Task<ActionResult<ApiResponse<PagedResult<UsuarioDto>>>> GetUsers([FromQuery] PaginationDto pagination)
        {
            var result = await usuarioService.GetPaginatedUsersAsync(pagination);

            return Ok(ApiResponse<PagedResult<UsuarioDto>>.Success(result));
        }
    }
}
