namespace Acontplus.TestInfrastructure.Persistence.Repositories.Interfaces;

public interface IUserRepository : IRepository<Usuario, int>
{
    Task<PagedResult<Usuario>> GetPaginatedUsersAsync(PaginationDto pagination);
}
