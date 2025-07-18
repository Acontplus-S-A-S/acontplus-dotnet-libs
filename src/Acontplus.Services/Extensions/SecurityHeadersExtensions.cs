using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Acontplus.Services.Extensions;


public static class SecurityHeadersExtensions
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

        // Configure CSP based on environment and configuration
        var useStrictCsp = app.ApplicationServices.GetService<IConfiguration>()?
            .GetValue<bool>("Security:UseStrictCSP") ?? false;

        if (useStrictCsp)
        {
            ConfigureStrictCSP(policyCollection, environment);
        }
        else
        {
            ConfigurePermissiveCSP(policyCollection, environment);
        }

        return app.UseSecurityHeaders(policyCollection);
    }

    private static void ConfigurePermissiveCSP(HeaderPolicyCollection policyCollection, IWebHostEnvironment environment)
    {
        // Permissive CSP for Angular applications with Google Fonts support
        policyCollection.AddContentSecurityPolicy(builder =>
        {
            builder.AddDefaultSrc().Self();
            builder.AddObjectSrc().None();
            builder.AddScriptSrc().Self().UnsafeInline().UnsafeEval(); // Required for Angular

            // Allow Google Fonts stylesheets and inline styles
            builder.AddStyleSrc().Self()
                .UnsafeInline() // Required for Angular inline styles
                .From("https://fonts.googleapis.com"); // Google Fonts CSS

            builder.AddImgSrc().Self().Data(); // Allow data URLs for images

            // Allow Google Fonts and Material Icons
            builder.AddFontSrc().Self()
                .Data() // Allow data URLs for fonts
                .From("https://fonts.gstatic.com"); // Google Fonts files

            builder.AddConnectSrc().Self(); // Allow API calls to same origin
            builder.AddMediaSrc().Self();
            builder.AddFrameSrc().None(); // Prevent embedding in frames
            builder.AddBaseUri().Self();
            builder.AddFormAction().Self(); // Only allow forms to submit to same origin
        });
    }

    private static void ConfigureStrictCSP(HeaderPolicyCollection policyCollection, IWebHostEnvironment environment)
    {
        // Strict CSP using nonces with Google Fonts support (future implementation)
        policyCollection.AddContentSecurityPolicy(builder =>
        {
            builder.AddDefaultSrc().Self();
            builder.AddObjectSrc().None();
            builder.AddScriptSrc().Self().WithNonce(); // Use nonces instead of unsafe-inline

            // Allow Google Fonts stylesheets with nonces
            builder.AddStyleSrc().Self()
                .WithNonce() // Use nonces for styles
                .From("https://fonts.googleapis.com"); // Google Fonts CSS

            builder.AddImgSrc().Self().Data(); // Allow data URLs for images

            // Allow Google Fonts and Material Icons
            builder.AddFontSrc().Self()
                .Data() // Allow data URLs for fonts
                .From("https://fonts.gstatic.com"); // Google Fonts files

            builder.AddConnectSrc().Self(); // Allow API calls to same origin
            builder.AddMediaSrc().Self();
            builder.AddFrameSrc().None(); // Prevent embedding in frames
            builder.AddBaseUri().Self();
            builder.AddFormAction().Self(); // Only allow forms to submit to same origin
        });
    }
}
