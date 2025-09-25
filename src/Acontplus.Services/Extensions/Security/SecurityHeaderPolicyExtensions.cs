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
        policyCollection.AddContentSecurityPolicy(builder =>
        {
            builder.AddDefaultSrc().Self();
            builder.AddObjectSrc().None();

            // Configure script sources with unsafe-inline and unsafe-eval for Angular
            var scriptSrc = builder.AddScriptSrc().Self().UnsafeInline().UnsafeEval();
            AddConfiguredSources(scriptSrc, cspConfig.AllowedScriptSources);

            // Configure style sources with unsafe-inline for Angular inline styles
            var styleSrc = builder.AddStyleSrc().Self().UnsafeInline();
            AddConfiguredSources(styleSrc, cspConfig.AllowedStyleSources);

            // Configure image sources with data URLs
            var imgSrc = builder.AddImgSrc().Self().Data();
            AddConfiguredSources(imgSrc, cspConfig.AllowedImageSources);

            // Configure font sources with data URLs
            var fontSrc = builder.AddFontSrc().Self().Data();
            AddConfiguredSources(fontSrc, cspConfig.AllowedFontSources);

            // Configure connect sources
            var connectSrc = builder.AddConnectSrc().Self();
            AddConfiguredSources(connectSrc, cspConfig.AllowedConnectSources);

            // Configure media sources
            var mediaSrc = builder.AddMediaSrc().Self();
            AddConfiguredSources(mediaSrc, cspConfig.AllowedMediaSources);

            // Configure frame sources
            var frameSrc = builder.AddFrameSrc().Self();
            AddConfiguredSources(frameSrc, cspConfig.AllowedFrameSources);

            // Configure base URI sources
            var baseUri = builder.AddBaseUri().Self();
            AddConfiguredSources(baseUri, cspConfig.AllowedBaseUriSources);

            // Configure form action sources
            var formAction = builder.AddFormAction().Self();
            AddConfiguredSources(formAction, cspConfig.AllowedFormActionSources);
        });
    }

    private static void ConfigureStrictCSP(HeaderPolicyCollection policyCollection, IWebHostEnvironment environment, CspConfiguration cspConfig)
    {
        policyCollection.AddContentSecurityPolicy(builder =>
        {
            builder.AddDefaultSrc().Self();
            builder.AddObjectSrc().None();

            // Configure script sources with nonces instead of unsafe-inline
            var scriptSrc = builder.AddScriptSrc().Self().WithNonce();
            AddConfiguredSources(scriptSrc, cspConfig.AllowedScriptSources);

            // Configure style sources with nonces
            var styleSrc = builder.AddStyleSrc().Self().WithNonce();
            AddConfiguredSources(styleSrc, cspConfig.AllowedStyleSources);

            // Configure image sources with data URLs
            var imgSrc = builder.AddImgSrc().Self().Data();
            AddConfiguredSources(imgSrc, cspConfig.AllowedImageSources);

            // Configure font sources with data URLs
            var fontSrc = builder.AddFontSrc().Self().Data();
            AddConfiguredSources(fontSrc, cspConfig.AllowedFontSources);

            // Configure connect sources
            var connectSrc = builder.AddConnectSrc().Self();
            AddConfiguredSources(connectSrc, cspConfig.AllowedConnectSources);

            // Configure media sources
            var mediaSrc = builder.AddMediaSrc().Self();
            AddConfiguredSources(mediaSrc, cspConfig.AllowedMediaSources);

            // Configure frame sources
            var frameSrc = builder.AddFrameSrc().Self();
            AddConfiguredSources(frameSrc, cspConfig.AllowedFrameSources);

            // Configure base URI sources
            var baseUri = builder.AddBaseUri().Self();
            AddConfiguredSources(baseUri, cspConfig.AllowedBaseUriSources);

            // Configure form action sources
            var formAction = builder.AddFormAction().Self();
            AddConfiguredSources(formAction, cspConfig.AllowedFormActionSources);
        });
    }

    private static void AddConfiguredSources<T>(T builder, IEnumerable<string> sources) where T : class
    {
        foreach (var source in sources)
        {
            // Use reflection to call the From method on the builder
            var fromMethod = builder.GetType().GetMethod("From", new[] { typeof(string) });
            fromMethod?.Invoke(builder, new object[] { source });
        }
    }
}
