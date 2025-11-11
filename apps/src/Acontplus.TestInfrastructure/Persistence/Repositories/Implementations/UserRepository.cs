using Acontplus.Persistence.Common.Repositories;

namespace Acontplus.TestInfrastructure.Persistence.Repositories.Implementations;

public class UserRepository : BaseRepository<Usuario>, IUserRepository
{
    public UserRepository(TestContext context) : base(context)
    {
    }

    public async Task<PagedResult<Usuario>> GetPaginatedUsersAsync(PaginationRequest pagination)
    {
        IQueryable<Usuario> query = _dbSet;

        // Apply specific filters based on pagination DTO
        //if (!string.IsNullOrEmpty(pagination.TextSearch))
        //{
        //    query = query.Where(u => u.FirstName.Contains(pagination.TextSearch) ||
        //                            u.LastName.Contains(pagination.TextSearch) ||
        //                            u.Email.Contains(pagination.TextSearch));
        //}

        //if (pagination.UserId > 0)
        //{
        //    query = query.Where(u => u.Id == pagination.UserId);
        //}

        // Example of sorting - you might want to make this more dynamic
        //query = query.OrderBy(u => u.LastName);
        query = query.OrderBy(u => u.Username);

        // Get total count for pagination metadata
        var totalCount = await query.CountAsync();

        // Apply pagination
        var items = await query
            .Skip((pagination.PageIndex - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PagedResult<Usuario>
        {
            Items = items,
            PageIndex = pagination.PageIndex,
            PageSize = pagination.PageSize,
            TotalCount = totalCount
        };
    }
}

