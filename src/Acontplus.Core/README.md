# Acontplus.Core

[![NuGet](https://img.shields.io/nuget/v/Acontplus.Core.svg)](https://www.nuget.org/packages/Acontplus.Core)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

A cutting-edge .NET 9+ foundational library leveraging the latest C# language features and business patterns. Built with performance, type safety, and developer experience in mind.

## 🚀 What's New (Latest Version)

- **✨ Improved Separation of Concerns** - `DomainError`/`DomainErrors` no longer create `Result` instances directly
  - Use `Result<T>.Failure(error)`, `Result<T, DomainErrors>.Failure(errors)` or extension helpers
  - New helpers: `error.ToResult<T>()`, `errors.ToFailureResult<T>()`
- **⚡ Enhanced Async Performance** - ValueTask and CancellationToken support
  - `MapAsync/BindAsync/TapAsync/MatchAsync` now have `ValueTask` variants and CT overloads
- **🛡️ Safer Default Handling** - Better default(Result) protection
  - Default guard, `TryGetValue`, `TryGetError`, and `Deconstruct(out bool, out TValue?, out TError?)`
- **🎯 Success-with-Warnings Helpers** - Enhanced warning pattern support
  - `value.ToSuccessWithWarningsResult(warnings)`

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

### 🌟 **Global Business Enums**

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

**Complete Railway-Oriented Programming implementation** with functional composition, multiple error handling, and clean separation of concerns.

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

#### **✨ Current API - Create Results Properly**

```csharp
// ✅ CURRENT: Single error using Result factory
public static Result<User> GetUser(int id) =>
    id <= 0 
        ? Result<User>.Failure(DomainError.Validation("INVALID_ID", "ID must be positive"))
        : Result<User>.Success(new User { Id = id });

// ✅ CURRENT: Single error using extension helper
public static Result<User> GetUserAlt(int id) =>
    id <= 0 
        ? DomainError.Validation("INVALID_ID", "ID must be positive").ToResult<User>()
        : new User { Id = id }.ToResult();

// ✅ CURRENT: Multiple errors using Result factory
public static Result<User, DomainErrors> ValidateUser(CreateUserRequest request)
{
    var errors = new List<DomainError>();
    
    if (string.IsNullOrEmpty(request.Name))
        errors.Add(DomainError.Validation("NAME_REQUIRED", "Name required"));
        
    if (string.IsNullOrEmpty(request.Email))
        errors.Add(DomainError.Validation("EMAIL_REQUIRED", "Email required"));
        
    return errors.Count > 0 
        ? Result<User, DomainErrors>.Failure(new DomainErrors(errors))
        : Result<User, DomainErrors>.Success(new User { Name = request.Name, Email = request.Email });
}

// ✅ CURRENT: Multiple errors using extension helper
public static Result<User, DomainErrors> ValidateUserAlt(CreateUserRequest request)
{
    var errors = new List<DomainError>();
    
    if (string.IsNullOrEmpty(request.Name))
        errors.Add(DomainError.Validation("NAME_REQUIRED", "Name required"));
        
    if (string.IsNullOrEmpty(request.Email))
        errors.Add(DomainError.Validation("EMAIL_REQUIRED", "Email required"));
        
    return errors.Count > 0 
        ? errors.ToFailureResult<User>()
        : Result<User, DomainErrors>.Success(new User { Name = request.Name, Email = request.Email });
}
```

#### **🔧 Result Factory Methods**

```csharp
// Single error results
Result<Product>.Success(product);
Result<Product>.Failure(domainError);

// Multiple error results
Result<Product, DomainErrors>.Success(product);
Result<Product, DomainErrors>.Failure(domainErrors);

// Extension helpers for convenience
var successResult = product.ToResult();
var failureResult = error.ToResult<Product>();
var multiFailureResult = errors.ToFailureResult<Product>();
```

#### **⚡ Enhanced Functional Composition**

```csharp
// Railway-oriented programming with async/ValueTask support
public async Task<Result<OrderConfirmation>> ProcessOrderAsync(CreateOrderRequest request, CancellationToken ct = default)
{
    return await ValidateOrderRequest(request)
        .Map(order => CalculateTotal(order))
        .MapAsync(order => ProcessPaymentAsync(order))
        .MapAsync(async (order, token) => await ReserveStockAsync(order, token), ct)
        .Map(order => GenerateConfirmation(order))
        .OnFailure(error => _logger.LogError("Order processing failed: {Error}", error));
}

// Pattern matching with improved ergonomics
public IActionResult HandleOrderResult(Result<Order> result)
{
    var (isSuccess, value, error) = result; // Deconstruct support
    
    return result.Match(
        success: order => Ok(order),
        failure: error => BadRequest(error.ToApiResponse<Order>())
    );
}

// Safe value access
public string GetOrderStatus(Result<Order> result)
{
    if (result.TryGetValue(out var order))
        return order.Status.ToString();
        
    if (result.TryGetError(out var error))
        return $"Error: {error.Message}";
        
    return "Unknown";
}
```

#### **🔗 Advanced Chaining Operations**

```csharp
// Chain operations with enhanced error handling
var result = await GetUserAsync(userId)
    .Map(user => ValidateUser(user))
    .MapAsync(user => EnrichUserDataAsync(user))
    .MapAsync(async (user, ct) => await CallExternalApiAsync(user, ct), CancellationToken.None)
    .MapError(error => DomainError.External("API_ERROR", $"External service failed: {error.Code}"))
    .OnSuccess(user => _logger.LogInformation("User processed: {UserId}", user.Id))
    .OnFailure(error => _logger.LogError("User processing failed: {Error}", error));

// ValueTask support for high-performance scenarios
public async ValueTask<Result<ProcessedData>> ProcessDataAsync(RawData data)
{
    return await ValidateData(data)
        .BindAsync(async validData => await TransformDataAsync(validData))
        .TapAsync(async processedData => await LogProcessingAsync(processedData));
}
```

#### **🚨 Comprehensive Error Handling**

```csharp
// Error severity analysis and HTTP mapping
var errors = DomainErrors.Multiple(
    DomainError.Internal("DB_ERROR", "Database connection failed"),
    DomainError.Validation("INVALID_EMAIL", "Invalid email format")
);

var mostSevere = errors.GetMostSevereErrorType(); // Returns ErrorType.Internal
var httpStatus = mostSevere.ToHttpStatusCode();   // Returns 500

// Error filtering and analysis
var validationErrors = errors.GetErrorsOfType(ErrorType.Validation);
var hasServerErrors = errors.HasErrorsOfType(ErrorType.Internal);
var summary = errors.GetAggregateErrorMessage();

// Convert to API responses
var apiResponse = errors.ToApiResponse<ProductDto>();
```

#### **⚠️ Success with Warnings Pattern**

```csharp
// Enhanced success with warnings support
public async Task<Result<SuccessWithWarnings<List<Product>>>> ImportProductsAsync(List<ProductDto> dtos)
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

    var successWithWarnings = new SuccessWithWarnings<List<Product>>(
        products, 
        new DomainWarnings(warnings)
    );
    
    return Result<SuccessWithWarnings<List<Product>>>.Success(successWithWarnings);
}

// Using extension helpers
var result = products.ToSuccessWithWarningsResult(warnings);
var resultWithMultiple = products.ToSuccessWithWarningsResult(warning1, warning2, warning3);
```

#### **🌐 HTTP Integration & Status Mapping**

```csharp
// Comprehensive HTTP status code mapping
var error = DomainError.Validation("INVALID_INPUT", "Input validation failed");
var statusCode = error.GetHttpStatusCode(); // Returns 422 (Unprocessable Entity)

// Built-in error type mappings:
ErrorType.Validation      → 422 Unprocessable Entity
ErrorType.NotFound        → 404 Not Found
ErrorType.Unauthorized    → 401 Unauthorized
ErrorType.Forbidden       → 403 Forbidden
ErrorType.Conflict        → 409 Conflict
ErrorType.Internal        → 500 Internal Server Error
ErrorType.External        → 502 Bad Gateway
ErrorType.RateLimited     → 429 Too Many Requests
ErrorType.Timeout         → 408 Request Timeout
// ... and more

// API Response integration
var apiResponse = error.ToApiResponse<ProductDto>();
var resultResponse = result.ToApiResponse("Operation completed successfully");
```

#### **🎨 Real-World Usage Examples**

```csharp
// ✅ Simple validation with current API
public Result<User> CreateUser(string name, string email)
{
    if (string.IsNullOrWhiteSpace(name))
        return DomainError.Validation("NAME_REQUIRED", "Name is required").ToResult<User>();
        
    if (!IsValidEmail(email))
        return DomainError.Validation("EMAIL_INVALID", "Invalid email format").ToResult<User>();
        
    return new User { Name = name, Email = email }.ToResult();
}

// ✅ Complex business logic with multiple validation
public async Task<Result<Order, DomainErrors>> ProcessOrderAsync(OrderRequest request)
{
    var validationErrors = new List<DomainError>();
    
    // Validate customer
    var customer = await _customerService.GetByIdAsync(request.CustomerId);
    if (customer is null)
        validationErrors.Add(DomainError.NotFound("CUSTOMER_NOT_FOUND", "Customer not found"));
    
    // Validate products
    foreach (var item in request.Items)
    {
        var product = await _productService.GetByIdAsync(item.ProductId);
        if (product is null)
            validationErrors.Add(DomainError.NotFound("PRODUCT_NOT_FOUND", $"Product {item.ProductId} not found"));
        else if (product.Stock < item.Quantity)
            validationErrors.Add(DomainError.Conflict("INSUFFICIENT_STOCK", $"Not enough stock for {product.Name}"));
    }
    
    if (validationErrors.Count > 0)
        return validationErrors.ToFailureResult<Order>();
        
    // Process order
    var order = new Order
    {
        CustomerId = request.CustomerId,
        Items = request.Items,
        Status = BusinessStatus.Active
    };
    
    return Result<Order, DomainErrors>.Success(await _orderRepository.CreateAsync(order));
}

// ✅ Functional composition for complex workflows
public async Task<Result<InvoiceDto>> GenerateInvoiceAsync(int orderId, CancellationToken ct = default)
{
    return await GetOrderAsync(orderId)
        .MapAsync(order => ValidateOrderForInvoicingAsync(order))
        .MapAsync(order => CalculateInvoiceAmountsAsync(order))
        .MapAsync(async (invoice, token) => await ApplyTaxCalculationsAsync(invoice, token), ct)
        .MapAsync(invoice => GeneratePdfAsync(invoice))
        .Map(invoice => ConvertToDto(invoice))
        .OnSuccess(invoice => _logger.LogInformation("Invoice generated: {InvoiceId}", invoice.Id))
        .OnFailure(error => _logger.LogError("Invoice generation failed: {Error}", error));
}
```

#### **🎯 Current Best Practices**

```csharp
// ✅ DO: Use Result factory methods or extension helpers
return Result<User>.Failure(DomainError.NotFound("USER_NOT_FOUND", $"User with ID {id} was not found"));
// OR
return DomainError.NotFound("USER_NOT_FOUND", $"User with ID {id} was not found").ToResult<User>();

// ✅ DO: Use pattern matching and deconstruction
var (isSuccess, user, error) = result;
if (isSuccess)
    ProcessUser(user!);

// ✅ DO: Use TryGet methods for safe access
if (result.TryGetValue(out var user))
    ProcessUser(user);

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
    ? errors.ToFailureResult<User>()
    : Result<User>.Success(CreateUser(name, email));
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
    { Length: 0 } => DomainError.Validation("EMPTY_INPUT", "Input cannot be empty").ToResult<ProcessedData>(),
    { Length: > 100 } => DomainError.Validation("TOO_LONG", "Input too long").ToResult<ProcessedData>(),
    _ when !DataValidation.IsValidEmail(input) => DomainError.Validation("INVALID_EMAIL", "Invalid email format").ToResult<ProcessedData>(),
    _ => ProcessInput(input)
};
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
    var error = DomainError.Validation("JSON_DESERIALIZE_ERROR", ex.Message);
    return error.ToResult<MyType>();
}

// Safe Deserialization with Fallback
var obj = jsonString.DeserializeSafe<MyType>(fallback: new MyType());

// Deep Cloning via JSON
var clone = myObject.CloneDeep(); // Creates deep copy via JSON serialization
```

### 🧩 **Powerful Extension Methods**

#### **Result Extensions**
```csharp
public static class ResultExtensions
{
    // Create Results from values and errors
    public static Result<T> ToResult<T>(this T value);
    public static Result<T> ToResult<T>(this DomainError error);
    public static Result<T, DomainErrors> ToFailureResult<T>(this DomainErrors errors);
    public static Result<T, DomainErrors> ToFailureResult<T>(this IEnumerable<DomainError> errors);
    
    // Success with warnings helpers
    public static Result<SuccessWithWarnings<T>> ToSuccessWithWarningsResult<T>(this T value, DomainWarnings warnings);
    public static Result<SuccessWithWarnings<T>> ToSuccessWithWarningsResult<T>(this T value, params DomainError[] warnings);
    
    // Fluent factory methods for common error types
    public static Result<T> ValidationError<T>(string code, string message, string? target = null);
    public static Result<T> NotFoundError<T>(string code, string message, string? target = null);
    public static Result<T> ConflictError<T>(string code, string message, string? target = null);
    public static Result<T> UnauthorizedError<T>(string code, string message, string? target = null);
}
```

#### **Other Extension Methods**
```csharp
// Nullable Extensions
public static class NullableExtensions
{
    public static bool IsNull<T>(this T? value) where T : class;
    public static bool IsNotNull<T>(this T? value) where T : class;
    public static T OrDefault<T>(this T? value, T defaultValue) where T : class;
    public static T OrThrow<T>(this T? value, Exception exception) where T : class;
}

// Enum Extensions
public static class EnumExtensions
{
    public static string DisplayName(this Enum value); // Gets Description attribute or ToString()
}

// Domain Error Extensions
public static class DomainErrorExtensions
{
    public static ApiResponse<T> ToApiResponse<T>(this DomainError error, string? correlationId = null);
    public static ApiResponse<T> ToApiResponse<T>(this Result<T> result, string? correlationId = null);
    public static HttpStatusCode GetHttpStatusCode(this DomainError error);
    public static string GetAggregateErrorMessage(this DomainErrors errors);
}
```

### 📚 Constants & Helpers

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

## 📖 Documentation

For detailed implementation guides and best practices, see:
- [Domain Error & Result Usage Guide](docs/DomainError-Result-Usage-Guide.md)
- [API Integration Examples](docs/api-integration-examples.md)
- [Performance Best Practices](docs/performance-guide.md)

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
