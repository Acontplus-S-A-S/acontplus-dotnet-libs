using Acontplus.Core.Domain.Common.Results;

namespace Acontplus.TestApplication.Interfaces;

public interface IUsuarioService
{
    // Standard CRUD operations
    Task<Result<Usuario, DomainErrors>> AddAsync(Usuario usuario);
    Task<Result<int, DomainError>> CreateAsync();
    Task<Result<PagedResult<UsuarioDto>, DomainError>> GetPaginatedUsersAsync(PaginationDto pagination);
    Task<Result<SpResponse, DomainError>> GetLegacySpResponseAsync();
    Task<Result<List<UsuarioDto>, DomainError>> GetDynamicUserListAsync();
    Task<Result<Usuario, DomainErrors>> UpdateAsync(int id, Usuario usuario);
    Task<Result<bool, DomainError>> DeleteAsync(int id);
    Task<Result<Usuario, DomainError>> GetByIdAsync(int id);
    Task<Result<SuccessWithWarnings<List<Usuario>>, DomainError>> ImportUsuariosAsync(List<UsuarioDto> dtos);

    // High-performance ADO.NET operations
    Task<Result<int, DomainError>> GetUserCountAsync();
    Task<Result<bool, DomainError>> CheckUserExistsAsync(string username);
    Task<Result<long, DomainError>> GetActiveUsersCountAsync();
    Task<Result<PagedResult<Usuario>, DomainError>> GetPagedUsersAdoAsync(PaginationDto pagination);
    Task<Result<PagedResult<Usuario>, DomainError>> GetPagedUsersComplexAsync(PaginationDto pagination, DateTime? createdAfter = null);
    Task<Result<PagedResult<Usuario>, DomainError>> GetPagedUsersFromStoredProcAsync(PaginationDto pagination);
    Task<Result<int, DomainError>> BulkInsertUsersAsync(List<UsuarioDto> users);
    Task<Result<int, DomainError>> ExecuteBatchOperationsAsync(List<int> userIds);
}
