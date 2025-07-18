# Acontplus.Utilities

[![NuGet](https://img.shields.io/nuget/v/Acontplus.Utilities.svg)](https://www.nuget.org/packages/Acontplus.Utilities)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

A comprehensive .NET 9+ utility library providing common functionality for enterprise applications. Modern async, extension methods, minimal API support, and more.

## 🚀 Features

- **Encryption** - Data encryption/decryption utilities
- **External Validations** - Third-party validation integrations
- **Barcode Generation** - Barcode creation and processing
- **Custom Logging** - Enhanced logging capabilities
- **Enum Extensions** - Enhanced enum functionality
- **Picture Helper** - Image processing utilities
- **Text Handlers** - Text manipulation and processing
- **API Response Extensions** - Convert results to `IActionResult` or `IResult` for MVC/Minimal APIs
- **Pagination & Metadata** - Helpers for API metadata, pagination, and diagnostics

## 📦 Installation

### NuGet Package Manager
```bash
Install-Package Acontplus.Utilities
```

### .NET CLI
```bash
dotnet add package Acontplus.Utilities
```

### PackageReference
```xml
<ItemGroup>
  <PackageReference Include="Acontplus.Utilities" Version="1.0.12" />
</ItemGroup>
```

## 🎯 Quick Start

### 1. API Response Extensions (Minimal API)
```csharp
using Acontplus.Utilities.Extensions;

app.MapGet("/example", () =>
{
    var response = new ApiResponse<string>("Hello World");
    return response.ToMinimalApiResult();
});
```

### 2. API Response Extensions (Controller)
```csharp
using Acontplus.Utilities.Extensions;

public IActionResult Get()
{
    var response = new ApiResponse<string>("Hello World");
    return response.ToActionResult();
}
```

### 3. Encryption Example
```csharp
var encryptionService = new SensitiveDataEncryptionService();
byte[] encrypted = await encryptionService.EncryptToBytesAsync("password", "data");
string decrypted = await encryptionService.DecryptFromBytesAsync("password", encrypted);
```

### 4. Pagination Metadata Example
```csharp
var metadata = new Dictionary<string, object>()
    .WithPagination(page: 1, pageSize: 10, totalItems: 100);
```

## 🗃️ Data Utilities

### DataConverters
```csharp
using Acontplus.Utilities.Data;

// Convert DataTable to JSON
string json = DataConverters.DataTableToJson(myDataTable);

// Convert DataSet to JSON
string json = DataConverters.DataSetToJson(myDataSet);

// Convert JSON to DataTable
DataTable table = DataConverters.JsonToDataTable(jsonString);

// Serialize any object (with DataTable/DataSet support)
string json = DataConverters.SerializeObjectCustom(myObject);

// Serialize and sanitize complex objects
string json = DataConverters.SerializeSanitizedData(myObject);
```

### DataTableMapper
```csharp
using Acontplus.Utilities.Data;

// Map a DataRow to a strongly-typed model
var model = DataTableMapper.MapDataRowToModel<MyModel>(dataRow);

// Map a DataTable to a list of models
List<MyModel> models = DataTableMapper.MapDataTableToList<MyModel>(dataTable);
```

## 🧩 JSON Utilities

### JsonHelper
```csharp
using Acontplus.Utilities.Json;

// Validate JSON
var result = JsonHelper.ValidateJson(jsonString);
if (!result.IsValid) Console.WriteLine(result.ErrorMessage);

// Get a property value from JSON
string? value = JsonHelper.GetJsonProperty<string>(jsonString, "propertyName");

// Merge two JSON objects
string merged = JsonHelper.MergeJson(json1, json2);

// Compare two JSON strings (ignoring property order)
bool areEqual = JsonHelper.AreEqual(json1, json2);
```

### JsonManipulationExtensions
```csharp
using Acontplus.Utilities.Json;

// Validate JSON using extension
bool isValid = jsonString.IsValidJson();

// Get property value using extension
int? id = jsonString.GetJsonProperty<int>("id");

// Merge JSON using extension
string merged = json1.MergeJson(json2);

// Compare JSON using extension
bool equal = json1.JsonEquals(json2);
```

## 🔄 Object Mapping

### ObjectMapper
```csharp
using Acontplus.Utilities.Mapping;

// Map between objects (AutoMapper-like)
var target = ObjectMapper.Map<SourceType, TargetType>(sourceObject);

// Configure custom mapping
ObjectMapper.CreateMap<SourceType, TargetType>()
    .ForMember(dest => dest.SomeProperty, src => src.OtherProperty)
    .Ignore(dest => dest.IgnoredProperty);

// Map with configuration
var mapped = ObjectMapper.Map<SourceType, TargetType>(sourceObject);
```

## 🔧 Advanced Usage

### File Name Sanitization
```csharp
string safeName = FileExtensions.SanitizeFileName("my*illegal:file?.txt");
```

### Base64 Conversion
```csharp
string base64 = FileExtensions.GetBase64FromByte(myBytes);
```

### Compression Utilities
```csharp
byte[] compressed = CompressionUtils.CompressGZip(data);
byte[] decompressed = CompressionUtils.DecompressGZip(compressed);
```

## 📚 API Documentation

- `SensitiveDataEncryptionService` - AES encryption/decryption helpers
- `FileExtensions` - File name and byte array utilities
- `CompressionUtils` - GZip/Deflate compression helpers
- `TextHandlers` - String formatting and splitting
- `ApiResponseExtensions` - API response helpers for MVC/Minimal APIs
- `DirectoryHelper` / `EnvironmentHelper` - Runtime and environment utilities

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