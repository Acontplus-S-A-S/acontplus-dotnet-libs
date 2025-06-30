using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Acontplus.ApiDocumentation;

/// <summary>
/// Extension methods for configuring API versioning and Swagger documentation.
/// </summary>
public static class ApiDocumentationExtensions
{
    /// <summary>
    /// Adds and configures API versioning and Swagger/OpenAPI documentation services.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <returns>The configured IServiceCollection.</returns>
    public static IServiceCollection AddApiVersioningAndDocumentation(this IServiceCollection services)
    {
        // 1. Configure API Versioning
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            // Combine multiple version readers for flexibility
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("X-Api-Version"),
                new MediaTypeApiVersionReader("x-api-version")
            );
        }).AddApiExplorer(options =>
        {
            // Format the version as "vX" (e.g., "v1", "v2")
            options.GroupNameFormat = "'v'V";
            // Automatically substitute the API version in route templates
            options.SubstituteApiVersionInUrl = true;
        });

        // 2. Add our custom options configurator for Swagger
        services.ConfigureOptions<ConfigureSwaggerOptions>();

        // 3. Configure Swagger Generator
        services.AddSwaggerGen(options =>
        {
            // Enable JWT Bearer token authentication in the Swagger UI
            var securitySchema = new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            };

            options.AddSecurityDefinition("Bearer", securitySchema);

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { securitySchema, ["Bearer"] }
            });

            // Include XML comments from all assemblies in the base directory
            var baseDirectory = AppContext.BaseDirectory;
            var xmlFiles = Directory.EnumerateFiles(baseDirectory, "*.xml", SearchOption.TopDirectoryOnly);
            foreach (var xmlFile in xmlFiles)
            {
                options.IncludeXmlComments(xmlFile);
            }
        });

        return services;
    }

    /// <summary>
    /// Configures the application pipeline to use Swagger and the Swagger UI with versioning support.
    /// </summary>
    /// <param name="app">The IApplicationBuilder to configure.</param>
    /// <returns>The configured IApplicationBuilder.</returns>
    public static IApplicationBuilder UseApiVersioningAndDocumentation(this IApplicationBuilder app)
    {
        // Enable middleware to serve generated Swagger as a JSON endpoint.
        app.UseSwagger();

        // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
        // specifying the Swagger JSON endpoint for each API version.
        app.UseSwaggerUI(options =>
        {
            var apiVersionDescriptionProvider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();

            // Build a swagger endpoint for each discovered API version, showing the latest versions first.
            foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions.Reverse())
            {
                var url = $"/swagger/{description.GroupName}/swagger.json";
                var name = description.GroupName.ToUpperInvariant();
                options.SwaggerEndpoint(url, name);
            }
        });

        return app;
    }
}
