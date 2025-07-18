# Acontplus.Persistence.SqlServer Usage Guide

Acontplus.Persistence.SqlServer provides robust SQL Server persistence with ADO.NET and EF Core integration.

## 📦 Installation

```bash
Install-Package Acontplus.Persistence.SqlServer
```

## 🚀 Features
- Database context management (EF Core)
- Repository and unit-of-work patterns
- ADO.NET and micro-ORM support
- Advanced error handling

## 🛠️ Basic Usage

### Register DbContext and Repository
```csharp
services.AddDbContext<BaseContext>(options =>
    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
services.AddScoped(typeof(IRepository<,>), typeof(BaseRepository<,>));
```

### Use ADO.NET Repository
```csharp
var repo = serviceProvider.GetRequiredService<IAdoRepository>();
var result = await repo.ExecuteQueryAsync("SELECT * FROM MyTable");
```

## 📖 See Also
- [API Reference](../Home.md)
- [Project README](https://github.com/Acontplus-S-A-S/acontplus-dotnet-libs/blob/main/src/Acontplus.Persistence.SqlServer/README.md) 