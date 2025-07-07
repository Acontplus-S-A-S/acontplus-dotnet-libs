using Acontplus.Core.Abstractions.Persistence;
using Acontplus.Core.Domain.Common;
using Acontplus.Core.Domain.Enums;
using Acontplus.Core.Domain.Exceptions;
using Acontplus.Core.DTOs.Requests;
using Acontplus.Core.DTOs.Responses;
using Acontplus.Core.Extensions;
using Acontplus.Data;
using Acontplus.TestApplication.DTOs;
using Acontplus.TestApplication.Interfaces;
using Acontplus.TestDomain.Entities;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;

namespace Acontplus.TestApplication.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IRepository<Usuario, int> _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UsuarioService> _logger;
        private readonly ISqlExceptionTranslator _exceptionTranslator;

        public UsuarioService(
            IUnitOfWork unitOfWork,
            ILogger<UsuarioService> logger, ISqlExceptionTranslator exceptionTranslator)
        {
            _usuarioRepository = unitOfWork.GetRepository<Usuario, int>();
            _unitOfWork = unitOfWork;
            _logger = logger;
            _exceptionTranslator = exceptionTranslator;
        }

        public async Task<Result<Usuario, DomainErrors>> AddAsync(Usuario usuario)
        {
            try
            {
                var existingUser = await _usuarioRepository.GetFirstOrDefaultAsync(x => x.Username == usuario.Username);

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
                    details: new Dictionary<string, object> { ["username"] = usuario.Username }));
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
                    details: new Dictionary<string, object> { ["userId"] = id });
            }
        }

        public async Task<Result<List<UsuarioDto>, DomainError>> GetDynamicUserListAsync()
        {
            try
            {
                var options = new CommandOptionsDto
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = 30
                };

                var result = await _unitOfWork.AdoRepository.QueryAsync<UsuarioDto>(
                    "SELECT * FROM dbo.Usuarios",
                    options: options);

                return Result<List<UsuarioDto>, DomainError>.Success(result);
            }
            // catch (DbException ex)
            // {
            //     _logger.LogError(ex, "Failed to execute raw SQL command");
            //     return DomainError.Internal(
            //         "SQL_COMMAND_FAILED",
            //         "Failed to execute database command");
            // }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute raw SQL command");
                return DomainError.Internal(
                    "SQL_COMMAND_FAILED",
                    "Failed to execute database command");
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

                return result.IsSuccess
                    ? Result<LegacySpResponse, DomainError>.Success(result)
                    : DomainError.Internal(result.Code, result.Message);
            }
            catch (Exception ex)
            {
                var domainEx = _exceptionTranslator.Translate(ex);

                // Handle validation errors first
                if (domainEx.ErrorType == ErrorType.Validation)
                {
                    return Result.Failure<LegacySpResponse>(
                        DomainError.Validation(
                            code: domainEx.ErrorCode,
                            message: domainEx.Message));
                }

                // Then handle transient errors
                if (_exceptionTranslator.IsTransient(ex))
                {
                    return Result.Failure<LegacySpResponse>(
                        DomainError.ServiceUnavailable(
                            code: "DB_TRANSIENT_ERROR",
                            message: "Database temporarily unavailable",
                            // shouldRetry: true,
                            details: new Dictionary<string, object>
                            {
                                ["procedure"] = "dbo.sp_Test",
                                ["error"] = ex.Message
                            }));
                }

                // Fallback to internal error
                return Result.Failure<LegacySpResponse>(
                    DomainError.Internal(
                        code: "SP_EXECUTION_ERROR",
                        message: "Failed to execute stored procedure",
                        details: new Dictionary<string, object>
                        {
                            ["procedure"] = "dbo.sp_Test",
                            ["error"] = ex.Message
                        }));
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
                    var usernameExists =
                        await _usuarioRepository.ExistsAsync(x => x.Username == usuario.Username && x.Id != id);

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
                    details: new Dictionary<string, object> { ["userId"] = id }));
            }
        }
    }
}