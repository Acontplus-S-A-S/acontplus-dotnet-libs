namespace Acontplus.Services.Extensions.Authentication;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {
        // Extract and validate configuration values
        var issuer = config["JwtSettings:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is required");
        var securityKey = config["JwtSettings:SecurityKey"] ?? throw new InvalidOperationException("JWT SecurityKey is required");

        // Get audience(s) - supports both string and array
        var audienceSection = config.GetSection("JwtSettings:Audience");
        string[] audiences;

        if (audienceSection.Value != null)
        {
            // Single audience (string)
            audiences = new[] { audienceSection.Value };
        }
        else
        {
            // Multiple audiences (array)
            audiences = audienceSection.Get<string[]>() ??
                throw new InvalidOperationException("JWT Audience is required");
        }

        if (audiences.Length == 0)
        {
            throw new InvalidOperationException("At least one JWT Audience is required");
        }

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Security best practices
                    RequireExpirationTime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    // Enhanced security
                    RequireSignedTokens = true,
                    ValidateTokenReplay = true,

                    // Configuration values
                    ValidIssuer = issuer,
                    ValidAudiences = audiences, // Supports both single and multiple audiences
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(securityKey)),

                    // Clock skew for time validation
                    ClockSkew = TimeSpan.FromMinutes(
                        Convert.ToInt32(config["JwtSettings:ClockSkew"] ?? "5"))
                };

                // Enhanced JWT bearer options
                options.RequireHttpsMetadata = Convert.ToBoolean(
                    config["JwtSettings:RequireHttps"] ?? "true");
                options.SaveToken = false;
                options.IncludeErrorDetails = false;
            });

        services.AddAuthorization();
        services.AddAuthorizationBuilder();

        return services;
    }
}
