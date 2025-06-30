using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Acontplus.ApiDocumentation;

/// <summary>
/// Configures the Swagger generation options.
/// </summary>
/// <remarks>
/// This class is responsible for creating a distinct Swagger document for each discovered API version.
/// It dynamically pulls metadata from both the API version description and the application's configuration.
/// </remarks>
public class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IConfiguration configuration)
    : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider = provider;
    private readonly IConfiguration _configuration = configuration;

    /// <summary>
    /// Configures the SwaggerGenOptions for all discovered API versions.
    /// </summary>
    /// <param name="options">The Swagger generation options to configure.</param>
    public void Configure(SwaggerGenOptions options)
    {
        // Create a Swagger document for each discovered API version
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateVersionInfo(description));
        }
    }

    /// <summary>
    /// This overload is required by the IConfigureNamedOptions interface and simply delegates to the primary Configure method.
    /// </summary>
    /// <param name="name">The name of the options instance (not used).</param>
    /// <param name="options">The Swagger generation options to configure.</param>
    public void Configure(string? name, SwaggerGenOptions options)
    {
        Configure(options);
    }

    /// <summary>
    /// Creates the OpenApiInfo object for a given API version.
    /// </summary>
    /// <param name="description">The description of the API version.</param>
    /// <returns>An OpenApiInfo object with metadata for the Swagger document.</returns>
    private OpenApiInfo CreateVersionInfo(ApiVersionDescription description)
    {
        // Retrieve the SwaggerInfo section from appsettings.json
        var swaggerInfoSection = _configuration.GetSection("SwaggerInfo");
        var entryAssemblyName = Assembly.GetEntryAssembly()?.GetName().Name ?? "My API";

        var info = new OpenApiInfo
        {
            Title = $"{entryAssemblyName} v{description.ApiVersion}",
            Version = description.ApiVersion.ToString(),
            Description = description.IsDeprecated
                ? "This API version has been deprecated. Please use a newer version."
                : $"API documentation for {entryAssemblyName}.",
            Contact = new OpenApiContact
            {
                Name = swaggerInfoSection["ContactName"] ?? "Support Team",
                Email = swaggerInfoSection["ContactEmail"] ?? "support@example.com",
                Url = new Uri(swaggerInfoSection["ContactUrl"] ?? "https://example.com/support")
            },
            License = new OpenApiLicense
            {
                Name = swaggerInfoSection["LicenseName"] ?? "MIT",
                Url = new Uri(swaggerInfoSection["LicenseUrl"] ?? "https://opensource.org/licenses/MIT")
            }
        };

        return info;
    }
}