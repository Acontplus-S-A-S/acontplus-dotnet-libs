namespace Acontplus.TestHostApi.Extensions;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
    IConfiguration configuration)
    {

        //var serviceProvider = services.BuildServiceProvider();
        //var loggingOptions = serviceProvider.GetService<LoggingOptions>();

        //// Dynamically update the log level
        //loggingOptions.UpdateLogLevel(LogEventLevel.Debug);
        return services;
    }
}
