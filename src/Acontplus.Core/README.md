# Acontplus.Core

[![NuGet](https://img.shields.io/nuget/v/Acontplus.Core.svg)](https://www.nuget.org/packages/Acontplus.Core)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

A modern .NET 9+ foundational library providing enterprise-grade components with domain-driven design patterns, comprehensive error handling, and modern C# features.

## 🚀 Features

### 🏗️ Core Architecture
- **Domain-Driven Design (DDD)** - Complete DDD implementation with entities, value objects, and domain events
- **Modern Entity System** - Base entities with audit trails, soft deletes, and domain event support
- **Specification Pattern** - Flexible query specifications for complex business rules
- **Result Pattern** - Functional error handling with modern .NET 9+ features

### 📊 Data Transfer Objects (DTOs)
- **Structured API Responses** - Consistent API response format with status, errors, and metadata
- **Pagination Support** - Built-in pagination DTOs for efficient data retrieval
- **Request/Response Models** - Type-safe request and response models

### 🔍 Validation & Error Handling
- **Domain Errors** - Comprehensive error types with HTTP status code mapping
- **Data Validation** - Common validation utilities for XML, JSON, and data formats
- **Error Extensions** - Fluent error handling with severity-based error aggregation

### 🎯 Modern .NET 9+ Features
- **Required Properties** - Compile-time null safety with required properties
- **Collection Expressions** - Modern collection initialization with `[]` syntax
- **Pattern Matching** - Advanced pattern matching for type-safe operations
- **Source Generators** - JSON serialization with source generation for performance
- **Nullable Reference Types** - Full nullable reference type support

### 🆕 Modern JSON Extensions

Acontplus.Core provides high-performance, secure JSON utilities via `Extensions/JsonExtensions`:

- **Enterprise-optimized serialization**: Consistent, camelCase, safe, and fast.
- **Multiple options**: Default, Pretty (for debugging), and Strict (for APIs).
- **Extension methods**: For easy serialization, deserialization, and deep cloning.

#### Usage Examples

```csharp
using Acontplus.Core.Extensions;

// Serialize with enterprise defaults
var json = myObject.SerializeModern();

// Serialize with pretty formatting (for debugging)
var prettyJson = myObject.SerializeModern(pretty: true);

// Deserialize with enterprise defaults
var obj = jsonString.DeserializeModern<MyType>();

// Safe deserialization with fallback
var objOrDefault = jsonString.DeserializeModernSafe<MyType>(fallback: new MyType());

// Deep clone via JSON
var clone = myObject.CloneViaJson();

// Use options directly (for ASP.NET Core, etc.)
var options = JsonExtensions.DefaultOptions;
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
<PackageReference Include="Acontplus.Core" Version="1.2.0" />
```

## 🎯 Quick Start

### 1. Basic Entity Usage

```csharp
public class Product : AuditableEntity<int>
{
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public string? Description { get; set; }
    
    // Factory method for creating products
    public static Product Create(int id, string name, decimal price, int createdByUserId)
    {
        return new Product
        {
            Id = id,
            Name = name,
            Price = price,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = createdByUserId
        };
    }
}
```

### 2. Domain Error Handling

```csharp
// Create domain errors
var validationError = DomainError.Validation(
    code: "PRODUCT_INVALID_PRICE",
    message: "Product price must be greater than zero",
    target: "price"
);

var notFoundError = DomainError.NotFound(
    code: "PRODUCT_NOT_FOUND",
    message: "Product with ID {id} was not found",
    target: "id"
);

// Convert to API response
var response = validationError.ToApiResponse<ProductDto>();
```

### 3. Result Pattern Usage

```csharp
public async Task<Result<Product>> GetProductAsync(int id)
{
    var product = await _repository.GetByIdAsync(id);
    
    return product is not null 
        ? Result<Product>.Success(product)
        : Result<Product>.Failure(DomainError.NotFound("PRODUCT_NOT_FOUND", $"Product {id} not found"));
}

// Usage with pattern matching
var result = await GetProductAsync(123);
var response = result.Match(
    success: product => ApiResponse<Product>.Success(product),
    failure: error => error.ToApiResponse<Product>()
);
```

### 4. Specification Pattern

```csharp
public class ActiveProductsSpecification : BaseSpecification<Product>
{
    public ActiveProductsSpecification(PaginationDto pagination) : base(p => p.IsActive)
    {
        ApplyPaging(pagination);
        AddOrderBy(p => p.CreatedAt, isDescending: true);
        AddInclude(p => p.Category);
    }
}

// Usage
var spec = new ActiveProductsSpecification(new PaginationDto { Page = 1, PageSize = 10 });
var products = await _repository.FindWithSpecificationAsync(spec);
```

### 5. API Response Handling

```csharp
[HttpGet("{id}")]
public async Task<ApiResponse<ProductDto>> GetProduct(int id)
{
    var product = await _productService.GetByIdAsync(id);
    
    if (product is null)
    {
        return ApiResponse<ProductDto>.Failure(
            DomainError.NotFound("PRODUCT_NOT_FOUND", $"Product {id} not found")
        );
    }
    
    return ApiResponse<ProductDto>.Success(product.ToDto());
}
```

## 🔧 Advanced Usage

### Domain Events

```csharp
public class ProductCreatedEvent : IDomainEvent
{
    public int ProductId { get; }
    public string ProductName { get; }
    public DateTime OccurredOn { get; }
    
    public ProductCreatedEvent(int productId, string productName)
    {
        ProductId = productId;
        ProductName = productName;
        OccurredOn = DateTime.UtcNow;
    }
}

// In your entity
public void MarkAsCreated()
{
    AddDomainEvent(new ProductCreatedEvent(Id, Name));
}
```

### Repository Pattern

```csharp
public interface IProductRepository : IRepository<Product, int>
{
    Task<IReadOnlyList<Product>> GetByCategoryAsync(int categoryId);
    Task<bool> ExistsByNameAsync(string name);
}

public class ProductRepository : BaseRepository<Product, int>, IProductRepository
{
    public ProductRepository(DbContext context) : base(context) { }
    
    public async Task<IReadOnlyList<Product>> GetByCategoryAsync(int categoryId)
    {
        return await FindAsync(p => p.CategoryId == categoryId);
    }
    
    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await ExistsAsync(p => p.Name == name);
    }
}
```

### Error Aggregation

```csharp
public async Task<Result<Product>> CreateProductAsync(CreateProductRequest request)
{
    var errors = new List<DomainError>();
    
    if (string.IsNullOrWhiteSpace(request.Name))
        errors.Add(DomainError.Validation("INVALID_NAME", "Product name is required"));
    
    if (request.Price <= 0)
        errors.Add(DomainError.Validation("INVALID_PRICE", "Price must be greater than zero"));
    
    if (await _repository.ExistsByNameAsync(request.Name))
        errors.Add(DomainError.Conflict("DUPLICATE_NAME", "Product name already exists"));
    
    if (errors.Any())
        return Result<Product>.Failure(errors.GetMostSevereError());
    
    var product = Product.Create(request.Name, request.Price, request.CreatedByUserId);
    await _repository.AddAsync(product);
    
    return Result<Product>.Success(product);
}
```

## 🧑‍💻 Example: Result & API Response Patterns with Usuario Module

### Controller Usage

```csharp
[HttpGet("{id:int}")]
public async Task<IActionResult> GetUsuario(int id)
{
    var result = await _usuarioService.GetByIdAsync(id);
    return result.ToGetActionResult();
}

[HttpPost]
public async Task<IActionResult> CreateUsuario([FromBody] UsuarioDto dto)
{
    var usuario = ObjectMapper.Map<UsuarioDto, Usuario>(dto);
    var result = await _usuarioService.AddAsync(usuario);
    if (result.IsSuccess && result.Value is not null)
    {
        var locationUri = $"/api/Usuario/{result.Value.Id}";
        return ApiResponse<Usuario>.Success(result.Value, new ApiResponseOptions { Message = "Usuario creado exitosamente." }).ToActionResult();
    }
    return result.ToActionResult();
}

[HttpPut("{id:int}")]
public async Task<IActionResult> UpdateUsuario(int id, [FromBody] UsuarioDto dto)
{
    var usuario = ObjectMapper.Map<UsuarioDto, Usuario>(dto);
    var result = await _usuarioService.UpdateAsync(id, usuario);
    if (result.IsSuccess)
    {
        return ApiResponse<Usuario>.Success(result.Value, new ApiResponseOptions { Message = "Usuario actualizado correctamente." }).ToActionResult();
    }
    return result.ToActionResult();
}

[HttpDelete("{id:int}")]
public async Task<IActionResult> DeleteUsuario(int id)
{
    var result = await _usuarioService.DeleteAsync(id);
    return result.ToDeleteActionResult();
}
```

### Minimal API Usage

```csharp
app.MapGet("/usuarios/{id:int}", async (int id, IUsuarioService service) =>
{
    var result = await service.GetByIdAsync(id);
    return result.ToMinimalApiResult();
});
```

### Service Layer Example

```csharp
public async Task<Result<Usuario, DomainErrors>> AddAsync(Usuario usuario)
{
    var errors = new List<DomainError>();
    if (string.IsNullOrWhiteSpace(usuario.Username))
        errors.Add(DomainError.Validation("USERNAME_REQUIRED", "Username is required"));
    if (string.IsNullOrWhiteSpace(usuario.Email))
        errors.Add(DomainError.Validation("EMAIL_REQUIRED", "Email is required"));

    if (errors.Count > 0)
        return DomainErrors.Multiple(errors);

    // ... check for existing user, add, etc.
}
```

## 🏗️ Architecture Patterns

### Domain-Driven Design
- **Entities**: Rich domain objects with identity and lifecycle
- **Value Objects**: Immutable objects representing concepts
- **Domain Events**: Decoupled communication between aggregates
- **Specifications**: Encapsulated business rules for queries

### Repository Pattern
- **Generic Repository**: Type-safe data access with common operations
- **Specification Pattern**: Flexible query composition
- **Unit of Work**: Transaction management and consistency

### Error Handling
- **Domain Errors**: Business logic errors with proper categorization
- **Result Pattern**: Functional error handling without exceptions
- **API Responses**: Consistent error response format

## 🔍 Validation Examples

```csharp
// XML Validation
if (!DataValidation.IsXml(xmlContent))
{
    return DomainError.Validation("INVALID_XML", "Invalid XML format");
}

// JSON Validation
if (!DataValidation.IsValidJson(jsonContent))
{
    return DomainError.Validation("INVALID_JSON", "Invalid JSON format");
}

// IP Address Validation
var validIp = DataValidation.ValidateIpAddress("192.168.1.1");
```

## 📚 API Documentation

### Entity Base Classes
- `Entity<TId>` - Base entity with domain events
- `AuditableEntity<TId>` - Entity with audit trail and soft delete
- `BaseEntity` - Convenience base for int-keyed entities

### DTOs
- `ApiResponse<T>` - Standardized API response format
- `PaginationDto` - Pagination parameters
- `ApiError` - Detailed error information

### Error Handling
- `DomainError` - Business logic errors
- `Result<T>` - Functional result pattern
- `ErrorType` - Categorized error types

### Repository
- `IRepository<TEntity, TId>` - Generic repository interface
- `ISpecification<T>` - Query specification pattern
- `PagedResult<T>` - Paginated query results

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

**Built with ❤️ for the .NET community**