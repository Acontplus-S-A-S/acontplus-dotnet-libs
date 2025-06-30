# Acontplus.Core

A comprehensive .NET utility library providing foundational components and common functionality for enterprise applications.

## Features

### Core Components
- **Base Entity System** - Foundational entity structures with common properties
- **Base DTO System** - Base classes for data transfer objects

### Utilities
- **Enum Extensions** - Helpful extension methods for working with enums
- **Nullable Extensions** - Extension methods for nullable types

### Validation
- **Data Validations** - Common validation logic
- **XML Validation** - XML validation utilities
- **Extreme Validations** - Specialized validation scenarios

### Logging
- **Error Logging** - Basic error logging infrastructure

### Reporting
- **Report Infrastructure** - Base reporting components

## Installation

```bash
dotnet add package Acontplus.Core
```

## Usage Examples

### Base Entity
```csharp
public class Product : BaseEntity
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```

### Enum Extensions
```csharp
public enum Status { Active, Inactive }

// Get display name or description
var displayName = Status.Active.GetDisplayName();

// Parse from string
var status = "Active".ToEnum<Status>();
```

### Data Validation
```csharp
var validator = new DataValidations();
if (!validator.IsValidEmail("test@example.com"))
{
    // Handle invalid email
}
```

## Documentation

Full API documentation is available at [https://github.com/Acontplus-S-A-S/acontplus-common/wiki]

## Contributing

We welcome contributions! Please follow these steps:
1. Fork the repository
2. Create a feature branch
3. Submit a pull request

See our [contribution guidelines]https://github.com/Acontplus-S-A-S/acontplus-dotnet-libs/blob/main/CONTRIBUTING.md) for more details.

## Support

For issues or questions:
- Open an issue on [GitHub]https://github.com/Acontplus-S-A-S/acontplus-dotnet-libs/issues)
- Email support@acontplus.com

## License

MIT License. See [LICENSE]https://github.com/Acontplus-S-A-S/acontplus-dotnet-libs/blob/main/LICENSE) for full details.

## Author

[Ivan Paz](https://linktr.ee/iferpaz7)

## Company

[Acontplus S.A.S.](https://acontplus.com.ec)