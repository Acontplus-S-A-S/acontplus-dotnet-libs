using Acontplus.Core.Abstractions.Persistence;
using Acontplus.Core.DTOs.Requests;
using Acontplus.Core.DTOs.Responses;
using Acontplus.TestDomain.Entities;

namespace Acontplus.TestInfrastructure.Persistence.Repositories.Interfaces;

public interface IUserRepository : IRepository<Usuario, int>
{
    Task<PagedResult<Usuario>> GetPaginatedUsersAsync(PaginationDto pagination);
}
