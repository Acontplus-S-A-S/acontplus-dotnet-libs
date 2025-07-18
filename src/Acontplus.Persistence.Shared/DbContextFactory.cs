namespace Acontplus.Persistence.Shared;

public class DbContextFactory<TContext> : IDbContextFactory<TContext> where TContext : DbContext
{
    private readonly ConcurrentDictionary<string, TContext> _contexts;
    public DbContextFactory(IDictionary<string, TContext> contexts)
    {
        _contexts = new ConcurrentDictionary<string, TContext>(contexts);
    }
    public TContext GetContext(string contextName)
    {
        if (_contexts.TryGetValue(contextName, out var context))
            return context;
        throw new KeyNotFoundException($"DbContext with name '{contextName}' not found.");
    }
}