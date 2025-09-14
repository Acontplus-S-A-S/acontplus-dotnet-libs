# Acontplus.Core

[![NuGet](https://img.shields.io/nuget/v/Acontplus.Core.svg)](https://www.nuget.org/packages/Acontplus.Core)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

A cutting-edge .NET 9+ foundational library leveraging the latest C# language features and business patterns. Built with performance, type safety, and developer experience in mind.

## 🚀 .NET Features

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
- **Warnings System** - Success with warnings pattern for complex business operations

### 📊 Advanced Data Patterns
- **Async Streaming** - `IAsyncEnumerable<T>` for memory-efficient processing
- **Projections** - Expression-based data transfer for performance  
- **Bulk Operations** - High-performance batch processing interfaces
- **Smart Pagination** - Advanced pagination with search and filtering
- **JSON Utilities** - System.Text.Json with source generation
- **Repository Interfaces** - Complete repository abstractions with CRUD, specifications, and bulk operations
- **Clean Architecture** - No persistence dependencies, implementations provided in separate packages

## 🔥 Core Features

### 🌟 **NEW: Global Business Enums**

**17 comprehensive business enums** available globally across all applications - no more duplicate definitions!

#### **🔄 Process & Status Management**
- **`BusinessStatus`** - 13 lifecycle states (Draft → Active → Archived)
- **`Priority`** - 5 priority levels (Low → Emergency) 
- **`DocumentType`** - 16 document types (Invoice, Contract, Report, etc.)
- **`EventType`** - 19 event types (Authentication, CRUD operations, Workflow, etc.)

#### **👤 Person & Demographics** 
- **`Gender`** - 5 inclusive options (Male, Female, NonBinary, Other, NotSpecified)
- **`MaritalStatus`** - 8 relationship states (Single, Married, Divorced, etc.)
- **`Title`** - 12 honorifics (Mr, Mrs, Dr, Prof, Sir, Dame, etc.)

#### **🏢 Business & Organization**
- **`Industry`** - 19 industry classifications (Technology, Healthcare, Finance, etc.)
- **`CompanySize`** - 11 size categories (Startup → Multinational Corporation)

#### **💰 Financial & Commerce**
- **`Currency`** - 15 international currencies (USD, EUR + Latin American)
- **`PaymentMethod`** - 15 payment options (Cards, Digital wallets, BNPL, etc.)

#### **🔐 Security & Access**
- **`UserRole`** - 14 role levels (Guest → SuperAdmin → ServiceAccount)

#### **🌍 Internationalization**
- **`Language`** - 20 languages (Major world languages + Latin American Spanish)  
- **`TimeZone`** - 16 time zones (UTC, regional + Latin American zones)

#### **📱 Communication & Content**
- **`CommunicationChannel`** - 11 channels (Email, SMS, WhatsApp, Teams, etc.)
- **`AddressType`** - 12 address categories (Home, Work, Billing, Shipping, etc.)
- **`ContentType`** - 20 media types (Text, Images, Videos, Documents, Archives)

```csharp
// ✅ Available everywhere via global using
public class Customer : BaseEntity 
{
    public Gender Gender { get; set; }                    // 🌟 Global enum
    public Title Title { get; set; }                      // 🌟 Global enum  
    public MaritalStatus MaritalStatus { get; set; }      // 🌟 Global enum
    public Language PreferredLanguage { get; set; }       // 🌟 Global enum
    public CommunicationChannel PreferredChannel { get; set; } // 🌟 Global enum
}

public class Order : BaseEntity
{
    public BusinessStatus Status { get; set; }            // 🌟 Global enum
    public Priority Priority { get; set; }                // 🌟 Global enum
    public Currency Currency { get; set; }                // 🌟 Global enum  
    public PaymentMethod PaymentMethod { get; set; }      // 🌟 Global enum
}
```

### 🔄 **Comprehensive Result Pattern System**

**Complete Railway-Oriented Programming implementation** with functional composition, multiple error handling, and fluent API design.

#### **🎯 Core Result Types**

```csharp
// Generic Result with custom error type
Result<TValue, TError>

// Result with fixed DomainError (most common)
Result<TValue>

// Multiple errors support
Result<TValue, DomainErrors>

// Success with warnings pattern
SuccessWithWarnings<TValue>
```

#### **✨ Fluent API - Create Results from Errors**

```csharp
// ✅ NEW: Fluent API from DomainError
public static Result<User> GetUser(int id) =>
    id <= 0 
        ? DomainError.Validation("INVALID_ID", "ID must be positive").Failure<User>()
        : new User { Id = id };

// ✅ NEW: Fluent API from DomainErrors
public static Result<User, DomainErrors> ValidateUser(CreateUserRequest request)
{
    var errors = new List<DomainError>();
    
    if (string.IsNullOrEmpty(request.Name))
        errors.Add(DomainError.Validation("NAME_REQUIRED", "Name required"));
        
    if (string.IsNullOrEmpty(request.Email))
        errors.Add(DomainError.Validation("EMAIL_REQUIRED", "Email required"));
        
    return errors.Count > 0 
        ? DomainErrors.Multiple(errors).FailureMultiple<User>()
        : new User { Name = request.Name, Email = request.Email };
}
```

#### **🔧 Traditional Factory Methods (Still Supported)**

```csharp
// Traditional approach still works
Result<Product>.Success(product);
Result<Product>.Failure(domainError);

// Generic version
Result<Product, DomainError>.Success(product);
Result<Product, DomainError>.Failure(domainError);
```

#### **🚀 Advanced Error Types**

```csharp
// 🔸 Single Domain Error
var singleError = DomainError.NotFound("USER_001", "User not found");

// 🔸 Multiple Domain Errors
var multipleErrors = DomainErrors.Multiple(
    DomainError.Validation("NAME_REQUIRED", "Name is required"),
    DomainError.Validation("EMAIL_INVALID", "Email format is invalid")
);

// 🔸 Domain Warnings (Success with issues)
var warnings = DomainWarnings.Multiple(
    DomainError.Validation("DATA_INCOMPLETE", "Some optional fields missing"),
    DomainError.Validation("DEPRECATED_API", "Using deprecated API version")
);
```

#### **⚡ Functional Composition**

```csharp
// Railway-oriented programming
public async Task<Result<OrderConfirmation>> ProcessOrderAsync(CreateOrderRequest request)
{
    return await ValidateOrderRequest(request)
        .Map(order => CalculateTotal(order))
        .MapAsync(order => ProcessPaymentAsync(order))
        .Map(order => GenerateConfirmation(order))
        .OnFailure(error => _logger.LogError("Order processing failed: {Error}", error));
}

// Pattern matching
public IActionResult HandleOrderResult(Result<Order> result)
{
    return result.Match(
        success: order => Ok(order),
        failure: error => BadRequest(error.ToApiError())
    );
}

// Async pattern matching
public async Task<string> FormatOrderResultAsync(Result<Order> result)
{
    return await result.MatchAsync(
        success: async order => await FormatOrderAsync(order),
        failure: async error => await FormatErrorAsync(error)
    );
}
```

#### **🔗 Chaining Operations**

```csharp
// Chain operations with Map
var result = await GetUserAsync(userId)
    .Map(user => ValidateUser(user))
    .MapAsync(user => EnrichUserDataAsync(user))
    .Map(user => ConvertToDto(user));

// Handle side effects
var result = await ProcessDataAsync()
    .OnSuccess(data => _logger.LogInformation("Processing completed"))
    .OnFailure(error => _logger.LogError("Processing failed: {Error}", error));

// Transform errors
var result = await CallExternalApiAsync()
    .MapError(error => DomainError.External("API_ERROR", $"External service failed: {error}"));
```

#### **🚨 Error Analysis & Handling**

```csharp
// Error severity analysis
var errors = DomainErrors.Multiple(
    DomainError.Internal("DB_ERROR", "Database connection failed"),
    DomainError.Validation("INVALID_EMAIL", "Invalid email format")
);

var mostSevere = errors.GetMostSevereErrorType(); // Returns ErrorType.Internal
var httpStatus = mostSevere.ToHttpStatusCode();   // Returns 500

// Error filtering and grouping
var validationErrors = errors.GetErrorsOfType(ErrorType.Validation);
var hasServerErrors = errors.HasErrorsOfType(ErrorType.Internal);

// Aggregate error messages
var summary = errors.GetAggregateErrorMessage();
// "Multiple errors occurred (2): [Internal] Database connection failed; [Validation] Invalid email format"
```

#### **⚠️ Success with Warnings Pattern**

```csharp
// Business operations that succeed but have warnings
public async Task<SuccessWithWarnings<List<Product>>> ImportProductsAsync(List<ProductDto> dtos)
{
    var products = new List<Product>();
    var warnings = new List<DomainError>();

    foreach (var dto in dtos)
    {
        try
        {
            var product = await CreateProductAsync(dto);
            products.Add(product);
        }
        catch (ValidationException ex)
        {
            warnings.Add(DomainError.Validation("IMPORT_WARNING", 
                $"Product {dto.Name} skipped: {ex.Message}"));
        }
    }

    return new SuccessWithWarnings<List<Product>>(products, new DomainWarnings(warnings));
}

// Extension methods for warnings
var result = products.WithWarning(DomainError.Validation("WARN_001", "Some data incomplete"));
var resultMultiple = products.WithWarnings(warnings);
```

#### **🌐 HTTP Integration**

```csharp
// Automatic HTTP status code mapping
var error = DomainError.Validation("INVALID_INPUT", "Input validation failed");
var statusCode = error.GetHttpStatusCode(); // Returns 422 (Unprocessable Entity)

// Convert to API responses
var apiResponse = error.ToApiResponse<ProductDto>();
var resultResponse = result.ToApiResponse<ProductDto>("Operation completed");

// Built-in error type mappings:
ErrorType.Validation      → 422 Unprocessable Entity
ErrorType.NotFound        → 404 Not Found
ErrorType.Unauthorized    → 401 Unauthorized
ErrorType.Forbidden       → 403 Forbidden
ErrorType.Conflict        → 409 Conflict
ErrorType.Internal        → 500 Internal Server Error
ErrorType.External        → 502 Bad Gateway
ErrorType.RateLimited     → 429 Too Many Requests
// ... and more
```

#### **🎨 Real-World Examples**

```csharp
// ✅ Simple validation
public Result<User> CreateUser(string name, string email)
{
    if (string.IsNullOrWhiteSpace(name))
        return DomainError.Validation("NAME_REQUIRED", "Name is required").Failure<User>();
        
    if (!IsValidEmail(email))
        return DomainError.Validation("EMAIL_INVALID", "Invalid email format").Failure<User>();
        
    return new User { Name = name, Email = email };
}

// ✅ Complex business logic with multiple errors
public async Task<Result<Order, DomainErrors>> ProcessOrderAsync(OrderRequest request)
{
    var errors = new List<DomainError>();
    
    // Validate customer
    var customer = await _customerService.GetByIdAsync(request.CustomerId);
    if (customer is null)
        errors.Add(DomainError.NotFound("CUSTOMER_NOT_FOUND", "Customer not found"));
    
    // Validate products
    foreach (var item in request.Items)
    {
        var product = await _productService.GetByIdAsync(item.ProductId);
        if (product is null)
            errors.Add(DomainError.NotFound("PRODUCT_NOT_FOUND", $"Product {item.ProductId} not found"));
        else if (product.Stock < item.Quantity)
            errors.Add(DomainError.Conflict("INSUFFICIENT_STOCK", $"Not enough stock for {product.Name}"));
    }
    
    if (errors.Count > 0)
        return DomainErrors.Multiple(errors).FailureMultiple<Order>();
        
    // Process order
    var order = new Order
    {
        CustomerId = request.CustomerId,
        Items = request.Items,
        Status = BusinessStatus.Active
    };
    
    return await _orderRepository.CreateAsync(order);
}

// ✅ Functional composition for complex workflows
public async Task<Result<InvoiceDto>> GenerateInvoiceAsync(int orderId)
{
    return await GetOrderAsync(orderId)
        .MapAsync(order => ValidateOrderForInvoicingAsync(order))
        .MapAsync(order => CalculateInvoiceAmountsAsync(order))
        .MapAsync(invoice => ApplyTaxCalculationsAsync(invoice))
        .MapAsync(invoice => GeneratePdfAsync(invoice))
        .Map(invoice => ConvertToDto(invoice))
        .OnSuccess(invoice => _logger.LogInformation("Invoice generated: {InvoiceId}", invoice.Id))
        .OnFailure(error => _logger.LogError("Invoice generation failed: {Error}", error));
}
```

#### **🎯 Best Practices**

```csharp
// ✅ DO: Use specific error codes and messages
DomainError.NotFound("USER_NOT_FOUND", $"User with ID {id} was not found");

// ❌ DON'T: Use generic error messages
DomainError.NotFound("ERROR", "Something went wrong");

// ✅ DO: Use fluent API for cleaner code
return id <= 0 
    ? DomainError.Validation("INVALID_ID", "ID must be positive").Failure<User>()
    : GetUserFromDatabase(id);

// ✅ DO: Use pattern matching for flow control
return result.Match(
    success: user => ProcessUser(user),
    failure: error => HandleError(error)
);

// ✅ DO: Chain operations for complex workflows
var result = await ValidateInput(input)
    .MapAsync(data => ProcessDataAsync(data))
    .Map(processed => FormatOutput(processed))
    .OnFailure(error => LogError(error));

// ✅ DO: Use DomainErrors for multiple validation errors
var errors = new List<DomainError>();
if (IsInvalid(name)) errors.Add(DomainError.Validation("INVALID_NAME", "Name invalid"));
if (IsInvalid(email)) errors.Add(DomainError.Validation("INVALID_EMAIL", "Email invalid"));

return errors.Count > 0 
    ? DomainErrors.Multiple(errors).FailureMultiple<User>()
    : CreateUser(name, email);
```

### 🔍 **Validation Utilities**

Comprehensive validation utilities for common business scenarios:

```csharp
// Data Validation
public static class DataValidation
{
    public static bool IsValidJson(string json);
    public static bool IsValidXml(string xml);
    public static bool IsValidEmail(string email);
    public static bool IsValidUrl(string url);
    public static bool IsValidPhoneNumber(string phoneNumber);
}

// XML Validation with Schemas
public static class XmlValidator
{
    public static IEnumerable<ValidationError> Validate(string xmlContent, string xsdSchema);
    public static bool IsValid(string xmlContent, string xsdSchema);
    public static ValidationResult ValidateWithDetails(string xmlContent, string xsdSchema);
}

// Usage Examples
var validationResult = input switch
{
    { Length: 0 } => DomainError.Validation("EMPTY_INPUT", "Input cannot be empty"),
    { Length: > 100 } => DomainError.Validation("TOO_LONG", "Input too long"),
    _ when !DataValidation.IsValidEmail(input) => DomainError.Validation("INVALID_EMAIL", "Invalid email format"),
    _ => null
};

// JSON Validation
if (!DataValidation.IsValidJson(jsonContent))
{
    return DomainError.Validation("INVALID_JSON", "Invalid JSON format");
}

// XML Validation with Schema
var xmlErrors = XmlValidator.Validate(xmlContent, xsdSchema);
if (xmlErrors.Any())
{
    return DomainError.Validation("INVALID_XML", "XML validation failed", 
        details: new Dictionary<string, object> { ["errors"] = xmlErrors.ToList() });
}
```

### 🔥 **Advanced JSON Extensions**

Business-optimized JSON handling with multiple serialization options:

```csharp
// JSON Serialization Options
public static class JsonExtensions
{
    public static JsonSerializerOptions DefaultOptions { get; } // Production-optimized
    public static JsonSerializerOptions PrettyOptions { get; }  // Development-friendly
    public static JsonSerializerOptions StrictOptions { get; }  // API-strict validation
}

// Serialization Methods
var json = myObject.SerializeOptimized(); // Uses DefaultOptions
var prettyJson = myObject.SerializeOptimized(pretty: true); // Uses PrettyOptions

// Deserialization with Error Handling
try
{
    var obj = jsonString.DeserializeOptimized<MyType>();
}
catch (JsonException ex)
{
    // Detailed error information
    var error = DomainError.Validation("JSON_DESERIALIZE_ERROR", ex.Message);
}

// Safe Deserialization with Fallback
var obj = jsonString.DeserializeSafe<MyType>(fallback: new MyType());

// Deep Cloning via JSON
var clone = myObject.CloneDeep(); // Creates deep copy via JSON serialization

// Business Examples
public class ProductService
{
    public async Task<string> ExportProductsAsync(List<Product> products, bool prettyFormat = false)
    {
        return products.SerializeOptimized(pretty: prettyFormat);
    }

    public async Task<List<Product>> ImportProductsAsync(string json)
    {
        try
        {
            return json.DeserializeOptimized<List<Product>>();
        }
        catch (JsonException ex)
        {
            throw new BusinessException("Failed to import products", ex);
        }
    }
}
```

### 🧩 **Powerful Extension Methods**

Comprehensive extension methods for enhanced productivity:

#### **Nullable Extensions**
```csharp
public static class NullableExtensions
{
    public static bool IsNull<T>(this T? value) where T : class;
    public static bool IsNotNull<T>(this T? value) where T : class;
    public static T OrDefault<T>(this T? value, T defaultValue) where T : class;
    public static T OrThrow<T>(this T? value, Exception exception) where T : class;
    public static T OrThrow<T>(this T? value, string message) where T : class;
}

// Usage Examples
var result = nullableValue.OrDefault("default value");
var safeValue = nullableValue.OrThrow("Value is required");
var user = userRepository.GetByIdAsync(id).OrThrow(new UserNotFoundException(id));

if (product.IsNotNull())
{
    // Process product
}
```

#### **Enum Extensions**
```csharp
public static class EnumExtensions
{
    public static string DisplayName(this Enum value); // Gets Description attribute or ToString()
}

// Usage with Description Attributes
public enum Priority
{
    [Description("Low Priority")]
    Low = 1,
    
    [Description("Normal Priority")]
    Normal = 2,
    
    [Description("High Priority - Urgent")]
    High = 3
}

var displayName = Priority.High.DisplayName(); // Returns "High Priority - Urgent"
```

#### **Pagination Extensions**
```csharp
public static class PaginationExtensions
{
    public static PaginationDto WithSearch(this PaginationDto pagination, string searchTerm);
    public static PaginationDto WithSort(this PaginationDto pagination, string sortBy, SortDirection direction);
    public static PaginationDto WithFilters(this PaginationDto pagination, Dictionary<string, object> filters);
    public static Dictionary<string, object> BuildSqlParameters(this PaginationDto pagination);
    public static Dictionary<string, object> BuildFiltersWithPrefix(this PaginationDto pagination, string prefix);
}
```

#### **Domain Error Extensions**
```csharp
public static class DomainErrorExtensions
{
    public static ApiResponse<T> ToApiResponse<T>(this DomainError error, string? correlationId = null);
    public static ApiResponse<T> ToApiResponse<T>(this Result<T> result, string? correlationId = null);
    public static DomainError GetMostSevereError(this IEnumerable<DomainError> errors);
    public static HttpStatusCode GetHttpStatusCode(this DomainError error);
    public static string GetAggregateErrorMessage(this DomainErrors errors);
}
```

### Validation System

#### **Data Validation**
```csharp
public static class DataValidation
{
    public static bool IsValidJson(string json);
    public static bool IsValidXml(string xml);
    public static bool IsValidEmail(string email);
    public static bool IsValidUrl(string url);
    public static bool IsValidPhoneNumber(string phoneNumber);
}
```

#### **XML Validation**
```csharp
public static class XmlValidator
{
    public static IEnumerable<ValidationError> Validate(string xmlContent, string xsdSchema);
    public static bool IsValid(string xmlContent, string xsdSchema);
    public static ValidationResult ValidateWithDetails(string xmlContent, string xsdSchema);
}
```

### Constants & Helpers

#### **API Metadata Keys**
```csharp
public static class ApiMetadataKeys
{
    public const string Page = "page";
    public const string PageSize = "pageSize";
    public const string TotalItems = "totalItems";
    public const string TotalPages = "totalPages";
    public const string HasNextPage = "hasNextPage";
    public const string HasPreviousPage = "hasPreviousPage";
    public const string CorrelationId = "correlationId";
    // ... and more
}
```

#### **API Response Helpers**
```csharp
public static class ApiResponseHelpers
{
    public static ApiResponse<T> CreateSuccessResponse<T>(T data, string message);
    public static ApiResponse<T> CreateErrorResponse<T>(string message, string errorCode);
    public static ApiResponse<T> CreateValidationErrorResponse<T>(IEnumerable<ValidationError> errors);
    public static ApiResponse<T> CreateNotFoundResponse<T>(string message);
}
```

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
