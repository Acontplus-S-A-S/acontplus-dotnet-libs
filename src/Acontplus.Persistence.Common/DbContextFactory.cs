namespace Acontplus.Persistence.Common;

public class DbContextFactory<TContext>(IDictionary<string, TContext> contexts) : IDbContextFactory<TContext>
    where TContext : DbContext
{
    private readonly ConcurrentDictionary<string, TContext> _contexts = new(contexts);

    public TContext GetContext(string contextName)
    {
        if (_contexts.TryGetValue(contextName, out var context))
            return context;
        throw new KeyNotFoundException($"DbContext with name '{contextName}' not found.");
    }
}
