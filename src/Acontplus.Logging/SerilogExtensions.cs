using Serilog.Formatting.Display;

namespace Acontplus.Logging;

public static class SerilogExtensions
{
    // This method will be called to CONFIGURE THE LOGGER ITSELF
    // It takes a LoggerConfiguration (which UseSerilog provides)
    public static LoggerConfiguration ConfigureAdvancedLogger(
        this LoggerConfiguration loggerConfiguration,
        IConfiguration configuration,
        string environment)
    {
        // Enable Serilog self-logging for debugging (useful during development)
        Serilog.Debugging.SelfLog.Enable(Console.WriteLine);

        // Read logging options from configuration
        var loggingOptions = new LoggingOptions();
        configuration.GetSection("AdvancedLogging").Bind(loggingOptions);

        loggerConfiguration
            .Enrich.With(new CustomTimeZoneEnricher(loggingOptions.TimeZoneId)); // Assuming CustomTimeZoneEnricher exists

        // Add console logging for development
        if (environment == Environments.Development)
        {
            loggerConfiguration.WriteTo.Console(
                outputTemplate: "{CustomTimestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
            );
        }
        else
        {
            // For production, often structured JSON for console, or no console logging if all goes to sinks
            loggerConfiguration.WriteTo.Console(formatter: new CompactJsonFormatter());
        }

        // Configure local file logging
        if (loggingOptions.EnableLocalFile && !string.IsNullOrEmpty(loggingOptions.LocalFilePath))
        {
            ConfigureLocalLogging(loggerConfiguration, loggingOptions, environment);
        }

        // Configure S3 logging
        if (loggingOptions.EnableS3Logging)
        {
            ConfigureS3Logging(loggerConfiguration, loggingOptions);
        }

        // Configure SQL Server logging
        if (loggingOptions.EnableDatabaseLogging)
        {
            ConfigureDatabaseLogging(loggerConfiguration, configuration, loggingOptions);
        }

        return loggerConfiguration; // Return the configured loggerConfiguration
    }

    // This method will be called to ADD YOUR LOGGING OPTIONS TO DI
    // It takes an IServiceCollection (which builder.Services is)
    public static IServiceCollection AddAdvancedLoggingOptions(this IServiceCollection services, IConfiguration configuration)
    {
        var loggingOptions = new LoggingOptions();
        configuration.GetSection("AdvancedLogging").Bind(loggingOptions);
        services.AddSingleton(loggingOptions); // Register LoggingOptions as a singleton in DI
        return services;
    }

    // The private helper methods (ConfigureLocalLogging, ConfigureS3Logging, ConfigureDatabaseLogging)
    // remain largely the same, as they correctly take LoggerConfiguration.
    // Ensure you use Serilog.Debugging.SelfLog.WriteLine for errors within these config methods.
    private static string GetEnvironmentName(IConfiguration configuration)
    {
        return Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ??
               Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
               Environments.Production;
    }

    private static void ConfigureLocalLogging(LoggerConfiguration loggerConfiguration, LoggingOptions options, string environment)
    {
        var rollingInterval = Enum.Parse<RollingInterval>(options.RollingInterval, true);
        var retainedFileCountLimit = options.RetainedFileCountLimit ?? 7;
        var fileSizeLimitBytes = options.FileSizeLimitBytes ?? 10 * 1024 * 1024; // 10 MB

        loggerConfiguration.WriteTo.Async(a => a.File(
            path: options.LocalFilePath,
            rollingInterval: rollingInterval,
            retainedFileCountLimit: retainedFileCountLimit,
            fileSizeLimitBytes: fileSizeLimitBytes,
            encoding: System.Text.Encoding.UTF8,
            buffered: true,
            shared: false,
            formatter: environment == Environments.Development ? new MessageTemplateTextFormatter("{CustomTimestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}") : new CompactJsonFormatter()
        ));
    }

    private static void ConfigureS3Logging(LoggerConfiguration loggerConfiguration, LoggingOptions options)
    {
        if (string.IsNullOrEmpty(options.S3BucketName) || string.IsNullOrEmpty(options.S3AccessKey) || string.IsNullOrEmpty(options.S3SecretKey))
        {
            Serilog.Debugging.SelfLog.WriteLine("S3 logging is enabled but required settings are missing. Disabling S3 logging.");
            return;
        }

        loggerConfiguration.WriteTo.Async(a => a.AmazonS3(
            path: options.LocalFilePath, // S3 path uses a similar convention
            bucketName: options.S3BucketName,
            Amazon.RegionEndpoint.USEast1, // Consider making region configurable
            awsAccessKeyId: options.S3AccessKey,
            awsSecretAccessKey: options.S3SecretKey,
            encoding: System.Text.Encoding.UTF8,
            formatter: new CompactJsonFormatter(), // Typically structured JSON for cloud sinks
            rollingInterval: Serilog.Sinks.AmazonS3.RollingInterval.Minute,
            failureCallback: e => Serilog.Debugging.SelfLog.WriteLine($"An error occurred in the S3 sink: {e.Message}")
        ));
    }

    private static void ConfigureDatabaseLogging(LoggerConfiguration loggerConfiguration, IConfiguration configuration, LoggingOptions options)
    {
        // Prioritize getting the "DefaultConnection" string
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // If "DefaultConnection" is not found or is empty,
        // you *could* fall back to options.DatabaseConnectionString if it's set.
        // Or, if you strictly want to use "DefaultConnection", remove this fallback.
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = options.DatabaseConnectionString; // Fallback
            Serilog.Debugging.SelfLog.WriteLine("DefaultConnection not found for database logging. Falling back to 'AdvancedLogging:DatabaseConnectionString'.");
        }

        if (string.IsNullOrEmpty(connectionString))
        {
            Serilog.Debugging.SelfLog.WriteLine("Database logging is enabled but no valid connection string ('DefaultConnection' or 'AdvancedLogging:DatabaseConnectionString') was found. Disabling database logging.");
            return; // Exit if no connection string is found
        }


        try
        {
            var sinkOpts = new MSSqlServerSinkOptions
            {
                TableName = "Logs",
                AutoCreateSqlTable = true,
                BatchPostingLimit = 1000,
                BatchPeriod = TimeSpan.FromSeconds(5),
            };

            var columnOpts = new ColumnOptions
            {
                LogEvent = { DataLength = 4000 }
            };

            columnOpts.Store.Remove(StandardColumn.Properties);
            columnOpts.Store.Add(StandardColumn.LogEvent);

            loggerConfiguration.WriteTo.Async(a => a.MSSqlServer(
                connectionString: connectionString,
                sinkOptions: sinkOpts,
                columnOptions: columnOpts,
                restrictedToMinimumLevel: LogEventLevel.Debug // Example: only log Information and above to DB
                                                              //appConfiguration: options.Configuration // Pass configuration to MSSqlServer sink for dynamic connection string resolution if needed
            ));

            Serilog.Debugging.SelfLog.WriteLine("Database logging configured successfully.");
        }
        catch (Exception ex)
        {
            Serilog.Debugging.SelfLog.WriteLine($"Failed to configure database logging: {ex.Message}");
        }
    }
}
