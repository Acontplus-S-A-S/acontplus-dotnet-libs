using Acontplus.Core.Domain.Common.Results;

namespace Acontplus.TestApplication.Interfaces;

public interface IUsuarioService
{
    Task<Result<Usuario, DomainErrors>> AddAsync(Usuario usuario);
    Task<Result<int, DomainError>> CreateAsync();
    Task<Result<PagedResult<UsuarioDto>, DomainError>> GetPaginatedUsersAsync(PaginationDto pagination);
    Task<Result<LegacySpResponse, DomainError>> GetLegacySpResponseAsync();
    Task<Result<List<UsuarioDto>, DomainError>> GetDynamicUserListAsync();
    Task<Result<Usuario, DomainErrors>> UpdateAsync(int id, Usuario usuario);
    Task<Result<bool, DomainError>> DeleteAsync(int id);
}