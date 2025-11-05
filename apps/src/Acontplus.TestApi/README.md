# Acontplus.TestApi

A comprehensive test and demonstration API for the Acontplus .NET libraries. This project showcases all available libraries with organized, well-documented examples.

## Project Structure

### Controllers Organization

The controllers are organized by functional domains:

```
Controllers/
├── Core/                    # Core system functionality
│   ├── BaseApiController.cs # Base controller with common functionality
│   ├── SecurityController.cs # Security-related endpoints
│   └── EncryptionController.cs # Data encryption demonstrations
├── Business/                # Business logic controllers
│   ├── UsuarioController.cs # User management
│   ├── DocumentoElectronicoController.cs # Electronic document processing
│   ├── AtsController.cs     # ATS (Tax Authority System) operations
│   └── ReportsController.cs # Report generation
├── Infrastructure/          # Infrastructure services
│   ├── BarcodeController.cs # Barcode generation
│   ├── PrintController.cs   # Printing services
│   ├── EmailController.cs   # Email services
│   └── ConfigurationTestController.cs # Configuration testing
└── Demo/                    # Demonstration and examples
    ├── WeatherForecastController.cs # Basic API example
    ├── SimpleModelController.cs     # Simple CRUD operations
    ├── AdvancedExamplesController.cs # Advanced usage patterns
    ├── AdvancedUsageController.cs   # Comprehensive service demonstrations
    └── LibraryShowcaseController.cs # All libraries integration
```

### Endpoints Organization

```
Endpoints/
├── Api/                     # General API endpoints
│   └── AdvancedEndpoints.cs # Advanced service demonstrations
├── Business/                # Business-specific endpoints
│   └── Customer/
│       └── IdentificationEndpoints.cs # Customer identification services
└── EndpointGroups.cs        # Endpoint organization utilities
```

### Models Organization

```
Models/
├── WeatherForecast.cs       # Basic models
├── Requests/                # Request DTOs
├── Responses/               # Response DTOs
└── DTOs/                    # Data transfer objects
```

### Extensions Organization

```
Extensions/
├── AddApplicationServices.cs # Service registration
└── Program/                 # Program configuration extensions
    ├── ServiceRegistrationExtensions.cs    # Service registration
    ├── MiddlewareConfigurationExtensions.cs # Middleware setup
    └── EndpointMappingExtensions.cs        # Endpoint mapping
```

## Library Coverage

This API comprehensively demonstrates all Acontplus libraries:

### Core Libraries

- **Acontplus.Services**: Caching, circuit breakers, request context, device detection, security headers
- **Acontplus.Core**: Domain models, enums, extensions, validation, persistence abstractions
- **Acontplus.Logging**: Structured logging, custom enrichers, advanced configuration
- **Acontplus.Utilities**: String extensions, security utilities, data encryption, validation helpers

### Business Libraries

- **Acontplus.Barcode**: QR code and barcode generation
- **Acontplus.Billing**: Document processing, tax calculations, electronic invoicing
- **Acontplus.Reports**: RDLC report generation, PDF creation, print services
- **Acontplus.Notifications**: Email and SMS services, notification templates

### Infrastructure Libraries

- **Acontplus.Persistence**: Database operations, Entity Framework integration
- **Acontplus.ApiDocumentation**: OpenAPI/Swagger configuration, versioning

## API Patterns Demonstrated

### 1. Traditional MVC Controllers
- Full controller classes with actions
- Model binding and validation
- View models and responses

### 2. Minimal API Endpoints
- Route handlers with lambda expressions
- Direct response handling
- Minimal configuration

### 3. Hybrid Approaches
- Controller-based minimal APIs
- Mixed routing strategies
- Flexible response formats

## Key Features

### Comprehensive Service Integration
- Dependency injection with Scrutor assembly scanning
- Service lifetime management
- Cross-cutting concerns (logging, security, caching)

### Advanced Configuration
- Serilog structured logging
- Environment-specific settings
- OpenAPI documentation
- Health checks and monitoring

### Security Features
- JWT authentication
- Authorization policies
- Data encryption
- Password hashing and validation

### Performance Optimizations
- Response caching
- Circuit breaker patterns
- Database connection pooling
- Optimized JSON serialization

## Usage Examples

### Basic API Call
```bash
GET /WeatherForecast
```

### Advanced Service Demonstration
```bash
GET /api/AdvancedUsage/advanced/dashboard
```

### Library Showcase
```bash
POST /api/LibraryShowcase/comprehensive/user-registration
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phoneNumber": "+1234567890",
  "password": "SecurePass123!"
}
```

### Barcode Generation
```bash
GET /api/Infrastructure/Barcode?text=HelloWorld&includeLabel=true
```

### Report Generation
```bash
GET /api/Business/Reports/sample-invoice
```

## Development

### Building
```bash
dotnet build
```

### Running
```bash
dotnet run
```

### Testing
```bash
dotnet test
```

### API Documentation
When running in development mode, visit:
- Swagger UI: `https://localhost:5001/swagger`
- OpenAPI JSON: `https://localhost:5001/openapi/v1.json`

## Health Checks

- `/health` - Overall health status
- `/health/ready` - Readiness probe
- `/health/live` - Liveness probe

## Configuration

The API supports multiple environments with configuration in `appsettings.json` and environment-specific overrides.

Key configuration sections:
- `Logging` - Serilog configuration
- `ConnectionStrings` - Database connections
- `Services` - External service configurations
- `Security` - Security settings
- `Notifications` - Email/SMS provider settings

## Contributing

This is a demonstration project for the Acontplus libraries. For contributions to the libraries themselves, please refer to the main repository documentation.
