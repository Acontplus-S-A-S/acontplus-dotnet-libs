# Acontplus.Persistence.SqlServer

[![NuGet](https://img.shields.io/nuget/v/Acontplus.Persistence.SqlServer.svg)](https://www.nuget.org/packages/Acontplus.Persistence.SqlServer)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

SQL Server implementation of the Acontplus persistence layer. Provides optimized Entity Framework Core integration, ADO.NET repositories, and SQL Server-specific features for high-performance data access.

> **Note:** This package implements the abstractions defined in [**Acontplus.Persistence.Common**](https://www.nuget.org/packages/Acontplus.Persistence.Common). For general persistence patterns and repository interfaces, see the common package.

## 🚀 SQL Server-Specific Features

- **SQL Server Optimization** - Query optimizations and connection pooling for SQL Server
- **Advanced Error Translation** - SQL Server error code mapping to domain exceptions
- **Transaction Management** - Distributed transactions and savepoints support
- **Bulk Operations** - SQL Server bulk insert/update capabilities
- **Performance Monitoring** - Query execution statistics and performance insights

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
  <PackageReference Include="Acontplus.Persistence.SqlServer" Version="1.5.12" />
  <PackageReference Include="Acontplus.Persistence.Common" Version="1.1.13" />
</ItemGroup>
```

## 🎯 Quick Start

### 1. Configure SQL Server Context
```csharp
services.AddDbContext<BaseContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(30), null);
        sqlOptions.CommandTimeout(60);
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
// Complex queries with SQL Server optimizations
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

## 🔧 SQL Server Configuration

### Connection String Best Practices
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MyApp;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=true;TrustServerCertificate=false;"
  }
}
```

### Performance Tuning
```csharp
services.AddDbContext<BaseContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        // Connection resilience
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);

        // Performance settings
        sqlOptions.CommandTimeout(60);
        sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });

    // Additional performance options
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    options.EnableSensitiveDataLogging(false);
});
```

## 📚 SQL Server API Reference

- `BaseContext` - Optimized EF Core context for SQL Server
- `AdoRepository` - ADO.NET operations with SQL Server-specific error handling
- `SqlExceptionTranslator` - Maps SQL Server error codes to domain exceptions
- `BulkOperations` - High-performance bulk insert/update operations
- `QueryOptimizer` - SQL Server query optimization utilities

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

**[Acontplus](https://www.acontplus.com)** - Software solutions

---

**Built with ❤️ for the .NET community**
