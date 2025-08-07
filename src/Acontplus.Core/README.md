# Acontplus.Core

[![NuGet](https://img.shields.io/nuget/v/Acontplus.Core.svg)](https://www.nuget.org/packages/Acontplus.Core)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

A cutting-edge .NET 9+ foundational library leveraging the latest C# language features and modern enterprise patterns. Built with performance, type safety, and developer experience in mind.

## 🚀 .NET 9 Modern Features

### 🎯 Latest C# Language Features
- **Collection Expressions** - Modern `[]` syntax for efficient collection initialization
- **Primary Constructors** - Concise record and class definitions
- **Required Properties** - Compile-time null safety with `required` keyword
- **Pattern Matching** - Advanced `switch` expressions and `is` patterns
- **Record Structs** - High-performance value types for DTOs and results
- **Nullable Reference Types** - Full compile-time null safety
- **Source Generators** - JSON serialization with AOT compilation support
- **Global Usings** - Clean namespace management with global using directives

### 🏗️ Modern Architecture Patterns
- **Domain-Driven Design (DDD)** - Complete DDD implementation with modern C# features
- **Functional Result Pattern** - Railway-oriented programming with record structs
- **Repository Pattern** - Comprehensive data access with bulk operations
- **Specification Pattern** - Type-safe query composition with expressions
- **Event Sourcing Ready** - Domain events with modern event patterns

### 📊 Advanced Data Patterns
- **Async Streaming** - `IAsyncEnumerable<T>` for memory-efficient processing
- **Projections** - Expression-based data transfer for performance
- **Bulk Operations** - High-performance batch processing with EF Core 9
- **Smart Pagination** - Advanced pagination with search and filtering
- **Modern JSON** - System.Text.Json with source generation

## 🔥 Cutting-Edge Features

### 🎯 Modern Entity System

```csharp
// Modern entity with required properties and collection expressions
public class Product : BaseEntity
{
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public string? Description { get; set; }
    public required int CategoryId { get; set; }
    
    // Factory method with modern syntax
    public static Product Create(int id, string name, decimal price, string description, int categoryId, int createdByUserId) =>
        new()
        {
            Id = id,
            Name = name,
            Price = price,
            Description = description,
            CategoryId = categoryId,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = createdByUserId
        };
}
```

### 🔄 Functional Result Pattern

```csharp
// Modern result pattern with record structs
public async Task<Result<Product>> GetProductAsync(int id)
{
    var product = await _repository.GetByIdAsync(id);
    
    return product is not null 
        ? Result<Product>.Success(product)
        : Result<Product>.Failure(DomainError.NotFound("PRODUCT_NOT_FOUND", $"Product {id} not found"));
}

// Pattern matching with modern syntax
var response = result.Match(
    success: product => ApiResponse<Product>.Success(product),
    failure: error => error.ToApiResponse<Product>()
);
```

### 📊 Modern Pagination

```csharp
// Collection expressions and modern initialization
var pagination = new PaginationDto
{
    PageIndex = 1,
    PageSize = 20,
    SortBy = "CreatedAt",
    SortDirection = SortDirection.Descending,
    SearchTerm = "search term",
    Filters = new Dictionary<string, object>
    {
        ["CategoryId"] = 1,
        ["IsActive"] = true
    }
};

// Modern specification pattern
public class ActiveProductsSpecification : BaseSpecification<Product>
{
    public ActiveProductsSpecification(PaginationDto pagination) : base(p => p.IsActive)
    {
        ApplyPaging(pagination);
        AddOrderBy(p => p.CreatedAt, isDescending: true);
        AddInclude(p => p.Category);
    }
}
```

### 🚀 Async Streaming & Projections

```csharp
// Memory-efficient async streaming
await foreach (var product in repository.FindAsyncEnumerable(p => p.IsActive))
{
    await ProcessProductAsync(product);
}

// High-performance projections
var summaries = await repository.GetPagedProjectionAsync(
    pagination,
    p => new ProductSummary(p.Id, p.Name, p.Price),
    p => p.IsActive
);
```

### 🔥 Modern JSON Extensions

```csharp
// Enterprise-optimized JSON with source generation
var json = myObject.SerializeModern();
var obj = jsonString.DeserializeModern<MyType>();
var clone = myObject.CloneViaJson();

// Modern JSON options for different scenarios
var options = JsonExtensions.DefaultOptions; // Production
var prettyOptions = JsonExtensions.PrettyOptions; // Development
var strictOptions = JsonExtensions.StrictOptions; // APIs
```

### 🎯 Modern Error Handling

```csharp
// Modern error creation with record structs
var validationError = DomainError.Validation(
    code: "PRODUCT_INVALID_PRICE",
    message: "Product price must be greater than zero",
    target: "price"
);

// Error aggregation with modern LINQ
var errors = new List<DomainError>();
if (string.IsNullOrWhiteSpace(request.Name))
    errors.Add(DomainError.Validation("INVALID_NAME", "Product name is required"));

if (errors.Any())
    return Result<Product>.Failure(errors.GetMostSevereError());
```

## 📦 Installation

### NuGet Package Manager
```bash
Install-Package Acontplus.Core
```

### .NET CLI
```bash
dotnet add package Acontplus.Core
```

### PackageReference
```xml
<PackageReference Include="Acontplus.Core" Version="1.3.3" />
```

## 🎯 Quick Start with .NET 9

### 1. Modern Entity Definition

```csharp
// Modern entity with required properties
public class Product : BaseEntity
{
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public string? Description { get; set; }
    public required int CategoryId { get; set; }
    
    // Factory method with modern syntax
    public static Product Create(int id, string name, decimal price, string description, int categoryId, int createdByUserId) =>
        new()
        {
            Id = id,
            Name = name,
            Price = price,
            Description = description,
            CategoryId = categoryId,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = createdByUserId
        };
}
```

### 2. Modern Error Handling

```csharp
// Modern error creation with record structs
var validationError = DomainError.Validation(
    code: "PRODUCT_INVALID_PRICE",
    message: "Product price must be greater than zero",
    target: "price"
);

// Pattern matching with modern syntax
var response = result.Match(
    success: product => ApiResponse<Product>.Success(product),
    failure: error => error.ToApiResponse<Product>()
);
```

### 3. Modern Repository Pattern

```csharp
// Modern repository interface
public interface IProductRepository : IRepository<Product>
{
    Task<IReadOnlyList<Product>> GetByCategoryAsync(int categoryId);
    Task<bool> ExistsByNameAsync(string name);
}

// Modern repository implementation
public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(DbContext context) : base(context) { }
    
    public async Task<IReadOnlyList<Product>> GetByCategoryAsync(int categoryId) =>
        await FindAsync(p => p.CategoryId == categoryId);
    
    public async Task<bool> ExistsByNameAsync(string name) =>
        await ExistsAsync(p => p.Name == name);
}
```

### 4. Modern Specification Pattern

```csharp
// Modern specification with primary constructor
public class ActiveProductsSpecification : BaseSpecification<Product>
{
    public ActiveProductsSpecification(PaginationDto pagination) : base(p => p.IsActive)
    {
        ApplyPaging(pagination);
        AddOrderBy(p => p.CreatedAt, isDescending: true);
        AddInclude(p => p.Category);
    }
}

// Usage with modern syntax
var spec = new ActiveProductsSpecification(new PaginationDto { PageIndex = 1, PageSize = 10 });
var products = await _repository.FindWithSpecificationAsync(spec);
```

### 5. Modern API Response Handling

```csharp
[HttpGet("{id}")]
public async Task<ApiResponse<ProductDto>> GetProduct(int id)
{
    var result = await _productService.GetByIdAsync(id);
    
    return result.Match(
        success: product => ApiResponse<ProductDto>.Success(product.ToDto()),
        failure: error => error.ToApiResponse<ProductDto>()
    );
}
```

## 🔧 Advanced .NET 9 Features

### Modern Domain Events

```csharp
// Modern domain event with record
public record ProductCreatedEvent(int ProductId, string ProductName, DateTime OccurredOn) : IDomainEvent
{
    public ProductCreatedEvent(int productId, string productName) : this(productId, productName, DateTime.UtcNow) { }
}

// Modern entity with domain events
public class Product : BaseEntity
{
    public void MarkAsCreated() =>
        AddDomainEvent(new ProductCreatedEvent(Id, Name));
}
```

### Modern Bulk Operations

```csharp
// High-performance bulk operations
await repository.BulkInsertAsync(products);
await repository.BulkUpdateAsync(p => p.CategoryId == 1, p => p.Status, "Active");
await repository.BulkDeleteAsync(p => p.CreatedAt < DateTime.UtcNow.AddDays(-30));

// Modern projections for efficient data transfer
var summaries = await repository.GetPagedProjectionAsync(
    pagination,
    p => new ProductSummary(p.Id, p.Name, p.Price),
    p => p.IsActive
);
```

### Modern Async Streaming

```csharp
// Memory-efficient processing with async streaming
await foreach (var product in repository.FindAsyncEnumerable(p => p.IsActive))
{
    await ProcessProductAsync(product);
}
```

### Modern JSON Handling

```csharp
// Enterprise-optimized JSON with source generation
var json = myObject.SerializeModern();
var obj = jsonString.DeserializeModern<MyType>();
var clone = myObject.CloneViaJson();

// Modern JSON options for different scenarios
var options = JsonExtensions.DefaultOptions; // Production
var prettyOptions = JsonExtensions.PrettyOptions; // Development
var strictOptions = JsonExtensions.StrictOptions; // APIs
```

## 🏗️ Modern Architecture Patterns

### Domain-Driven Design
- **Modern Entities**: Record-based entities with primary constructors
- **Value Objects**: Immutable objects with modern C# features
- **Domain Events**: Record-based events with modern patterns
- **Specifications**: Type-safe query composition with expressions

### Repository Pattern
- **Generic Repository**: Type-safe data access with modern features
- **Specification Pattern**: Flexible query composition with modern syntax
- **Unit of Work**: Transaction management with modern patterns
- **Bulk Operations**: High-performance batch processing with EF Core 9

### Error Handling
- **Domain Errors**: Record struct-based error types with HTTP mapping
- **Result Pattern**: Functional error handling with modern syntax
- **API Responses**: Consistent error response format with modern features
- **Error Aggregation**: Multiple error handling with modern LINQ

## 🔍 Modern Validation Examples

```csharp
// Modern validation with pattern matching
var validationResult = input switch
{
    { Length: 0 } => DomainError.Validation("EMPTY_INPUT", "Input cannot be empty"),
    { Length: > 100 } => DomainError.Validation("TOO_LONG", "Input too long"),
    _ => null
};

// Modern JSON validation
if (!DataValidation.IsValidJson(jsonContent))
{
    return DomainError.Validation("INVALID_JSON", "Invalid JSON format");
}
```

## 📚 Modern API Documentation

### Entity Base Classes
- `Entity<TId>` - Modern base entity with domain events
- `BaseEntity` - Modern base for int-keyed entities with audit trail
- `IAuditableEntity` - Interface for audit trail support
- `IEntityWithDomainEvents` - Interface for domain event support

### DTOs
- `ApiResponse<T>` - Modern API response format with record structs
- `PaginationDto` - Modern pagination with search and filtering
- `PagedResult<T>` - Modern paginated results with metadata
- `ApiError` - Modern error information with record structs

### Error Handling
- `DomainError` - Modern business logic errors with HTTP mapping
- `Result<T>` - Modern functional result pattern with record structs
- `ErrorType` - Modern error type categorization
- `ErrorTypeExtensions` - Modern error type utilities

### Repository
- `IRepository<TEntity>` - Modern comprehensive repository interface
- `ISpecification<T>` - Modern query specification pattern
- `BaseSpecification<T>` - Modern base class for specifications
- `PagedResult<T>` - Modern paginated query results

### Modern Features
- **Collection Expressions**: `[]` syntax for modern collection initialization
- **Required Properties**: Compile-time null safety with `required` keyword
- **Record Structs**: High-performance value types for DTOs and results
- **Primary Constructors**: Concise type definitions with modern syntax
- **Pattern Matching**: Advanced type-safe operations with modern patterns
- **Async Streaming**: Memory-efficient data processing with `IAsyncEnumerable<T>`
- **Source Generation**: JSON serialization with AOT compilation support

## 🆕 What's New in Version 1.3.3

### .NET 9 Modern Features
- **Primary Constructors**: Concise record and class definitions
- **Collection Expressions**: Modern `[]` syntax for efficient initialization
- **Required Properties**: Compile-time null safety with `required` keyword
- **Record Structs**: High-performance value types for DTOs and results
- **Pattern Matching**: Advanced `switch` expressions and `is` patterns
- **Source Generators**: JSON serialization with AOT compilation support
- **Global Usings**: Clean namespace management with global using directives

### Performance Improvements
- **Source Generation**: JSON serialization with source generation for AOT
- **Record Structs**: High-performance value types for DTOs and results
- **Collection Expressions**: Modern collection initialization with better performance
- **Async Streaming**: Memory-efficient data processing with `IAsyncEnumerable<T>`
- **Bulk Operations**: Optimized batch processing with EF Core 9

### Developer Experience
- **Modern C# Features**: Latest .NET 9+ language capabilities
- **Type Safety**: Enhanced compile-time safety with modern features
- **Better Documentation**: Comprehensive examples with modern syntax
- **Error Handling**: Improved error aggregation with modern patterns

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

**[Acontplus S.A.S.](https://acontplus.com.ec)** - Enterprise software solutions

---

**Built with ❤️ for the .NET community using the latest .NET 9 features**
