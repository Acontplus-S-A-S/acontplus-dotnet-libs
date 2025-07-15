# Acontplus.Logging Usage Guide

Acontplus.Logging provides advanced logging for .NET applications, built on Serilog, with support for local, S3, and database sinks.

## 📦 Installation

```bash
Install-Package Acontplus.Logging
```

## 🚀 Features
- Local file, Amazon S3, and database logging
- Serilog integration and configuration
- Custom time zone enricher
- Buffered and rolling logs

## 🛠️ Basic Usage

### Configure Logging in Program.cs
```csharp
builder.Host.UseSerilog((hostContext, services, loggerConfiguration) =>
{
    loggerConfiguration.ConfigureAdvancedLogger(hostContext.Configuration, environment);
    loggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);
    loggerConfiguration.ReadFrom.Services(services);
});

builder.Services.AddAdvancedLoggingOptions(builder.Configuration);
```

### appsettings.json Example
```json
"AdvancedLogging": {
  "EnableLocalFile": true,
  "LocalFilePath": "logs/log-.log",
  "EnableS3Logging": false,
  "EnableDatabaseLogging": false,
  "TimeZoneId": "America/Guayaquil"
}
```

## 📖 See Also
- [API Reference](../Home.md)
- [Project README](../../src/Acontplus.Logging/README.md) 