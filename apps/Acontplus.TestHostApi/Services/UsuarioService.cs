using System.Linq.Expressions;

namespace Acontplus.TestHostApi.Services
{
    public interface IUsuarioService
    {
        Task<ApiResponse> AddAsync(Usuario usuario);
        Task<int> CreateAsync();
        Task<PagedResult<UsuarioDto>> GetPaginatedUsersAsync(PaginationDto pagination);
        Task<ApiResponse> UpdateAsync(int id, Usuario usuario);
        Task<ApiResponse> DeleteAsync(int id);
    }

    public class UsuarioService(
        IUnitOfWork unitOfWork,
        ILogger<UsuarioService>? logger = null) : IUsuarioService
    {
        private readonly IRepository<Usuario> _usuarioRepository = unitOfWork.GetRepository<Usuario>();

        public async Task<ApiResponse> AddAsync(Usuario usuario)
        {
            try
            {
                await _usuarioRepository.AddAsync(usuario);
                await unitOfWork.SaveChangesAsync();
                return ApiResponse.Success();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error adding usuario");
                return ApiResponse.Error("0", "Error adding usuario");
            }
        }

        public async Task<int> CreateAsync()
        {
            var options = new CommandOptionsDto
            {
                CommandType = CommandType.Text,
                CommandTimeout = 30 // Set a timeout of 30 seconds
            };

            return await unitOfWork.AdoRepository.ExecuteNonQueryAsync(
                "INSERT INTO Test.WorkerTest(Content) VALUES ('Inserting')",
                options: options);
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            try
            {
                var userFound = await _usuarioRepository.GetByIdAsync(id);
                if (userFound == null)
                {
                    return ApiResponse.Error("0", "User not found");
                }

                userFound.MarkAsDeleted();
                await _usuarioRepository.UpdateAsync(userFound);
                await unitOfWork.SaveChangesAsync();

                return ApiResponse.Success(payload: userFound);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error deleting usuario with id {Id}", id);
                return ApiResponse.Error("0", "Error deleting usuario");
            }
        }

        public async Task<PagedResult<UsuarioDto>> GetPaginatedUsersAsync(PaginationDto paginationDto)
        {
            try
            {
                // Example filtering - adjust as needed
                Expression<Func<Usuario, bool>> filter = u => !u.IsDeleted;

                // Example sorting - could be parameterized
                Expression<Func<Usuario, object>> orderBy = u => u.CreatedAt;
                bool orderByDescending = true;

                // Get paged data from repository
                var pagedResult = await _usuarioRepository.GetPagedAsync(
                    pagination: paginationDto,
                    predicate: filter,
                    orderBy: orderBy,
                    orderByDescending: orderByDescending);

                // Map to DTOs
                var userDtos = pagedResult.Items.Select(user => ObjectMapper.Map<Usuario, UsuarioDto>(user)).ToList();

                return new PagedResult<UsuarioDto>
                {
                    Items = userDtos,
                    PageIndex = pagedResult.PageIndex,
                    PageSize = pagedResult.PageSize,
                    TotalCount = pagedResult.TotalCount
                };
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error getting paginated users");
                throw;
            }
        }
        public async Task<ApiResponse> UpdateAsync(int id, Usuario usuario)
        {
            try
            {
                var userFound = await _usuarioRepository.GetByIdAsync(id);
                if (userFound == null)
                {
                    return ApiResponse.Error("0", "User not found");
                }

                // Map updated properties from input usuario to userFound
                // This depends on your mapping strategy

                await _usuarioRepository.UpdateAsync(userFound);
                await unitOfWork.SaveChangesAsync();

                return ApiResponse.Success(payload: userFound);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error updating usuario with id {Id}", id);
                return ApiResponse.Error("0", "Error updating usuario");
            }
        }
    }
}
