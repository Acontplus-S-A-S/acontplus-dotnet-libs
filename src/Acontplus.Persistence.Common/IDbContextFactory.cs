namespace Acontplus.Persistence.Common;

public interface IDbContextFactory<TContext> where TContext : DbContext
{
    TContext GetContext(string contextName);
}
