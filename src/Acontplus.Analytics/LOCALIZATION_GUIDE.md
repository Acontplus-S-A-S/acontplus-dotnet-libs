# Localization Guide

## Why Localization is NOT Included in the Library

The `Acontplus.Analytics` package **intentionally does not include built-in localization** for the following reasons:

### 1. **Business-Specific Labels**
Each business domain has unique terminology:
- Restaurant: "Table Turnover", "Average Preparation Time", "Tips"
- E-commerce: "Cart Abandonment", "Conversion Rate", "Product Views"
- Healthcare: "Patient Satisfaction", "Appointment Duration", "Treatment Success"

### 2. **Custom Properties**
Applications extend base DTOs with custom properties that require domain-specific labels:

```csharp
public class RestaurantDashboardDto : BaseDashboardStatsDto
{
    public decimal TableTurnoverRate { get; set; }  // Unique to restaurants
    public int TotalTips { get; set; }               // Unique to restaurants
}
```

The library cannot predict what labels these custom properties need.

### 3. **Integration with Existing Systems**
Applications may already have localization infrastructure:
- **.resx files** (traditional .NET resources)
- **Database-driven** localization tables
- **JSON files** for i18n frameworks
- **Third-party libraries** (e.g., ResX Manager, Localization.AspNetCore)
- **Content Management Systems** with multilingual support

### 4. **Separation of Concerns**
The library's responsibility is **data structure and retrieval**, not presentation:
- **Library Layer**: Provides DTOs with optional `Labels` property
- **Application Layer**: Populates labels based on business needs and user preferences

### 5. **Flexibility and Control**
Applications have complete control over:
- Which languages to support
- How labels are stored and retrieved
- When and how labels are applied
- Label naming conventions
- Fallback strategies

## How to Implement Localization in Your Application

### Option 1: Static Dictionary Classes

```csharp
public class RestaurantStatisticsLocalization
{
    public static Dictionary<string, string> GetLabels(string language) => language switch
    {
        "es" => GetSpanishLabels(),
        "en" => GetEnglishLabels(),
        _ => GetEnglishLabels()
    };

    private static Dictionary<string, string> GetSpanishLabels() => new()
    {
        { "TotalRevenue", "Ingresos Totales" },
        { "NetRevenue", "Ingresos Netos" },
        { "TableTurnoverRate", "Tasa de Rotación de Mesas" },
        { "TotalTips", "Propinas Totales" }
    };

    private static Dictionary<string, string> GetEnglishLabels() => new()
    {
        { "TotalRevenue", "Total Revenue" },
        { "NetRevenue", "Net Revenue" },
        { "TableTurnoverRate", "Table Turnover Rate" },
        { "TotalTips", "Total Tips" }
    };
}
```

### Option 2: Resource Files (.resx)

```csharp
public class LocalizationService
{
    private readonly IStringLocalizer<StatisticsResources> _localizer;

    public Dictionary<string, string> GetDashboardLabels()
    {
        return new Dictionary<string, string>
        {
            { "TotalRevenue", _localizer["TotalRevenue"].Value },
            { "NetRevenue", _localizer["NetRevenue"].Value },
            { "TableTurnoverRate", _localizer["TableTurnoverRate"].Value }
        };
    }
}
```

### Option 3: Database-Driven

```csharp
public class DbLocalizationService
{
    private readonly IRepository<Translation> _translationRepo;

    public async Task<Dictionary<string, string>> GetLabelsAsync(
        string module,
        string language)
    {
        var translations = await _translationRepo.GetAllAsync(
            t => t.Module == module && t.Language == language);

        return translations.ToDictionary(t => t.Key, t => t.Value);
    }
}
```

### Option 4: JSON Files

```json
// locales/es.json
{
  "statistics": {
    "totalRevenue": "Ingresos Totales",
    "netRevenue": "Ingresos Netos",
    "tableTurnoverRate": "Tasa de Rotación de Mesas"
  }
}
```

```csharp
public class JsonLocalizationService
{
    public Dictionary<string, string> LoadLabels(string language)
    {
        var json = File.ReadAllText($"locales/{language}.json");
        var data = JsonSerializer.Deserialize<LocalizationData>(json);
        return data.Statistics;
    }
}
```

## Applying Localization

### In Your Service Layer

```csharp
public class RestaurantStatisticsApplicationService
{
    private readonly IStatisticsService<DashboardDto, RealTimeDto, AggregatedDto, TrendDto> _statsService;
    private readonly ILocalizationService _localizationService;

    public async Task<Result<DashboardDto, DomainError>> GetLocalizedDashboard(
        FilterRequest filter,
        string language)
    {
        var result = await _statsService.GetDashboardStatsAsync(filter);

        if (result.IsSuccess)
        {
            // Apply localization after retrieving data
            result.Value.Labels = _localizationService.GetDashboardLabels(language);
        }

        return result;
    }
}
```

### In Your Controller

```csharp
[HttpGet("dashboard")]
public async Task<IActionResult> GetDashboard(
    [FromQuery] DateTime startDate,
    [FromQuery] DateTime endDate,
    [FromHeader(Name = "Accept-Language")] string language = "en")
{
    var filter = new FilterRequest { /* ... */ };

    var result = await _statsService.GetDashboardStatsAsync(filter);

    if (result.IsSuccess)
    {
        // Apply localization based on Accept-Language header
        result.Value.Labels = RestaurantStatisticsLocalization.GetLabels(language);
    }

    return result.Match(
        success => Ok(success),
        error => BadRequest(error)
    );
}
```

### Using Middleware

```csharp
public class StatisticsLocalizationMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Set culture from header
        var language = context.Request.Headers["Accept-Language"].FirstOrDefault() ?? "en";
        var culture = new CultureInfo(language);

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        await _next(context);
    }
}
```

## Best Practices

### ✅ DO

- **Implement localization in your application layer** where you have business context
- **Use your organization's existing localization strategy** for consistency
- **Extend base DTOs** with domain-specific properties and their labels
- **Apply labels after data retrieval** to keep service layer clean
- **Use Accept-Language header** or user preferences to determine language

### ❌ DON'T

- **Don't expect the library to provide all labels** - it doesn't know your business domain
- **Don't hard-code labels in DTOs** - keep them flexible via the Labels dictionary
- **Don't mix data retrieval with localization** - separate concerns
- **Don't limit yourself to predefined languages** - support what your business needs

## Benefits of This Approach

| Benefit | Description |
|---------|-------------|
| **Flexibility** | Use any localization system (RESX, JSON, DB, etc.) |
| **Domain-Specific** | Labels match your exact business terminology |
| **No Lock-In** | Not forced into a specific i18n framework |
| **Maintainability** | Localization changes don't require library updates |
| **Extensibility** | Add new languages without touching the library |
| **Consistency** | Integrate with organization-wide localization strategy |

## Summary

The `Acontplus.Analytics` library provides the **data structure** (DTOs with a `Labels` property), and **you provide the content** (business-specific translations). This separation ensures the library remains flexible, reusable, and non-opinionated about localization strategies.

**Your application = Your languages, Your labels, Your way.**
