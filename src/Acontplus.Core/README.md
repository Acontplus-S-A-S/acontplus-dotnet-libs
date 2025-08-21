# Acontplus.Core

[![NuGet](https://img.shields.io/nuget/v/Acontplus.Core.svg)](https://www.nuget.org/packages/Acontplus.Core)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

A cutting-edge .NET 9+ foundational library leveraging the latest C# language features and business patterns. Built with performance, type safety, and developer experience in mind.

## 🚀 .NET 9 Features

### 🎯 Latest C# Language Features
- **Collection Expressions** - `[]` syntax for efficient collection initialization
- **Primary Constructors** - Concise record and class definitions
- **Required Properties** - Compile-time null safety with `required` keyword
- **Pattern Matching** - Advanced `switch` expressions and `is` patterns
- **Record Structs** - High-performance value types for DTOs and results
- **Nullable Reference Types** - Full compile-time null safety
- **Source Generators** - JSON serialization with AOT compilation support
- **Global Usings** - Clean namespace management with global using directives

### 🏗️ Architecture Patterns
- **Domain-Driven Design (DDD)** - Complete DDD implementation with C# features
- **Functional Result Pattern** - Railway-oriented programming with record structs
- **Repository Pattern** - Comprehensive data access with bulk operations
- **Specification Pattern** - Type-safe query composition with expressions
- **Event Sourcing Ready** - Domain events with event patterns

### 📊 Advanced Data Patterns
- **Async Streaming** - `IAsyncEnumerable<T>` for memory-efficient processing
- **Projections** - Expression-based data transfer for performance
- **Bulk Operations** - High-performance batch processing with EF Core 9
- **Smart Pagination** - Advanced pagination with search and filtering
- **JSON Utilities** - System.Text.Json with source generation

## 🔥 Core Features

### 🎯 Entity System

```csharp
// Entity with required properties and collection expressions
public class Product : BaseEntity
{
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public string? Description { get; set; }
    public required int CategoryId { get; set; }
    
    // Factory method with syntax
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
// Result pattern with record structs
public async Task<Result<Product>> GetProductAsync(int id)
{
    var product = await _repository.GetByIdAsync(id);
    
    return product is not null 
        ? Result<Product>.Success(product)
        : Result<Product>.Failure(DomainError.NotFound("PRODUCT_NOT_FOUND", $"Product {id} not found"));
}

// Pattern matching with syntax
var response = result.Match(
    success: product => ApiResponse<Product>.Success(product),
    failure: error => error.ToApiResponse<Product>()
);
```

### 📊 Pagination

```csharp
// Collection expressions and initialization
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

// Specification pattern
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

### 🔥 JSON Extensions

```csharp
// Business-optimized JSON with source generation
var json = myObject.SerializeOptimized();
var obj = jsonString.DeserializeOptimized<MyType>();
var clone = myObject.CloneDeep();

// JSON options for different scenarios
var options = JsonExtensions.DefaultOptions; // Production
var prettyOptions = JsonExtensions.PrettyOptions; // Development
var strictOptions = JsonExtensions.StrictOptions; // APIs
```

### 🎯 Error Handling

```csharp
// Error creation with record structs
var validationError = DomainError.Validation(
    code: "PRODUCT_INVALID_PRICE",
    message: "Product price must be greater than zero",
    target: "price"
);

// Error aggregation with LINQ
var errors = new List<DomainError>();
if (string.IsNullOrWhiteSpace(request.Name))
    errors.Add(DomainError.Validation("INVALID_NAME", "Product name is required"));

if (errors.Any())
    return Result<Product>.Failure(errors.GetMostSevereError());
```

### 🔍 Validation Utilities

```csharp
// Data validation with pattern matching
var validationResult = input switch
{
    { Length: 0 } => DomainError.Validation("EMPTY_INPUT", "Input cannot be empty"),
    { Length: > 100 } => DomainError.Validation("TOO_LONG", "Input too long"),
    _ => null
};

// JSON validation
if (!DataValidation.IsValidJson(jsonContent))
{
    return DomainError.Validation("INVALID_JSON", "Invalid JSON format");
}

// XML validation
var xmlErrors = XmlValidator.Validate(xmlContent, xsdSchema);
if (xmlErrors.Any())
{
    return DomainError.Validation("INVALID_XML", "XML validation failed");
}
```

### 🧩 Extension Methods

```csharp
// Pagination extensions
var pagination = new PaginationDto()
    .WithSearch("laptop")
    .WithSort("Price", SortDirection.Asc)
    .WithFilters(new Dictionary<string, object>
    {
        ["categoryId"] = 1,
        ["isActive"] = true
    });

// Repository audit extensions
await repository.AddWithAuditAsync(entity, userId);
await repository.UpdateWithAuditAsync(entity, userId);

// JSON extensions
var json = myObject.SerializeOptimized();
var obj = jsonString.DeserializeOptimized<MyType>();

// Nullable extensions
var result = nullableValue.OrElse("default");
var safeValue = nullableValue.ThrowIfNull("Value is required");
```

### 📊 Constants & Metadata

```csharp
// API metadata keys for consistent responses
var metadata = new Dictionary<string, object>
{
    [ApiMetadataKeys.Page] = 1,
    [ApiMetadataKeys.PageSize] = 20,
    [ApiMetadataKeys.TotalItems] = 100,
    [ApiMetadataKeys.TotalPages] = 5
};

// API response helpers
var response = ApiResponseHelpers.CreateSuccessResponse(data, "Operation successful");
var errorResponse = ApiResponseHelpers.CreateErrorResponse("Operation failed", "ERROR_CODE");
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

### 1. Entity Definition

```csharp
// Entity with required properties
public class Product : BaseEntity
{
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public string? Description { get; set; }
    public required int CategoryId { get; set; }
    
    // Factory method with syntax
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

### 2. Error Handling

```csharp
// Error creation with record structs
var validationError = DomainError.Validation(
    code: "PRODUCT_INVALID_PRICE",
    message: "Product price must be greater than zero",
    target: "price"
);

// Pattern matching with syntax
var response = result.Match(
    success: product => ApiResponse<ProductDto>.Success(product.ToDto()),
    failure: error => error.ToApiResponse<ProductDto>()
);
```

### 3. Repository Pattern

```csharp
// Repository interface
public interface IProductRepository : IRepository<Product>
{
    Task<IReadOnlyList<Product>> GetByCategoryAsync(int categoryId);
    Task<bool> ExistsByNameAsync(string name);
}

// Repository implementation
public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(DbContext context) : base(context) { }
    
    public async Task<IReadOnlyList<Product>> GetByCategoryAsync(int categoryId) =>
        await FindAsync(p => p.CategoryId == categoryId);
    
    public async Task<bool> ExistsByNameAsync(string name) =>
        await ExistsAsync(p => p.Name == name);
}
```

### 4. Specification Pattern

```csharp
// Specification with primary constructor
public class ActiveProductsSpecification : BaseSpecification<Product>
{
    public ActiveProductsSpecification(PaginationDto pagination) : base(p => p.IsActive)
    {
        ApplyPaging(pagination);
        AddOrderBy(p => p.CreatedAt, isDescending: true);
        AddInclude(p => p.Category);
    }
}

// Usage with syntax
var spec = new ActiveProductsSpecification(new PaginationDto { PageIndex = 1, PageSize = 10 });
var products = await _repository.FindWithSpecificationAsync(spec);
```

### 5. API Response Handling

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

## 📄 Core Examples

### PaginationDto with Multiple Filters

The `PaginationDto` provides advanced pagination capabilities with search, sorting, and multiple filters for business applications.

#### Backend (Controller) with Stored Procedures

```csharp
[HttpGet]
public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts([FromQuery] PaginationDto pagination)
{
    var result = await _productService.GetPaginatedProductsAsync(pagination);
    return Ok(result);
}

// Service implementation
public async Task<PagedResult<ProductDto>> GetPaginatedProductsAsync(PaginationDto pagination)
{
    var spParameters = new Dictionary<string, object>
    {
        ["@PageIndex"] = pagination.PageIndex,
        ["@PageSize"] = pagination.PageSize,
        ["@SearchTerm"] = pagination.SearchTerm ?? (object)DBNull.Value,
        ["@SortBy"] = pagination.SortBy ?? "CreatedAt",
        ["@SortDirection"] = pagination.SortDirection.ToString()
    };

    // Add filters using the built-in methods
    var filterParams = pagination.BuildSqlParameters();
    if (filterParams != null)
    {
        foreach (var param in filterParams)
        {
            spParameters[param.Key] = param.Value;
        }
    }

    // Execute stored procedure with all parameters
    var dataSet = await _adoRepository.GetDataSetAsync("sp_GetPaginatedProducts", spParameters);
    
    // Process results and return PagedResult
    var products = ProcessStoredProcedureResults(dataSet);
    var totalCount = GetTotalCountFromDataSet(dataSet);
    
    return new PagedResult<ProductDto>(products, pagination.PageIndex, pagination.PageSize, totalCount);
}
```

#### Frontend (JavaScript/TypeScript)

```typescript
// API client function
async function getProducts(filters: ProductFilters = {}) {
    const params = new URLSearchParams({
        pageIndex: '1',
        pageSize: '20',
        searchTerm: filters.searchTerm || '',
        sortBy: 'createdAt',
        sortDirection: 'desc'
    });

    // Add filters
    if (filters.categoryId) params.append('filters[categoryId]', filters.categoryId.toString());
    if (filters.isActive !== undefined) params.append('filters[isActive]', filters.isActive.toString());
    if (filters.minPrice) params.append('filters[minPrice]', filters.minPrice.toString());
    if (filters.maxPrice) params.append('filters[maxPrice]', filters.maxPrice.toString());
    if (filters.createdDate) params.append('filters[createdDate]', filters.createdDate);

    const response = await fetch(`/api/products?${params.toString()}`);
    return response.json();
}

// Usage examples
const products = await getProducts({
    searchTerm: 'laptop',
    categoryId: 1,
    isActive: true,
    minPrice: 500,
    maxPrice: 2000
});

// URL generated: /api/products?pageIndex=1&pageSize=20&searchTerm=laptop&sortBy=createdAt&sortDirection=desc&filters[categoryId]=1&filters[isActive]=true&filters[minPrice]=500&filters[maxPrice]=2000
```

#### React Component Example

```tsx
import React, { useState, useEffect } from 'react';

interface ProductFilters {
    searchTerm?: string;
    categoryId?: number;
    isActive?: boolean;
    minPrice?: number;
    maxPrice?: number;
}

const ProductList: React.FC = () => {
    const [products, setProducts] = useState([]);
    const [filters, setFilters] = useState<ProductFilters>({});
    const [pagination, setPagination] = useState({ pageIndex: 1, pageSize: 20 });

    const fetchProducts = async () => {
        const params = new URLSearchParams({
            pageIndex: pagination.pageIndex.toString(),
            pageSize: pagination.pageSize.toString(),
            sortBy: 'createdAt',
            sortDirection: 'desc'
        });

        // Add search term
        if (filters.searchTerm) {
            params.append('searchTerm', filters.searchTerm);
        }

        // Add filters
        Object.entries(filters).forEach(([key, value]) => {
            if (value !== undefined && key !== 'searchTerm') {
                params.append(`filters[${key}]`, value.toString());
            }
        });

        const response = await fetch(`/api/products?${params.toString()}`);
        const data = await response.json();
        setProducts(data.items);
    };

    useEffect(() => {
        fetchProducts();
    }, [filters, pagination]);

    return (
        <div>
            {/* Filter controls */}
            <input
                type="text"
                placeholder="Search products..."
                onChange={(e) => setFilters(prev => ({ ...prev, searchTerm: e.target.value }))}
            />
            <select onChange={(e) => setFilters(prev => ({ ...prev, categoryId: Number(e.target.value) }))}>
                <option value="">All Categories</option>
                <option value="1">Electronics</option>
                <option value="2">Clothing</option>
            </select>
            <input
                type="number"
                placeholder="Min Price"
                onChange={(e) => setFilters(prev => ({ ...prev, minPrice: Number(e.target.value) }))}
            />
            <input
                type="number"
                placeholder="Max Price"
                onChange={(e) => setFilters(prev => ({ ...prev, maxPrice: Number(e.target.value) }))}
            />
            
            {/* Product list */}
            {products.map(product => (
                <div key={product.id}>{product.name} - ${product.price}</div>
            ))}
        </div>
    );
};
```

#### Using Extension Methods

```csharp
// Create pagination with extension methods
var pagination = new PaginationDto()
    .WithSearch("laptop")
    .WithSort("Price", SortDirection.Asc)
    .WithFilters(new Dictionary<string, object>
    {
        ["categoryId"] = 1,
        ["isActive"] = true,
        ["minPrice"] = 500,
        ["maxPrice"] = 2000
    });

// Use built-in filter building methods
var sqlParams = pagination.BuildSqlParameters();
// Result: { ["@categoryId"] = 1, ["@isActive"] = true, ["@minPrice"] = 500, ["@maxPrice"] = 2000 }

var customParams = pagination.BuildFiltersWithPrefix(":");
// Result: { [":categoryId"] = 1, [":isActive"] = true, [":minPrice"] = 500, [":maxPrice"] = 2000 }
```

#### Query Parameter Examples

```
# Basic pagination
GET /api/products?pageIndex=1&pageSize=20

# With search and sorting
GET /api/products?pageIndex=1&pageSize=20&searchTerm=laptop&sortBy=price&sortDirection=asc

# With multiple filters
GET /api/products?pageIndex=1&pageSize=20&filters[categoryId]=1&filters[isActive]=true&filters[minPrice]=500

# Complex filter combinations
GET /api/products?pageIndex=1&pageSize=20&searchTerm=laptop&filters[categoryId]=1&filters[isActive]=true&filters[minPrice]=500&filters[maxPrice]=2000&filters[createdDate]=2024-01-01
```

#### Stored Procedure with Dynamic WHERE

```sql
CREATE OR ALTER PROCEDURE sp_GetPaginatedProducts
    @PageIndex INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(100) = NULL,
    @SortBy NVARCHAR(50) = 'CreatedAt',
    @SortDirection NVARCHAR(4) = 'ASC',
    -- Dynamic filters
    @CategoryId INT = NULL,
    @IsActive BIT = NULL,
    @MinPrice DECIMAL(18,2) = NULL,
    @MaxPrice DECIMAL(18,2) = NULL,
    @CreatedDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @SQL NVARCHAR(MAX);
    DECLARE @WhereClause NVARCHAR(MAX) = '';
    
    -- Build WHERE clause dynamically
    IF @SearchTerm IS NOT NULL
        SET @WhereClause = @WhereClause + ' AND (Name LIKE @SearchTerm OR Description LIKE @SearchTerm)';
    
    IF @CategoryId IS NOT NULL
        SET @WhereClause = @WhereClause + ' AND CategoryId = @CategoryId';
    
    IF @IsActive IS NOT NULL
        SET @WhereClause = @WhereClause + ' AND IsActive = @IsActive';
    
    IF @MinPrice IS NOT NULL
        SET @WhereClause = @WhereClause + ' AND Price >= @MinPrice';
    
    IF @MaxPrice IS NOT NULL
        SET @WhereClause = @WhereClause + ' AND Price <= @MaxPrice';
    
    IF @CreatedDate IS NOT NULL
        SET @WhereClause = @WhereClause + ' AND CAST(CreatedAt AS DATE) = @CreatedDate';
    
    -- Remove leading ' AND ' if exists
    IF LEN(@WhereClause) > 0
        SET @WhereClause = ' WHERE ' + SUBSTRING(@WhereClause, 6, LEN(@WhereClause));
    
    -- Build and execute final query with pagination
    SET @SQL = '
        WITH PaginatedData AS (
            SELECT 
                Id, Name, Description, Price, CategoryId, IsActive, CreatedAt,
                ROW_NUMBER() OVER (ORDER BY ' + @SortBy + ' ' + @SortDirection + ') AS RowNum
            FROM Products
            WHERE IsDeleted = 0' + @WhereClause + '
        )
        SELECT 
            Id, Name, Description, Price, CategoryId, IsActive, CreatedAt
        FROM PaginatedData
        WHERE RowNum BETWEEN (@PageIndex - 1) * @PageSize + 1 
                          AND @PageIndex * @PageSize;
        
        -- Get total count for pagination
        SELECT COUNT(*) AS TotalCount
        FROM Products
        WHERE IsDeleted = 0' + @WhereClause + ';';
    
    EXEC sp_executesql @SQL,
        N'@PageIndex INT, @PageSize INT, @SearchTerm NVARCHAR(100), @CategoryId INT, @IsActive BIT, @MinPrice DECIMAL(18,2), @MaxPrice DECIMAL(18,2), @CreatedDate DATE',
        @PageIndex, @PageSize, @SearchTerm, @CategoryId, @IsActive, @MinPrice, @MaxPrice, @CreatedDate;
END
```

## 🔧 Advanced .NET 9 Features

### Domain Events

```csharp
// Domain event with record
public record ProductCreatedEvent(int ProductId, string ProductName, DateTime OccurredOn) : IDomainEvent
{
    public ProductCreatedEvent(int productId, string productName) : this(productId, productName, DateTime.UtcNow) { }
}

// Entity with domain events
public class Product : BaseEntity
{
    public void MarkAsCreated() =>
        AddDomainEvent(new ProductCreatedEvent(Id, Name));
}
```

### Bulk Operations

```csharp
// High-performance bulk operations
await repository.BulkInsertAsync(products);
await repository.BulkUpdateAsync(p => p.CategoryId == 1, p => p.Status, "Active");
await repository.BulkDeleteAsync(p => p.CreatedAt < DateTime.UtcNow.AddDays(-30));

// Projections for efficient data transfer
var summaries = await repository.GetPagedProjectionAsync(
    pagination,
    p => new ProductSummary(p.Id, p.Name, p.Price),
    p => p.IsActive
);
```

### Async Streaming

```csharp
// Memory-efficient processing with async streaming
await foreach (var product in repository.FindAsyncEnumerable(p => p.IsActive))
{
    await ProcessProductAsync(product);
}
```

### JSON Handling

```csharp
// Business-optimized JSON with source generation
var json = myObject.SerializeOptimized();
var obj = jsonString.DeserializeOptimized<MyType>();
var clone = myObject.CloneDeep();

// JSON options for different scenarios
var options = JsonExtensions.DefaultOptions; // Production
var prettyOptions = JsonExtensions.PrettyOptions; // Development
var strictOptions = JsonExtensions.StrictOptions; // APIs
```

## 🏗️ Architecture Patterns

### Domain-Driven Design
- **Entities**: Record-based entities with primary constructors
- **Value Objects**: Immutable objects with C# features
- **Domain Events**: Record-based events with patterns
- **Specifications**: Type-safe query composition with expressions

### Repository Pattern
- **Generic Repository**: Type-safe data access with features
- **Specification Pattern**: Flexible query composition with syntax
- **Unit of Work**: Transaction management with patterns
- **Bulk Operations**: High-performance batch processing with EF Core 9

### Error Handling
- **Domain Errors**: Record struct-based error types with HTTP mapping
- **Result Pattern**: Functional error handling with syntax
- **API Responses**: Consistent error response format with features
- **Error Aggregation**: Multiple error handling with LINQ

## 🔍 Validation Examples

```csharp
// Validation with pattern matching
var validationResult = input switch
{
    { Length: 0 } => DomainError.Validation("EMPTY_INPUT", "Input cannot be empty"),
    { Length: > 100 } => DomainError.Validation("TOO_LONG", "Input too long"),
    _ => null
};

// JSON validation
if (!DataValidation.IsValidJson(jsonContent))
{
    return DomainError.Validation("INVALID_JSON", "Invalid JSON format");
}

// XML validation
var xmlErrors = XmlValidator.Validate(xmlContent, xsdSchema);
if (xmlErrors.Any())
{
    return DomainError.Validation("INVALID_XML", "XML validation failed");
}
```

## 📚 API Documentation

### Entity Base Classes
- `Entity<TId>` - Base entity with domain events
- `BaseEntity` - Base for int-keyed entities with audit trail
- `IAuditableEntity` - Interface for audit trail support
- `IEntityWithDomainEvents` - Interface for domain event support

### DTOs
- `ApiResponse<T>` - API response format with record structs
- `PaginationDto` - Pagination with search and filtering
- `PagedResult<T>` - Paginated results with metadata
- `ApiError` - Error information with record structs

### Error Handling
- `DomainError` - Business logic errors with HTTP mapping
- `Result<T>` - Functional result pattern with record structs
- `ErrorType` - Error type categorization
- `ErrorTypeExtensions` - Error type utilities

### Repository
- `IRepository<TEntity>` - Comprehensive repository interface
- `ISpecification<T>` - Query specification pattern
- `BaseSpecification<T>` - Base class for specifications
- `PagedResult<T>` - Paginated query results

### Validation
- `DataValidation` - Data validation utilities
- `XmlValidator` - XML validation with XSD schemas
- `ValidationExtensions` - Extension methods for validation

### Extensions
- `PaginationExtensions` - Pagination builder methods
- `RepositoryAuditExtensions` - Audit trail extensions
- `JsonExtensions` - JSON serialization utilities (with optimized and deprecated methods)
- `NullableExtensions` - Nullable type helpers
- `EnumExtensions` - Enum manipulation utilities

### Constants & Helpers
- `ApiMetadataKeys` - Standard API metadata keys
- `ApiResponseHelpers` - API response creation utilities

### Features
- **Collection Expressions**: `[]` syntax for collection initialization
- **Required Properties**: Compile-time null safety with `required` keyword
- **Record Structs**: High-performance value types for DTOs and results
- **Primary Constructors**: Concise type definitions with syntax
- **Pattern Matching**: Advanced type-safe operations with patterns
- **Async Streaming**: Memory-efficient data processing with `IAsyncEnumerable<T>`
- **Source Generation**: JSON serialization with AOT compilation support

## 🆕 What's New in Version 1.3.3

### .NET 9 Features
- **Primary Constructors**: Concise record and class definitions
- **Collection Expressions**: `[]` syntax for efficient initialization
- **Required Properties**: Compile-time null safety with `required` keyword
- **Record Structs**: High-performance value types for DTOs and results
- **Pattern Matching**: Advanced `switch` expressions and `is` patterns
- **Source Generators**: JSON serialization with AOT compilation support
- **Global Usings**: Clean namespace management with global using directives

### Performance Improvements
- **Source Generation**: JSON serialization with source generation for AOT
- **Record Structs**: High-performance value types for DTOs and results
- **Collection Expressions**: Collection initialization with better performance
- **Async Streaming**: Memory-efficient data processing with `IAsyncEnumerable<T>`
- **Bulk Operations**: Optimized batch processing with EF Core 9

### Developer Experience
- **C# Features**: Latest .NET 9+ language capabilities
- **Type Safety**: Enhanced compile-time safety with features
- **Better Documentation**: Comprehensive examples with syntax
- **Error Handling**: Improved error aggregation with patterns

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

**Built with ❤️ for the .NET community using the latest .NET 9 features**
