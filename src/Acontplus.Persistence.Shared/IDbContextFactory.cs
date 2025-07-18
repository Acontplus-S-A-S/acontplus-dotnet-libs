namespace Acontplus.Persistence.Shared;

public interface IDbContextFactory<TContext> where TContext : DbContext
{
    TContext GetContext(string contextName);
}