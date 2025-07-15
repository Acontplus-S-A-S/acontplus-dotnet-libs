# Acontplus.Notifications Usage Guide

Acontplus.Notifications provides advanced notification services: email (MailKit, Amazon SES), WhatsApp, push, templates, and queueing.

## 📦 Installation

```bash
Install-Package Acontplus.Notifications
```

## 🚀 Features
- Email via MailKit and Amazon SES
- WhatsApp and push notification support
- Email queueing and retry logic
- Templated emails (Scriban)
- Dependency Injection ready

## 🛠️ Basic Usage

### Configure Notification Services
```csharp
services.AddAcontplusNotifications(Configuration);
```

### Send an Email
```csharp
await mailKitService.SendAsync(emailModel);
```

### Send via Amazon SES
```csharp
await sesService.SendAsync(emailModel);
```

## 📖 See Also
- [API Reference](../Home.md)
- [Project README](../../src/Acontplus.Notifications/README.md) 