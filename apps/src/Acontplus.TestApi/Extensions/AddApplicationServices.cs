using Acontplus.Services.Configuration;

namespace Acontplus.TestApi.Extensions;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddTestServices(this IServiceCollection services,
    IConfiguration configuration)
    {


        services.AddHttpContextAccessor();

        services.AddOpenApi();




        JsonConfigurationService.ConfigureAspNetCore(services, useStrictMode: false);

        JsonConfigurationService.RegisterJsonConfiguration(services);

        return services;
    }
}

