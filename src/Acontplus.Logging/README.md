# Acontplus.Logging

## Description
`Acontplus.Logging` is a library that provides an advanced logging system for .NET applications, built on top of Serilog. It allows storing logs in local files, Amazon S3, or a database, depending on the configuration defined in `appsettings.json`. It seamlessly integrates with the .NET Generic Host and ASP.NET Core applications.

## Installation
To install the library, run the following command in the NuGet Package Manager Console:
```bash
Install-Package Acontplus.Logging
```
Or using the .NET CLI:
```bash
dotnet add package Acontplus.Logging
```
Additionally, ensure you have the appropriate Serilog integration package for your host:
- For ASP.NET Core Web APIs (with `WebApplication.CreateBuilder`):
  ```bash
  dotnet add package Serilog.AspNetCore
  ```
- For Generic Hosts (like Worker Services):
  ```bash
  dotnet add package Serilog.Extensions.Hosting
  ```

## Configuration
To enable and customize the logging system, edit the `appsettings.json` file and add the `AdvancedLogging` section. This section will control the behavior of your custom sinks.

```json
"AdvancedLogging": {
    "EnableLocalFile": true,
    "Shared": false,
    "Buffered": true,
    "LocalFilePath": "logs/log-.log",
    "RollingInterval": "Day",
    "RetainedFileCountLimit": 7,
    "FileSizeLimitBytes": 10485760, // 10MB in bytes
    "EnableS3Logging": false,
    "S3BucketName": "my-application-logs",
    "S3AccessKey": "your-access-key",
    "S3SecretKey": "your-secret-key",
    "EnableDatabaseLogging": false,
    "DatabaseConnectionString": "Server=...",
    "TimeZoneId": "America/Guayaquil"
}
```

You can also leverage Serilog's built-in configuration capabilities via the `Serilog` section in `appsettings.json` to define default log levels and global enrichers, which will merge with your `AdvancedLogging` settings:

```json
"Serilog": {
  "MinimumLevel": {
    "Default": "Information",
    "Override": {
      "Microsoft": "Warning",
      "System": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Enrich": [
    "FromLogContext",
    "WithEnvironmentUserName",
    "WithMachineName"
  ],
  "Properties": {
    "Application": "YourAppName"
  }
}
```

### Configuration Options
- **EnableLocalFile** *(bool)*: Enables or disables storing logs in local files.
- **Shared** *(bool)*: Enables or disables shared log files (multiple processes can write to the same file).
- **Buffered** *(bool)*: Enables or disables buffered logging for local files (improves performance by writing in chunks).
- **LocalFilePath** *(string)*: Path to the log file. It can include `{Date}` to generate a file per day (e.g., `logs/log-.log`).
- **RollingInterval** *(string)*: Interval to roll log files. Possible values: `Year`, `Month`, `Day`, `Hour`, `Minute`.
- **RetainedFileCountLimit** *(int)*: Number of historical log files to keep.
- **FileSizeLimitBytes** *(int)*: Maximum size of a single log file in bytes before it rolls over.
- **EnableS3Logging** *(bool)*: Enables or disables storing logs in Amazon S3.
- **S3BucketName** *(string)*: Name of the S3 bucket where logs will be stored.
- **S3AccessKey** *(string)*: AWS access key for the S3 bucket (use environment variables or secrets in production).
- **S3SecretKey** *(string)*: AWS secret key for the S3 bucket (use environment variables or secrets in production).
- **EnableDatabaseLogging** *(bool)*: Enables or disables storing logs in a database.
- **DatabaseConnectionString** *(string)*: Connection string to the database where logs will be stored (use secrets in production).
- **TimeZoneId** *(string)*: Time zone ID for the custom timestamp enricher (e.g., "America/Guayaquil", "UTC").

## Usage

Integrating `Acontplus.Common.Logging` involves two main steps in your `Program.cs` file to ensure Serilog is properly configured and your custom options are available in the Dependency Injection container.

```csharp
using Acontplus.Logging; // Your NuGet namespace
using Serilog;
using Microsoft.Extensions.DependencyInjection; // For IServiceCollection and casting
using Microsoft.Extensions.Hosting; // For Environments
using Microsoft.AspNetCore.Builder; // For WebApplication.CreateBuilder (if using ASP.NET Core)
using System; // For Exception and Task

public class Program
{
    public static void Main(string[] args)
    {
        // 1. Optional: Create a bootstrap logger for very early startup issues.
        //    This temporary logger captures logs before the main Serilog configuration is built.
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            var builder = WebApplication.CreateBuilder(args); // For ASP.NET Core Web API
            // Or for a Generic Host (Worker Service): var builder = Host.CreateDefaultBuilder(args);

            var environment = builder.Environment.EnvironmentName;

            // 2. Configure Serilog for the host. This integrates Serilog with Microsoft.Extensions.Logging.
            //    Call your ConfigureAdvancedLogger extension within this callback.
            builder.Host.UseSerilog((hostContext, services, loggerConfiguration) =>
            {
                // Apply your custom advanced logging settings from appsettings.json
                // This method configures Serilog's sinks, enrichers, and base minimum level.
                loggerConfiguration.ConfigureAdvancedLogger(hostContext.Configuration, environment);

                // Read additional Serilog configuration from the "Serilog" section of appsettings.json.
                // This merges with your programmatic configuration and allows for overrides/additions.
                loggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);
                // Resolve services for Serilog sinks/enrichers if they need DI.
                loggerConfiguration.ReadFrom.Services(services);
            });

            // 3. Register your custom LoggingOptions class into the Dependency Injection container.
            //    This makes your 'AdvancedLogging' configuration available to other services if needed.
            builder.Services.AddAdvancedLoggingOptions(builder.Configuration);

            // Add other application services here (e.g., builder.Services.AddControllers();)
            // ... your application service registrations ...

            var app = builder.Build();

            // 4. (For ASP.NET Core APIs) Add Serilog's request logging middleware.
            //    Place this early in your HTTP request pipeline to capture request/response details.
            app.UseSerilogRequestLogging();

            // ... your other middleware configurations ...
            // e.g., app.UseHttpsRedirection();
            // e.g., app.UseAuthorization();
            // e.g., app.MapControllers();

            app.Run();
        }
        catch (Exception ex)
        {
            // Catch any critical startup errors and log them using the bootstrap logger
            Log.Fatal(ex, "Host terminated unexpectedly during bootstrap.");
        }
        finally
        {
            // Ensure all buffered logs are flushed on application shutdown
            Log.CloseAndFlush();
        }
    }
}
```

## Requirements
- .NET 6 or higher
- Proper write permissions if `EnableLocalFile` is enabled.
- AWS account with S3 permissions if `EnableS3Logging` is enabled.
- Accessible database if `EnableDatabaseLogging` is enabled.

## Contributions
Contributions to improve this library are welcome. To report bugs or suggestions, open an issue in the official repository.

## License
This project is licensed under the MIT License - see the LICENSE file for details.

## Author

[Ivan Paz](https://linktr.ee/iferpaz7)

## Company

[Acontplus S.A.S.](https://acontplus.com.ec)
```