# Acontplus.Persistence.SqlServer

[![NuGet](https://img.shields.io/nuget/v/Acontplus.Persistence.SqlServer.svg)](https://www.nuget.org/packages/Acontplus.Persistence.SqlServer)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

A .NET 9+ library for SQL Server persistence, ADO.NET, and Entity Framework Core integration. Includes repositories, context management, and advanced error handling for robust data access.

## 🚀 Features

- **Database context management** - Base context and factory for EF Core
- **Repository pattern** - Generic and ADO.NET repositories
- **SQL Server integration** - Optimized for SQL Server
- **Data reader mapping utilities** - Fast mapping from DbDataReader
- **Parameter handling helpers** - Safe and flexible parameterization
- **Advanced error handling** - Custom exceptions and translators

## 📦 Installation

### NuGet Package Manager
```bash
Install-Package Acontplus.Persistence.SqlServer
```

### .NET CLI
```bash
dotnet add package Acontplus.Persistence.SqlServer
```

### PackageReference
```xml
<ItemGroup>
  <PackageReference Include="Acontplus.Persistence.SqlServer" Version="1.0.14" />
</ItemGroup>
```

## 🎯 Quick Start

### 1. Register DbContext and Repository
```csharp
services.AddDbContext<BaseContext>(options =>
    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
```

### 2. Use ADO.NET Repository
```csharp
var repo = serviceProvider.GetRequiredService<IAdoRepository>();
var result = await repo.ExecuteQueryAsync("SELECT * FROM MyTable");
```

## 🔧 Advanced Usage

### Custom Exception Handling
```csharp
try {
    // ... data access code ...
} catch (RepositoryException ex) {
    // handle repository errors
}
```

### DataTable Mapping
```csharp
var entities = DbDataReaderMapper.MapToList<MyEntity>(reader);
```

## 📚 API Documentation

- `BaseContext` - EF Core base context for SQL Server
- `AdoRepository` - ADO.NET repository for SQL Server with retry and error handling
- `RepositoryException`, `SqlDomainException`, `UnitOfWorkException` - Error handling
- `DbDataReaderMapper` - Data reader mapping utilities
- `DataTableNameMapper` - Table name helpers

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
