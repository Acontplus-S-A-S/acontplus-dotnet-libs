namespace Acontplus.TestInfrastructure.Persistence.Repositories.Interfaces;

public interface IUserRepository : IRepository<Usuario>
{
    Task<PagedResult<Usuario>> GetPaginatedUsersAsync(PaginationDto pagination);
}
