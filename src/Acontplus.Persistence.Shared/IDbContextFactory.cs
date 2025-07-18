using Microsoft.EntityFrameworkCore;

namespace Acontplus.Persistence.Abstractions;

public interface IDbContextFactory<TContext> where TContext : DbContext
{
    TContext GetContext(string contextName);
} 