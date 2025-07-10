// AcontPlus.Services/Configuration/JsonConfigurationService.cs
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Acontplus.Services.Configuration;

/// <summary>
/// Service for configuring JSON serialization in enterprise applications
/// Provides centralized configuration for System.Text.Json across the application
/// </summary>
public static class JsonConfigurationService
{
    /// <summary>
    /// Get default JSON serializer options optimized for enterprise applications
    /// </summary>
    /// <returns>Configured JsonSerializerOptions</returns>
    public static JsonSerializerOptions GetDefaultOptions() => new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        WriteIndented = false, // Optimized for production
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    /// <summary>
    /// Get JSON options with pretty formatting for development/debugging
    /// </summary>
    /// <returns>Configured JsonSerializerOptions with formatting</returns>
    public static JsonSerializerOptions GetPrettyOptions() => new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        WriteIndented = true, // Pretty formatting
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    /// <summary>
    /// Get strict JSON options for critical APIs
    /// </summary>
    /// <returns>Configured JsonSerializerOptions with strict validation</returns>
    public static JsonSerializerOptions GetStrictOptions() => new()
    {
        PropertyNameCaseInsensitive = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        AllowTrailingCommas = false,
        ReadCommentHandling = JsonCommentHandling.Disallow,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        NumberHandling = JsonNumberHandling.Strict
    };

    /// <summary>
    /// Configure JSON options for ASP.NET Core applications
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="useStrictMode">Whether to use strict JSON validation</param>
    public static void ConfigureAspNetCore(IServiceCollection services, bool useStrictMode = false)
    {
        var jsonOptions = useStrictMode ? GetStrictOptions() : GetDefaultOptions();

        // Configure HTTP JSON options (for minimal APIs)
        services.ConfigureHttpJsonOptions(options =>
        {
            CopyOptionsTo(jsonOptions, options.SerializerOptions);
        });

        // Configure MVC JSON options (for controllers)
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                CopyOptionsTo(jsonOptions, options.JsonSerializerOptions);
            });
    }

    /// <summary>
    /// Configure JSON options for ASP.NET Core with custom environment settings
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="isDevelopment">Whether the application is in development mode</param>
    /// <param name="useStrictMode">Whether to use strict JSON validation</param>
    public static void ConfigureAspNetCore(IServiceCollection services, bool isDevelopment, bool useStrictMode = false)
    {
        var jsonOptions = useStrictMode ? GetStrictOptions() :
                         isDevelopment ? GetPrettyOptions() : GetDefaultOptions();

        services.ConfigureHttpJsonOptions(options =>
        {
            CopyOptionsTo(jsonOptions, options.SerializerOptions);
        });

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                CopyOptionsTo(jsonOptions, options.JsonSerializerOptions);
            });
    }

    /// <summary>
    /// Copy JSON serializer options from source to target
    /// </summary>
    /// <param name="source">Source options</param>
    /// <param name="target">Target options</param>
    private static void CopyOptionsTo(JsonSerializerOptions source, JsonSerializerOptions target)
    {
        target.PropertyNameCaseInsensitive = source.PropertyNameCaseInsensitive;
        target.PropertyNamingPolicy = source.PropertyNamingPolicy;
        target.AllowTrailingCommas = source.AllowTrailingCommas;
        target.ReadCommentHandling = source.ReadCommentHandling;
        target.WriteIndented = source.WriteIndented;
        target.DefaultIgnoreCondition = source.DefaultIgnoreCondition;
        target.NumberHandling = source.NumberHandling;

        // Copy converters
        foreach (var converter in source.Converters)
        {
            target.Converters.Add(converter);
        }
    }

    /// <summary>
    /// Register JSON configuration as a singleton service
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterJsonConfiguration(IServiceCollection services)
    {
        services.AddSingleton(provider => GetDefaultOptions());
        services.AddSingleton<IJsonConfigurationProvider, JsonConfigurationProvider>();
    }
}

/// <summary>
/// Interface for JSON configuration provider
/// </summary>
public interface IJsonConfigurationProvider
{
    JsonSerializerOptions GetDefaultOptions();
    JsonSerializerOptions GetPrettyOptions();
    JsonSerializerOptions GetStrictOptions();
}

/// <summary>
/// Implementation of JSON configuration provider
/// </summary>
public class JsonConfigurationProvider : IJsonConfigurationProvider
{
    public JsonSerializerOptions GetDefaultOptions() => JsonConfigurationService.GetDefaultOptions();
    public JsonSerializerOptions GetPrettyOptions() => JsonConfigurationService.GetPrettyOptions();
    public JsonSerializerOptions GetStrictOptions() => JsonConfigurationService.GetStrictOptions();
}