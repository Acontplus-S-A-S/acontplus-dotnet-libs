namespace Acontplus.Core.Abstractions.Persistence;

public interface IConnectionStringProvider
{
    string GetConnectionString(string name);
}
