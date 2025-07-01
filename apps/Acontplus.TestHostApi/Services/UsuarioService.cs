using Acontplus.Core.Domain.Common;
using System.Data.Common;
using System.Linq.Expressions;
using Acontplus.Core.Extensions;

namespace Acontplus.TestHostApi.Services
{
    public interface IUsuarioService
    {
        Task<Result<Usuario, DomainErrors>> AddAsync(Usuario usuario);
        Task<Result<int, DomainError>> CreateAsync();
        Task<Result<PagedResult<UsuarioDto>, DomainError>> GetPaginatedUsersAsync(PaginationDto pagination);
        Task<Result<LegacySpResponse, DomainError>> GetLegacySpResponseAsync();
        Task<Result<Usuario, DomainErrors>> UpdateAsync(int id, Usuario usuario);
        Task<Result<bool, DomainError>> DeleteAsync(int id);
    }

    public class UsuarioService : IUsuarioService
    {
        private readonly IRepository<Usuario> _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(
            IUnitOfWork unitOfWork,
            ILogger<UsuarioService> logger)
        {
            _usuarioRepository = unitOfWork.GetRepository<Usuario>();
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<Usuario, DomainErrors>> AddAsync(Usuario usuario)
        {
            try
            {
                var existingUser = await _usuarioRepository.GetFirstOrDefaultAsync(
                    x => x.Username == usuario.Username);

                if (existingUser is not null)
                {
                    return DomainErrors.Single(DomainError.Conflict(
                        "USERNAME_EXISTS",
                        $"Username '{usuario.Username}' already exists"));
                }

                var addedUser = await _usuarioRepository.AddAsync(usuario);
                await _unitOfWork.SaveChangesAsync();

                return Result<Usuario, DomainErrors>.Success(addedUser);
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "Database error adding user {Username}", usuario.Username);
                return DomainErrors.Single(DomainError.Internal(
                    "DB_INSERT_ERROR",
                    "Failed to create user due to database error",
                    new Dictionary<string, object> { ["username"] = usuario.Username }));
            }
        }

        public async Task<Result<int, DomainError>> CreateAsync()
        {
            try
            {
                var options = new CommandOptionsDto
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = 30
                };

                var result = await _unitOfWork.AdoRepository.ExecuteNonQueryAsync(
                    "INSERT INTO Test.WorkerTest(Content) VALUES ('Inserting')",
                    options: options);

                return Result<int, DomainError>.Success(result);
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "Failed to execute raw SQL command");
                return DomainError.Internal(
                    "SQL_COMMAND_FAILED",
                    "Failed to execute database command");
            }
        }

        public async Task<Result<bool, DomainError>> DeleteAsync(int id)
        {
            try
            {
                var userFound = await _usuarioRepository.GetByIdAsync(id);
                if (userFound == null)
                {
                    return DomainError.NotFound(
                        "USER_NOT_FOUND",
                        $"User with ID {id} not found");
                }

                userFound.MarkAsDeleted();
                await _usuarioRepository.UpdateAsync(userFound);
                await _unitOfWork.SaveChangesAsync();

                return Result<bool, DomainError>.Success(true);
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "Database error deleting user {UserId}", id);
                return DomainError.Internal(
                    "DB_DELETE_ERROR",
                    "Failed to delete user",
                    new Dictionary<string, object> { ["userId"] = id });
            }
        }

        public async Task<Result<LegacySpResponse, DomainError>> GetLegacySpResponseAsync()
        {
            try
            {
                var options = new CommandOptionsDto
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = 30
                };

                var result = await _unitOfWork.AdoRepository.QuerySingleOrDefaultAsync<LegacySpResponse>(
                    "dbo.sp_Test",
                    options: options);

                if (result.IsSuccess)
                {
                    return Result<LegacySpResponse, DomainError>.Success(result);
                }
                return DomainError.Internal(result.Code, result.Message);

            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "Failed to execute raw SQL command");
                return DomainError.Internal(
                    "SQL_COMMAND_FAILED",
                    "Failed to execute database command");
            }
        }

        public async Task<Result<PagedResult<UsuarioDto>, DomainError>> GetPaginatedUsersAsync(
            PaginationDto paginationDto)
        {
            try
            {
                Expression<Func<Usuario, bool>> filter = u => !u.IsDeleted;
                Expression<Func<Usuario, object>> orderBy = u => u.CreatedAt;

                var pagedResult = await _usuarioRepository.GetPagedAsync(
                    pagination: paginationDto,
                    predicate: filter,
                    orderBy: orderBy,
                    orderByDescending: true);

                var userDtos = pagedResult.Items
                    .Select(ObjectMapper.Map<Usuario, UsuarioDto>)
                    .ToList();

                var links = pagedResult.BuildPaginationLinks(
    baseRoute: "/api/users",
    pageSize: paginationDto.PageSize);

                var metadata = new Dictionary<string, object>
                {
                    ["filter"] = new { IsDeleted = false },
                    ["sort"] = "CreatedAt DESC",
                    ["links"] = links
                };

                return new PagedResult<UsuarioDto>(
                    items: userDtos,
                    pageIndex: pagedResult.PageIndex,
                    pageSize: pagedResult.PageSize,
                    totalCount: pagedResult.TotalCount,
                    metadata: metadata);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paginated users");
                return DomainError.Internal(
                    code: "PAGINATION_ERROR",
                    message: "Failed to retrieve paginated users",
                    details: new Dictionary<string, object>
                    {
                        ["page"] = paginationDto.PageIndex,
                        ["pageSize"] = paginationDto.PageSize
                    });
            }
        }

        public async Task<Result<Usuario, DomainErrors>> UpdateAsync(int id, Usuario usuario)
        {
            try
            {
                var userFound = await _usuarioRepository.GetByIdAsync(id);
                if (userFound == null)
                {
                    return DomainErrors.Single(DomainError.NotFound(
                        "USER_NOT_FOUND",
                        $"User with ID {id} not found"));
                }

                // Validate username uniqueness if changed
                if (userFound.Username != usuario.Username)
                {
                    var usernameExists = await _usuarioRepository.ExistsAsync(
                        x => x.Username == usuario.Username && x.Id != id);

                    if (usernameExists)
                    {
                        return DomainErrors.Single(DomainError.Conflict(
                            "USERNAME_EXISTS",
                            $"Username '{usuario.Username}' already exists"));
                    }
                }

                // Update properties
                userFound.Username = usuario.Username;
                userFound.Email = usuario.Email;
                // ... other properties

                await _usuarioRepository.UpdateAsync(userFound);
                await _unitOfWork.SaveChangesAsync();

                return Result<Usuario, DomainErrors>.Success(userFound);
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "Database error updating user {UserId}", id);
                return DomainErrors.Single(DomainError.Internal(
                    "DB_UPDATE_ERROR",
                    "Failed to update user",
                    new Dictionary<string, object> { ["userId"] = id }));
            }
        }
    }
}
