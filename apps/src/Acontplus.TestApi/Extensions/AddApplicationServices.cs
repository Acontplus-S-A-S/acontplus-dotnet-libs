using System.Text.Json;
using System.Text.Json.Serialization;

namespace Acontplus.TestApi.Extensions;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
    IConfiguration configuration)
    {


        services.AddHttpContextAccessor();
        //var serviceProvider = services.BuildServiceProvider();
        //var loggingOptions = serviceProvider.GetService<LoggingOptions>();

        //// Dynamically update the log level
        //loggingOptions.UpdateLogLevel(LogEventLevel.Debug);

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            options.SerializerOptions.TypeInfoResolver = ApiResponseJsonContext.Default;
        });
        return services;
    }
}
