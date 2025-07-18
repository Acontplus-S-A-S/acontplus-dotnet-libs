namespace Acontplus.Services.Configuration;

public static class ApplicationConfigurationBuilder
{
    private static string? GetPlatformSharedFolder(IConfiguration baseConfig)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return baseConfig.GetValue<string>("SharedPaths:Windows");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return baseConfig.GetValue<string>("SharedPaths:Linux");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return baseConfig.GetValue<string>("SharedPaths:OSX");
        }

        return string.Empty;
    }

    public static IConfiguration Load()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        var baseConfig = builder.Build();

        var sharedFolder = Environment.GetEnvironmentVariable("SHARED_SETTINGS_PATH") ??
                          GetPlatformSharedFolder(baseConfig);

        if (!string.IsNullOrEmpty(sharedFolder))
        {
            var sharedFile = Path.Combine(sharedFolder, $"sharedsettings.{environment}.json");
            if (File.Exists(sharedFile))
            {
                builder.AddJsonFile(sharedFile, optional: true, reloadOnChange: true);
            }
        }

        return builder.Build();
    }
}
