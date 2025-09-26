# Acontplus.Persistence.PostgreSQL

[![NuGet](https://img.shields.io/nuget/v/Acontplus.Persistence.PostgreSQL.svg)](https://www.nuget.org/packages/Acontplus.Persistence.PostgreSQL)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

PostgreSQL implementation of the Acontplus persistence layer. Provides optimized Entity Framework Core integration, ADO.NET repositories, and PostgreSQL-specific features for high-performance data access.

> **Note:** This package implements the abstractions defined in [**Acontplus.Persistence.Common**](https://www.nuget.org/packages/Acontplus.Persistence.Common). For general persistence patterns and repository interfaces, see the common package.

## 🚀 PostgreSQL-Specific Features

- **PostgreSQL Optimization** - Query optimizations and connection pooling for PostgreSQL
- **Advanced Error Translation** - PostgreSQL error code mapping to domain exceptions
- **JSON/JSONB Support** - Native JSON operations and indexing
- **Array Types** - PostgreSQL array type handling and operations
- **Full-Text Search** - PostgreSQL full-text search integration
- **Performance Monitoring** - Query execution statistics and performance insights

## 📦 Installation

### NuGet Package Manager
```bash
Install-Package Acontplus.Persistence.PostgreSQL
```

### .NET CLI
```bash
dotnet add package Acontplus.Persistence.PostgreSQL
```

### PackageReference
```xml
<ItemGroup>
  <PackageReference Include="Acontplus.Persistence.PostgreSQL" Version="1.0.10" />
  <PackageReference Include="Acontplus.Persistence.Common" Version="1.1.13" />
</ItemGroup>
```

## 🎯 Quick Start

### 1. Configure PostgreSQL Context
```csharp
services.AddDbContext<BaseContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(30), null);
        npgsqlOptions.CommandTimeout(60);
    }));

services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
```

### 2. Use Repository Pattern
```csharp
public class UserService
{
    private readonly IRepository<User> _userRepository;

    public UserService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<User>> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user != null
            ? Result<User>.Success(user)
            : Result<User>.Failure(DomainError.NotFound("USER_NOT_FOUND", $"User {id} not found"));
    }
}
```

### 3. Advanced Query Operations
```csharp
// Complex queries with PostgreSQL optimizations
public async Task<IReadOnlyList<OrderSummary>> GetOrderSummariesAsync(
    DateTime startDate,
    CancellationToken ct = default)
{
    var queryExpression = (IQueryable<Order> q) => q
        .Where(o => o.CreatedAt >= startDate)
        .Join(_context.Set<Customer>(),
            order => order.CustomerId,
            customer => customer.Id,
            (order, customer) => new { Order = order, Customer = customer })
        .Select(x => new OrderSummary
        {
            OrderId = x.Order.Id,
            CustomerName = $"{x.Customer.FirstName} {x.Customer.LastName}",
            TotalAmount = x.Order.TotalAmount,
            Status = x.Order.Status
        });

    return await _orderRepository.ExecuteQueryToListAsync(queryExpression, ct);
}
```

## 🔧 PostgreSQL Configuration

### Connection String Best Practices
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=MyApp;Username=myuser;Password=mypass;SSL Mode=Require;Trust Server Certificate=true;"
  }
}
```

### Performance Tuning
```csharp
services.AddDbContext<BaseContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        // Connection resilience
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);

        // Performance settings
        npgsqlOptions.CommandTimeout(60);
        npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });

    // Additional performance options
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    options.EnableSensitiveDataLogging(false);
});
```

## 📚 PostgreSQL API Reference

- `BaseContext` - Optimized EF Core context for PostgreSQL
- `AdoRepository` - ADO.NET operations with PostgreSQL-specific error handling
- `PostgresExceptionTranslator` - Maps PostgreSQL error codes to domain exceptions
- `JsonOperations` - JSON/JSONB query and manipulation utilities
- `FullTextSearch` - PostgreSQL full-text search integration

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
