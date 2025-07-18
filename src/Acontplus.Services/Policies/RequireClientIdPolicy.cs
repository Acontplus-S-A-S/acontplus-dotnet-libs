using Microsoft.AspNetCore.Authorization;

namespace Acontplus.Services.Policies;

/// <summary>
/// Authorization requirement that validates the presence and validity of a Client-Id header.
/// </summary>
public class RequireClientIdRequirement : IAuthorizationRequirement
{
    public List<string>? AllowedClientIds { get; }
    public bool AllowAnonymous { get; }

    public RequireClientIdRequirement(List<string>? allowedClientIds = null, bool allowAnonymous = false)
    {
        AllowedClientIds = allowedClientIds;
        AllowAnonymous = allowAnonymous;
    }
}

/// <summary>
/// Authorization handler for Client-Id validation.
/// </summary>
public class RequireClientIdHandler : AuthorizationHandler<RequireClientIdRequirement>
{
    private readonly ILogger<RequireClientIdHandler> _logger;

    public RequireClientIdHandler(ILogger<RequireClientIdHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RequireClientIdRequirement requirement)
    {
        if (context.Resource is not HttpContext httpContext)
        {
            _logger.LogWarning("Authorization context does not contain HttpContext");
            context.Fail();
            return Task.CompletedTask;
        }

        // Get Client-Id from headers
        if (!httpContext.Request.Headers.TryGetValue("Client-Id", out var clientIdHeader))
        {
            if (requirement.AllowAnonymous)
            {
                _logger.LogDebug("No Client-Id header found, but anonymous access is allowed");
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            _logger.LogWarning("Client-Id header is required but not provided");
            context.Fail();
            return Task.CompletedTask;
        }

        var clientId = clientIdHeader.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(clientId))
        {
            _logger.LogWarning("Client-Id header is empty");
            context.Fail();
            return Task.CompletedTask;
        }

        // Validate against allowed client IDs if specified
        if (requirement.AllowedClientIds?.Any() == true)
        {
            if (!requirement.AllowedClientIds.Contains(clientId, StringComparer.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Client-Id '{ClientId}' is not in the allowed list", clientId);
                context.Fail();
                return Task.CompletedTask;
            }
        }

        _logger.LogDebug("Client-Id '{ClientId}' validation successful", clientId);
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}

/// <summary>
/// Extension methods for registering Client-Id authorization policies.
/// </summary>
public static class ClientIdPolicyExtensions
{
    public static IServiceCollection AddClientIdAuthorization(
        this IServiceCollection services,
        List<string>? allowedClientIds = null)
    {
        services.AddScoped<IAuthorizationHandler, RequireClientIdHandler>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireClientId", policy =>
                policy.Requirements.Add(new RequireClientIdRequirement(allowedClientIds)));

            options.AddPolicy("RequireClientIdOrAnonymous", policy =>
                policy.Requirements.Add(new RequireClientIdRequirement(allowedClientIds, allowAnonymous: true)));
        });

        return services;
    }
}