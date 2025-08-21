# Acontplus.Persistence.Common

[![NuGet](https://img.shields.io/nuget/v/Acontplus.Persistence.Common.svg)](https://www.nuget.org/packages/Acontplus.Persistence.Common)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

Advanced persistence abstractions and infrastructure. Includes generic repository patterns, context factory, connection string providers, and multi-provider support for SQL Server, PostgreSQL, and other databases with business-ready abstractions.

## 🚀 Features

### 🏗️ Core Abstractions
- **Generic Repository Pattern** - Type-safe data access with C# features
- **Context Factory** - Flexible database context creation and management
- **Connection String Provider** - Hierarchical and environment-based connection management
- **Multi-Provider Support** - SQL Server, PostgreSQL, and extensible for other databases
- **Business Patterns** - Unit of work, specification pattern, and audit trail support

### 🔧 Contemporary Architecture
- **.NET 9+ Compatible** - Latest C# features and performance optimizations
- **Async/Await Support** - Full asynchronous operation support
- **Dependency Injection** - Seamless integration with Microsoft DI container
- **Configuration Driven** - Flexible configuration through appsettings.json
- **Error Handling** - Comprehensive error handling with domain mapping

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
<PackageReference Include="Acontplus.Persistence.Common" Version="1.1.0" />
```

## 🎯 Quick Start

### 1. Register Services in DI

```csharp
using Acontplus.Persistence.Common;

// In Program.cs or Startup.cs
builder.Services.AddSingleton<IConnectionStringProvider, ConfigurationConnectionStringProvider>();
builder.Services.AddSingleton<IDbContextFactory<MyDbContext>, DbContextFactory<MyDbContext>>();
```

### 2. Configure Connection Strings

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MyApp;Trusted_Connection=true;",
    "TenantA": "Server=localhost;Database=TenantA;Trusted_Connection=true;",
    "TenantB": "Server=localhost;Database=TenantB;Trusted_Connection=true;"
  }
}
```

### 3. Use in Your Application

```csharp
public class ProductService
{
    private readonly IDbContextFactory<MyDbContext> _contextFactory;
    private readonly IConnectionStringProvider _connectionProvider;

    public ProductService(
        IDbContextFactory<MyDbContext> contextFactory,
        IConnectionStringProvider connectionProvider)
    {
        _contextFactory = contextFactory;
        _connectionProvider = connectionProvider;
    }

    public async Task<IEnumerable<Product>> GetProductsAsync(string tenantId = null)
    {
        var context = _contextFactory.GetContext(tenantId);
        var repository = new BaseRepository<Product>(context);
        
        return await repository.FindAsync(p => p.IsActive);
    }
}
```

## 🔧 Advanced Usage

### Multi-Tenant Database Access

```csharp
public class MultiTenantService
{
    private readonly IDbContextFactory<MyDbContext> _contextFactory;

    public MultiTenantService(IDbContextFactory<MyDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<Product> GetProductAsync(int productId, string tenantId)
    {
        // Get context for specific tenant
        var context = _contextFactory.GetContext(tenantId);
        var repository = new BaseRepository<Product>(context);
        
        return await repository.GetByIdAsync(productId);
    }

    public async Task<IEnumerable<Product>> GetProductsForAllTenantsAsync()
    {
        var allProducts = new List<Product>();
        var tenants = new[] { "TenantA", "TenantB", "TenantC" };

        foreach (var tenant in tenants)
        {
            var context = _contextFactory.GetContext(tenant);
            var repository = new BaseRepository<Product>(context);
            var products = await repository.FindAsync(p => p.IsActive);
            allProducts.AddRange(products);
        }

        return allProducts;
    }
}
```

### Custom Connection String Provider

```csharp
public class CustomConnectionStringProvider : IConnectionStringProvider
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomConnectionStringProvider(
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetConnectionString(string name)
    {
        // Get tenant from HTTP context
        var tenant = _httpContextAccessor.HttpContext?.User?.FindFirst("TenantId")?.Value;
        
        if (!string.IsNullOrEmpty(tenant))
        {
            return _configuration.GetConnectionString($"{name}_{tenant}");
        }

        return _configuration.GetConnectionString(name);
    }
}
```

### Repository with Specifications

```csharp
public class ProductRepository : BaseRepository<Product>
{
    public ProductRepository(DbContext context) : base(context) { }

    public async Task<IEnumerable<Product>> GetActiveProductsByCategoryAsync(int categoryId)
    {
        var spec = new ActiveProductsByCategorySpecification(categoryId);
        return await FindWithSpecificationAsync(spec);
    }

    public async Task<PagedResult<Product>> GetPagedProductsAsync(PaginationDto pagination)
    {
        var spec = new ProductPaginationSpecification(pagination);
        return await GetPagedAsync(spec);
    }
}

public class ActiveProductsByCategorySpecification : BaseSpecification<Product>
{
    public ActiveProductsByCategorySpecification(int categoryId) 
        : base(p => p.IsActive && p.CategoryId == categoryId)
    {
        AddInclude(p => p.Category);
        AddOrderBy(p => p.Name);
    }
}
```

## 📊 Configuration Options

### Connection String Provider Options

```csharp
public class ConnectionStringOptions
{
    public string DefaultConnection { get; set; } = string.Empty;
    public Dictionary<string, string> TenantConnections { get; set; } = new();
    public bool UseEnvironmentVariables { get; set; } = true;
    public string EnvironmentPrefix { get; set; } = "DB_";
}
```

### Context Factory Options

```csharp
public class ContextFactoryOptions
{
    public bool EnableRetryOnFailure { get; set; } = true;
    public int MaxRetryCount { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(5);
    public bool EnableDetailedErrors { get; set; } = false;
    public bool EnableSensitiveDataLogging { get; set; } = false;
}
```

## 🔍 Best Practices

### 1. Connection String Management

```csharp
// Use hierarchical configuration
var connectionString = _connectionProvider.GetConnectionString("DefaultConnection");

// Handle missing connections gracefully
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found");
}
```

### 2. Context Lifecycle Management

```csharp
public class UnitOfWork : IDisposable
{
    private readonly DbContext _context;
    private readonly BaseRepository<Product> _productRepository;
    private readonly BaseRepository<Category> _categoryRepository;

    public UnitOfWork(IDbContextFactory<MyDbContext> contextFactory, string tenantId = null)
    {
        _context = contextFactory.GetContext(tenantId);
        _productRepository = new BaseRepository<Product>(_context);
        _categoryRepository = new BaseRepository<Category>(_context);
    }

    public BaseRepository<Product> Products => _productRepository;
    public BaseRepository<Category> Categories => _categoryRepository;

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
```

### 3. Error Handling

```csharp
public async Task<Result<Product>> GetProductSafelyAsync(int id)
{
    try
    {
        var context = _contextFactory.GetContext();
        var repository = new BaseRepository<Product>(context);
        var product = await repository.GetByIdAsync(id);
        
        return product is not null 
            ? Result<Product>.Success(product)
            : Result<Product>.Failure(DomainError.NotFound("PRODUCT_NOT_FOUND", $"Product {id} not found"));
    }
    catch (Exception ex)
    {
        return Result<Product>.Failure(DomainError.Internal("DATABASE_ERROR", ex.Message));
    }
}
```

## 📚 API Reference

### IConnectionStringProvider

```csharp
public interface IConnectionStringProvider
{
    string GetConnectionString(string name);
}
```

### IDbContextFactory<T>

```csharp
public interface IDbContextFactory<T> where T : DbContext
{
    T GetContext(string tenantId = null);
    T GetContext(IConnectionStringProvider connectionProvider, string connectionName);
}
```

### BaseRepository<T>

```csharp
public class BaseRepository<T> : IRepository<T> where T : class
{
    public BaseRepository(DbContext context);
    
    // Async operations
    public Task<T?> GetByIdAsync(int id);
    public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    public Task<T> AddAsync(T entity);
    public Task UpdateAsync(T entity);
    public Task DeleteAsync(T entity);
    
    // Specification pattern
    public Task<IEnumerable<T>> FindWithSpecificationAsync(ISpecification<T> spec);
    public Task<PagedResult<T>> GetPagedAsync(ISpecification<T> spec);
}
```

## 🔧 Dependencies

- **.NET 9.0+** - Advanced .NET framework
- **Entity Framework Core** - ORM and data access
- **Microsoft.Extensions.Configuration** - Configuration management
- **Acontplus.Core** - Core abstractions and patterns

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

**Built with ❤️ for the .NET community using cutting-edge .NET 9 features** 
