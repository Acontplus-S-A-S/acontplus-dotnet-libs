namespace Demo.Application.Interfaces;

public interface IUsuarioService
{
    // Standard CRUD operations
    Task<Result<Usuario, DomainErrors>> AddAsync(Usuario usuario);
    Task<Result<int, DomainError>> CreateAsync();
    Task<Result<PagedResult<UsuarioDto>, DomainError>> GetPaginatedUsersAsync(PaginationRequest pagination);
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
    Task<Result<PagedResult<Usuario>, DomainError>> GetPagedUsersAdoAsync(PaginationRequest pagination);
    Task<Result<PagedResult<Usuario>, DomainError>> GetPagedUsersComplexAsync(PaginationRequest pagination, DateTime? createdAfter = null);
    Task<Result<PagedResult<Usuario>, DomainError>> GetPagedUsersFromStoredProcAsync(PaginationRequest pagination);
    Task<Result<int, DomainError>> BulkInsertUsersAsync(List<UsuarioDto> users);
    Task<Result<int, DomainError>> ExecuteBatchOperationsAsync(List<int> userIds);

    // Test methods for exception handling
    Task<Result<Usuario, DomainError>> GetUserWithExceptionAsync(int id);
    Task<Result<Usuario, DomainError>> GetUserWithCustomErrorAsync(int id);
}

