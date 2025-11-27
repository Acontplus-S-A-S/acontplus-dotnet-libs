using Acontplus.ApiDocumentation;
using Serilog;

public static class MiddlewareConfigurationExtensions
{
    public static void ConfigureTestApiMiddleware(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        // Use Serilog request logging BEFORE other middleware like UseRouting, UseAuthentication, etc.
        app.UseSerilogRequestLogging(); // Captures HTTP request/response details

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseApiVersioningAndDocumentation();
        }

        app.UseRouting();
        app.UseHttpsRedirection();
        app.UseAuthorization();

        // Controllers have been converted to Minimal API endpoints
        // app.MapControllers();
    }
}

