namespace Acontplus.Persistence.Common;

public class DbContextFactory<TContext>(IDictionary<string, TContext> contexts) : IDbContextFactory<TContext>
    where TContext : DbContext
{
    private readonly ConcurrentDictionary<string, TContext> _contexts = new(contexts);

    public TContext GetContext(string contextName)
    {
        return _contexts.TryGetValue(contextName, out var context)
            ? context
            : throw new KeyNotFoundException($"DbContext with name '{contextName}' not found.");
    }
}
