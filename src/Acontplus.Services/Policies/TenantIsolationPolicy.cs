using Acontplus.Services.Extensions.Context;
using Microsoft.AspNetCore.Authorization;

namespace Acontplus.Services.Policies;

/// <summary>
/// Authorization requirement for tenant isolation and validation.
/// </summary>
public class TenantIsolationRequirement : IAuthorizationRequirement
{
    public bool RequireTenantHeader { get; }
    public bool ValidateUserTenantAccess { get; }

    public TenantIsolationRequirement(bool requireTenantHeader = true, bool validateUserTenantAccess = true)
    {
        RequireTenantHeader = requireTenantHeader;
        ValidateUserTenantAccess = validateUserTenantAccess;
    }
}

/// <summary>
/// Authorization handler for tenant isolation validation.
/// </summary>
public class TenantIsolationHandler : AuthorizationHandler<TenantIsolationRequirement>
{
    private readonly ILogger<TenantIsolationHandler> _logger;

    public TenantIsolationHandler(ILogger<TenantIsolationHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TenantIsolationRequirement requirement)
    {
        if (context.Resource is not HttpContext httpContext)
        {
            _logger.LogWarning("Authorization context does not contain HttpContext");
            context.Fail();
            return Task.CompletedTask;
        }

        // Get Tenant-Id from headers
        if (!httpContext.Request.Headers.TryGetValue("Tenant-Id", out var tenantIdHeader))
        {
            if (requirement.RequireTenantHeader)
            {
                _logger.LogWarning("Tenant-Id header is required but not provided");
                context.Fail();
                return Task.CompletedTask;
            }

            _logger.LogDebug("No Tenant-Id header found, but not required");
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var tenantId = tenantIdHeader.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            _logger.LogWarning("Tenant-Id header is empty");
            context.Fail();
            return Task.CompletedTask;
        }

        // Validate user has access to the specified tenant
        if (requirement.ValidateUserTenantAccess && context.User.Identity?.IsAuthenticated == true)
        {
            var userTenantId = context.User.GetClaimValue<string>("tenant_id");
            if (!string.IsNullOrEmpty(userTenantId) &&
                !string.Equals(userTenantId, tenantId, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning(
                    "User tenant '{UserTenant}' does not match requested tenant '{RequestedTenant}'",
                    userTenantId, tenantId);
                context.Fail();
                return Task.CompletedTask;
            }
        }

        _logger.LogDebug("Tenant isolation validation successful for tenant '{TenantId}'", tenantId);
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}

/// <summary>
/// Extension methods for registering tenant isolation authorization policies.
/// </summary>
public static class TenantIsolationPolicyExtensions
{
    public static IServiceCollection AddTenantIsolationAuthorization(this IServiceCollection services)
    {
        services.AddScoped<IAuthorizationHandler, TenantIsolationHandler>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireTenant", policy =>
                policy.Requirements.Add(new TenantIsolationRequirement()));

            options.AddPolicy("ValidateTenantAccess", policy =>
                policy.Requirements.Add(new TenantIsolationRequirement(validateUserTenantAccess: true)));

            options.AddPolicy("OptionalTenant", policy =>
                policy.Requirements.Add(new TenantIsolationRequirement(requireTenantHeader: false)));
        });

        return services;
    }
}