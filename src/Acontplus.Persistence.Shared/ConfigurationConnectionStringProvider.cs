using Acontplus.Core.Abstractions.Persistence;
using Microsoft.Extensions.Configuration;

namespace Acontplus.Persistence.Shared;

/// <summary>
/// Default implementation of IConnectionStringProvider using IConfiguration.
/// Supports hierarchical, environment-based, and secure connection string resolution.
/// </summary>
public class ConfigurationConnectionStringProvider : IConnectionStringProvider
{
    private readonly IConfiguration _configuration;
    public ConfigurationConnectionStringProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public string GetConnectionString(string name) => _configuration.GetConnectionString(name);
}