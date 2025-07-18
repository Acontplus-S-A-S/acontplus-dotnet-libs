namespace Acontplus.Persistence.Postgres.Context;

public class DbContextFactory(IDictionary<string, BaseContext> context)
{
    public BaseContext GetContext(string contextName)
    {
        return context[contextName];
    }
}
