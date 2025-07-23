# Acontplus.Persistence.Common

[![NuGet](https://img.shields.io/nuget/v/Acontplus.Persistence.Common.svg)](https://www.nuget.org/packages/Acontplus.Persistence.Common)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

Common persistence abstractions and infrastructure for Acontplus .NET libraries. Includes generic repository, context factory, connection string provider, and mapping utilities for multi-provider support (SQL Server, PostgreSQL, etc).

## 🚀 Features
- Generic repository and context factory patterns
- Connection string provider abstraction and implementation
- Designed for multi-tenant, sharded, and cloud-native scenarios
- .NET 9+ compatible

## 📦 Installation

### NuGet Package Manager
```bash
Install-Package Acontplus.Persistence.Common
```

### .NET CLI
```bash
dotnet add package Acontplus.Persistence.Common
```

### PackageReference
```xml
<ItemGroup>
  <PackageReference Include="Acontplus.Persistence.Common" Version="1.0.0" />
</ItemGroup>
```

## 🎯 Quick Start

### 1. Register in DI
```csharp
services.AddSingleton<IConnectionStringProvider, ConfigurationConnectionStringProvider>();
services.AddSingleton<IDbContextFactory<MyDbContext>, DbContextFactory<MyDbContext>>();
```

### 2. Use in Your Code
```csharp
var contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<MyDbContext>>();
var dbContext = contextFactory.GetContext("TenantA");

var connProvider = serviceProvider.GetRequiredService<IConnectionStringProvider>();
var connString = connProvider.GetConnectionString("DefaultConnection");
```

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👨‍💻 Author

**Ivan Paz** - [@iferpaz7](https://linktr.ee/iferpaz7)

## 🏢 Company

**[Acontplus S.A.S.](https://acontplus.com.ec)** - Enterprise software solutions

---

**Built with ❤️ for the .NET community** 
