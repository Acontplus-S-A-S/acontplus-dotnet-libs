# Acontplus.Notifications

[![NuGet](https://img.shields.io/nuget/v/Acontplus.Notifications.svg)](https://www.nuget.org/packages/Acontplus.Notifications)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A .NET 9+ library for notifications: email, MailKit, Amazon SES, WhatsApp, and push. Includes templates, queueing, and advanced delivery options.

## 🚀 Features

- Email notifications via MailKit and Amazon SES
- WhatsApp and push notification support
- Email queueing and retry logic
- Templated email support (Scriban)
- Dependency Injection ready
- Advanced delivery and error handling

## 📦 Installation

### NuGet Package Manager
```bash
Install-Package Acontplus.Notifications
```

### .NET CLI
```bash
dotnet add package Acontplus.Notifications
```

### PackageReference
```xml
<ItemGroup>
  <PackageReference Include="Acontplus.Notifications" Version="1.0.16" />
</ItemGroup>
```

## 🎯 Quick Start

### 1. Configure Services
```csharp
// Register services in DI container
services.AddSingleton<IMailKitService, MailKitService>();
services.AddSingleton<IAmazonSesService, AmazonSesService>(); // Note: AmazonSesService implements IMailKitService
```

### 2. Send an Email
```csharp
public class EmailSender
{
    private readonly IMailKitService _mailKitService;
    public EmailSender(IMailKitService mailKitService) => _mailKitService = mailKitService;
    public async Task SendAsync(EmailModel email)
    {
        await _mailKitService.SendAsync(email, CancellationToken.None);
    }
}
```

### 3. Send via Amazon SES
```csharp
public class SesSender
{
    private readonly IAmazonSesService _sesService;
    public SesSender(IAmazonSesService sesService) => _sesService = sesService;
    public async Task SendAsync(EmailModel email)
    {
        await _sesService.SendAsync(email, CancellationToken.None);
    }
}
```

## 📚 API Documentation

- `IMailKitService` - Email sending interface (implemented by both MailKit and Amazon SES services)
- `AmazonSesService` - Amazon SES email service
- `MailKitService` - MailKit email service
- `EmailModel` - Email message model
- `Notification` - Notification entity

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details.

### Development Setup
```bash
git clone https://github.com/Acontplus-S-A-S/acontplus-dotnet-libs.git
cd acontplus-dotnet-libs
dotnet restore
dotnet build
```

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

- 📧 Email: proyectos@acontplus.com
- 🐛 Issues: [GitHub Issues](https://github.com/Acontplus-S-A-S/acontplus-dotnet-libs/issues)
- 📖 Documentation: [Wiki](https://github.com/Acontplus-S-A-S/acontplus-dotnet-libs/wiki)

## 👨‍💻 Author

**Ivan Paz** - [@iferpaz7](https://linktr.ee/iferpaz7)

## 🏢 Company

**[Acontplus S.A.S.](https://acontplus.com.ec)** - Software solutions

---

**Built with ❤️ for the .NET community**
