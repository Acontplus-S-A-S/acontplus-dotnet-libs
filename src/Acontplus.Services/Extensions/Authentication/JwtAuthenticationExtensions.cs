using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Acontplus.Services.Extensions.Authentication;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {
        // Extract and validate configuration values
        var issuer = config["JwtSettings:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is required");
        var audience = config["JwtSettings:Audience"] ?? throw new InvalidOperationException("JWT Audience is required");
        var securityKey = config["JwtSettings:SecurityKey"] ?? throw new InvalidOperationException("JWT SecurityKey is required");

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Security best practices - hardcoded for security
                    RequireExpirationTime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    // Enhanced security - hardcoded for security
                    RequireSignedTokens = true,
                    ValidateTokenReplay = true,

                    // Configuration values
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(securityKey)),

                    // Clock skew for time validation
                    ClockSkew = TimeSpan.FromMinutes(
                        Convert.ToInt32(config["JwtSettings:ClockSkew"] ?? "5"))
                };

                // Enhanced JWT bearer options
                options.RequireHttpsMetadata = Convert.ToBoolean(
                    config["JwtSettings:RequireHttps"] ?? "true");
                options.SaveToken = false; // Don't save tokens in claims
                options.IncludeErrorDetails = false; // Don't expose internal errors
            });

        services.AddAuthorization();
        services.AddAuthorizationBuilder();

        return services;
    }
}
