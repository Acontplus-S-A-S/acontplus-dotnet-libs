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
            return result.Match<IActionResult>(
                value => value is not null
                    ? result.ToCreatedActionResult($"/api/Usuario/{value.Id}")
                    : result.ToActionResult("Usuario creado exitosamente."),
                error => result.ToActionResult()
            );
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUsuario(int id, [FromBody] UsuarioDto usuarioDto)
        {
            var usuario = ObjectMapper.Map<UsuarioDto, Usuario>(usuarioDto);
            var result = await usuarioService.UpdateAsync(id, usuario);
            return result.ToPutActionResult();
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

        [HttpGet("legacy-ado")]
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

        #region High-Performance ADO.NET Endpoints

        /// <summary>
        /// Gets the total count of users using scalar query
        /// </summary>
        [HttpGet("count")]
        [ProducesResponseType<ApiResponse<int>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserCount()
        {
            var result = await usuarioService.GetUserCountAsync();
            return result.ToGetActionResult();
        }

        /// <summary>
        /// Checks if a user exists by username
        /// </summary>
        [HttpGet("exists/{username}")]
        [ProducesResponseType<ApiResponse<bool>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckUserExists(string username)
        {
            var result = await usuarioService.CheckUserExistsAsync(username);
            return result.ToGetActionResult();
        }

        /// <summary>
        /// Gets count of active users (modified in last 6 months)
        /// </summary>
        [HttpGet("active-count")]
        [ProducesResponseType<ApiResponse<long>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActiveUsersCount()
        {
            var result = await usuarioService.GetActiveUsersCountAsync();
            return result.ToGetActionResult();
        }

        /// <summary>
        /// Gets paginated users using high-performance ADO.NET with optional search
        /// </summary>
        [HttpGet("paged-ado")]
        [ProducesResponseType<ApiResponse<PagedResult<UsuarioDto>>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPagedUsersAdo([FromQuery] PaginationDto pagination)
        {
            var result = await usuarioService.GetPagedUsersAdoAsync(pagination);
            return result.ToGetActionResult();
        }

        /// <summary>
        /// Gets paginated users with complex filtering (created in last 30 days)
        /// </summary>
        [HttpGet("paged-complex")]
        [ProducesResponseType<ApiResponse<PagedResult<UsuarioDto>>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPagedUsersComplex([FromQuery] PaginationDto pagination)
        {
            var result = await usuarioService.GetPagedUsersComplexAsync(pagination);
            return result.ToGetActionResult();
        }

        /// <summary>
        /// Gets paginated users from stored procedure with email domain filter
        /// </summary>
        [HttpGet("paged-sp")]
        [ProducesResponseType<ApiResponse<PagedResult<UsuarioDto>>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPagedUsersFromStoredProc([FromQuery] PaginationDto pagination, [FromQuery] string? emailDomain = null)
        {
            var result = await usuarioService.GetPagedUsersFromStoredProcAsync(pagination, emailDomain);
            return result.ToGetActionResult();
        }

        /// <summary>
        /// Bulk inserts users using SqlBulkCopy for maximum performance
        /// </summary>
        [HttpPost("bulk")]
        [ProducesResponseType<ApiResponse<int>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> BulkInsertUsers([FromBody] List<UsuarioDto> users)
        {
            var result = await usuarioService.BulkInsertUsersAsync(users);
            return result.ToActionResult("Usuarios insertados exitosamente.");
        }

        /// <summary>
        /// Executes batch operations (updates) in a single transaction
        /// </summary>
        [HttpPost("batch")]
        [ProducesResponseType<ApiResponse<int>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> ExecuteBatchOperations([FromBody] List<int> userIds)
        {
            var result = await usuarioService.ExecuteBatchOperationsAsync(userIds);
            return result.ToActionResult("Operaciones batch ejecutadas exitosamente.");
        }

        #endregion

        private string CreatePageLink(PaginationDto pagination, int page)
        {
            return $"?page={page}&pageSize={pagination.PageSize}";
        }
    }
}
