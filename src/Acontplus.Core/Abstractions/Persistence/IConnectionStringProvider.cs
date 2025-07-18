namespace Acontplus.Core.Abstractions.Persistence;

/// <summary>
/// Provides a way to resolve connection strings by name or key.
/// Useful for multi-tenant, sharded, or dynamic connection scenarios.
/// </summary>
public interface IConnectionStringProvider
{
    /// <summary>
    /// Gets a connection string by logical name or key.
    /// </summary>
    string GetConnectionString(string name);
}