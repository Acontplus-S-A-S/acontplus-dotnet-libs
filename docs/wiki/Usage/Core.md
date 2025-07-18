# Acontplus.Core Usage Guide

Acontplus.Core provides foundational components for enterprise .NET applications, including DDD patterns, error handling, DTOs, and modern C# features.

## 📦 Installation

```bash
Install-Package Acontplus.Core
```

## 🚀 Features
- Domain-Driven Design (DDD) base entities and value objects
- Specification and repository patterns
- Functional error handling (Result pattern)
- Structured API responses and pagination
- Validation utilities (XML, JSON, data)
- Modern .NET 9+ features (nullable, pattern matching, source generators)

## 🛠️ Basic Usage

### Entity Example
```csharp
public class Customer : Entity<Guid>
{
    public string Name { get; set; }
    // ...
}
```

### Result Pattern
```csharp
var result = Result.Success();
if (result.IsFailure) { /* handle error */ }
```

### API Response
```csharp
var response = new ApiResponse<string>("Hello World");
```

## 📖 See Also
- [API Reference](../Home.md)
- [Project README](https://github.com/Acontplus-S-A-S/acontplus-dotnet-libs/blob/main/src/Acontplus.Core/README.md) 