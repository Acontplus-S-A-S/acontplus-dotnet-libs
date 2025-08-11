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
            builder.AddScriptSrc().Self().UnsafeInline().UnsafeEval(); // Required for Angular

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
            builder.AddMediaSrc().Self();
            builder.AddFrameSrc().None(); // Prevent embedding in frames
            builder.AddBaseUri().Self();
            builder.AddFormAction().Self(); // Only allow forms to submit to same origin
        });
    }

    private static void ConfigureStrictCSP(HeaderPolicyCollection policyCollection, IWebHostEnvironment environment, CspConfiguration cspConfig)
    {
        // Strict CSP using nonces with Google Fonts support (future implementation)
        policyCollection.AddContentSecurityPolicy(builder =>
        {
            builder.AddDefaultSrc().Self();
            builder.AddObjectSrc().None();
            builder.AddScriptSrc().Self().WithNonce(); // Use nonces instead of unsafe-inline

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
            builder.AddMediaSrc().Self();
            builder.AddFrameSrc().None(); // Prevent embedding in frames
            builder.AddBaseUri().Self();
            builder.AddFormAction().Self(); // Only allow forms to submit to same origin
        });
    }
}
