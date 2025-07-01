namespace Acontplus.TestHostApi.Repositories.Interfaces;

public interface IUserRepository : IRepository<Usuario, int>
{
    Task<PagedResult<Usuario>> GetPaginatedUsersAsync(PaginationDto pagination);
}
