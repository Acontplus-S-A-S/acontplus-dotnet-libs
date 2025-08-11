using Acontplus.Services.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace Acontplus.Services.Extensions.Security;

public static class SecurityHeaderPolicyExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app, IWebHostEnvironment environment)
    {
        var policyCollection = new HeaderPolicyCollection()
            .AddFrameOptionsDeny()
            .AddContentTypeOptionsNoSniff()
            .AddReferrerPolicyStrictOriginWhenCrossOrigin()
            .RemoveServerHeader();

        if (!environment.IsDevelopment())
        {
            // Only add HSTS in production
            policyCollection.AddStrictTransportSecurityMaxAgeIncludeSubDomains(maxAgeInSeconds: 60 * 60 * 24 * 365); // 1 year
        }

        // Get CSP configuration from settings
        var cspConfig = app.ApplicationServices.GetService<IOptions<RequestContextConfiguration>>()?.Value?.Csp
            ?? new CspConfiguration();

        // Configure CSP based on environment and configuration
        var useStrictCsp = app.ApplicationServices.GetService<IConfiguration>()?
            .GetValue<bool>("Security:UseStrictCSP") ?? false;

        if (useStrictCsp)
        {
            ConfigureStrictCSP(policyCollection, environment, cspConfig);
        }
        else
        {
            ConfigurePermissiveCSP(policyCollection, environment, cspConfig);
        }

        return app.UseSecurityHeaders(policyCollection);
    }

    private static void ConfigurePermissiveCSP(HeaderPolicyCollection policyCollection, IWebHostEnvironment environment, CspConfiguration cspConfig)
    {
        // Permissive CSP for Angular applications with Google Fonts support
        policyCollection.AddContentSecurityPolicy(builder =>
        {
            builder.AddDefaultSrc().Self();
            builder.AddObjectSrc().None();

            // Allow configured script sources and inline scripts
            var scriptSrc = builder.AddScriptSrc().Self().UnsafeInline().UnsafeEval(); // Required for Angular
            foreach (var source in cspConfig.AllowedScriptSources)
            {
                scriptSrc.From(source);
            }

            // Allow configured style sources and inline styles
            var styleSrc = builder.AddStyleSrc().Self().UnsafeInline(); // Required for Angular inline styles
            foreach (var source in cspConfig.AllowedStyleSources)
            {
                styleSrc.From(source);
            }

            // Allow configured image sources and data URLs
            var imgSrc = builder.AddImgSrc().Self().Data();
            foreach (var source in cspConfig.AllowedImageSources)
            {
                imgSrc.From(source);
            }

            // Allow configured font sources and data URLs
            var fontSrc = builder.AddFontSrc().Self().Data();
            foreach (var source in cspConfig.AllowedFontSources)
            {
                fontSrc.From(source);
            }

            // Allow configured connect sources
            var connectSrc = builder.AddConnectSrc().Self();
            foreach (var source in cspConfig.AllowedConnectSources)
            {
                connectSrc.From(source);
            }

            // Allow configured media sources
            var mediaSrc = builder.AddMediaSrc().Self();
            foreach (var source in cspConfig.AllowedMediaSources)
            {
                mediaSrc.From(source);
            }

            // Allow configured frame sources instead of blocking all
            var frameSrc = builder.AddFrameSrc().Self();
            foreach (var source in cspConfig.AllowedFrameSources)
            {
                frameSrc.From(source);
            }

            // Allow configured base URI sources
            var baseUri = builder.AddBaseUri().Self();
            foreach (var source in cspConfig.AllowedBaseUriSources)
            {
                baseUri.From(source);
            }

            // Allow configured form action sources
            var formAction = builder.AddFormAction().Self();
            foreach (var source in cspConfig.AllowedFormActionSources)
            {
                formAction.From(source);
            }
        });
    }

    private static void ConfigureStrictCSP(HeaderPolicyCollection policyCollection, IWebHostEnvironment environment, CspConfiguration cspConfig)
    {
        // Strict CSP using nonces with Google Fonts support (future implementation)
        policyCollection.AddContentSecurityPolicy(builder =>
        {
            builder.AddDefaultSrc().Self();
            builder.AddObjectSrc().None();

            // Allow configured script sources with nonces
            var scriptSrc = builder.AddScriptSrc().Self().WithNonce(); // Use nonces instead of unsafe-inline
            foreach (var source in cspConfig.AllowedScriptSources)
            {
                scriptSrc.From(source);
            }

            // Allow configured style sources with nonces
            var styleSrc = builder.AddStyleSrc().Self().WithNonce(); // Use nonces for styles
            foreach (var source in cspConfig.AllowedStyleSources)
            {
                styleSrc.From(source);
            }

            // Allow configured image sources and data URLs
            var imgSrc = builder.AddImgSrc().Self().Data();
            foreach (var source in cspConfig.AllowedImageSources)
            {
                imgSrc.From(source);
            }

            // Allow configured font sources and data URLs
            var fontSrc = builder.AddFontSrc().Self().Data();
            foreach (var source in cspConfig.AllowedFontSources)
            {
                fontSrc.From(source);
            }

            // Allow configured connect sources
            var connectSrc = builder.AddConnectSrc().Self();
            foreach (var source in cspConfig.AllowedConnectSources)
            {
                connectSrc.From(source);
            }

            // Allow configured media sources
            var mediaSrc = builder.AddMediaSrc().Self();
            foreach (var source in cspConfig.AllowedMediaSources)
            {
                mediaSrc.From(source);
            }

            // Allow configured frame sources instead of blocking all
            var frameSrc = builder.AddFrameSrc().Self();
            foreach (var source in cspConfig.AllowedFrameSources)
            {
                frameSrc.From(source);
            }

            // Allow configured base URI sources
            var baseUri = builder.AddBaseUri().Self();
            foreach (var source in cspConfig.AllowedBaseUriSources)
            {
                baseUri.From(source);
            }

            // Allow configured form action sources
            var formAction = builder.AddFormAction().Self();
            foreach (var source in cspConfig.AllowedFormActionSources)
            {
                formAction.From(source);
            }
        });
    }
}
