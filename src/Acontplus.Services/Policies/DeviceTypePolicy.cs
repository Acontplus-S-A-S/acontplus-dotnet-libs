using Acontplus.Services.Services.Abstractions;

namespace Acontplus.Services.Policies;

/// <summary>
/// Authorization requirement for device type validation and restrictions.
/// </summary>
public class DeviceTypeRequirement : IAuthorizationRequirement
{
    public List<DeviceType> AllowedDeviceTypes { get; }
    public bool RequireDeviceValidation { get; }

    public DeviceTypeRequirement(List<DeviceType> allowedDeviceTypes, bool requireDeviceValidation = true)
    {
        AllowedDeviceTypes = allowedDeviceTypes ?? throw new ArgumentNullException(nameof(allowedDeviceTypes));
        RequireDeviceValidation = requireDeviceValidation;
    }
}

/// <summary>
/// Authorization handler for device type validation.
/// </summary>
public class DeviceTypeHandler : AuthorizationHandler<DeviceTypeRequirement>
{
    private readonly IDeviceDetectionService _deviceDetectionService;
    private readonly ILogger<DeviceTypeHandler> _logger;

    public DeviceTypeHandler(
        IDeviceDetectionService deviceDetectionService,
        ILogger<DeviceTypeHandler> logger)
    {
        _deviceDetectionService = deviceDetectionService ?? throw new ArgumentNullException(nameof(deviceDetectionService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        DeviceTypeRequirement requirement)
    {
        if (context.Resource is not HttpContext httpContext)
        {
            _logger.LogWarning("Authorization context does not contain HttpContext");
            context.Fail();
            return Task.CompletedTask;
        }

        try
        {
            // Validate device headers if required
            if (requirement.RequireDeviceValidation)
            {
                if (!_deviceDetectionService.ValidateDeviceHeaders(httpContext))
                {
                    _logger.LogWarning("Device header validation failed");
                    context.Fail();
                    return Task.CompletedTask;
                }
            }

            // Detect device type
            var deviceType = _deviceDetectionService.DetectDeviceType(httpContext);

            // Check if device type is allowed
            if (!requirement.AllowedDeviceTypes.Contains(deviceType))
            {
                _logger.LogWarning(
                    "Device type '{DeviceType}' is not allowed. Allowed types: {AllowedTypes}",
                    deviceType, string.Join(", ", requirement.AllowedDeviceTypes));
                context.Fail();
                return Task.CompletedTask;
            }

            _logger.LogDebug("Device type '{DeviceType}' validation successful", deviceType);
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during device type validation");
            context.Fail();
            return Task.CompletedTask;
        }
    }
}

/// <summary>
/// Extension methods for registering device type authorization policies.
/// </summary>
public static class DeviceTypePolicyExtensions
{
    public static IServiceCollection AddDeviceTypeAuthorization(this IServiceCollection services)
    {
        services.AddScoped<IAuthorizationHandler, DeviceTypeHandler>();

        services.AddAuthorization(options =>
        {
            // Policy for mobile-only access
            options.AddPolicy("MobileOnly", policy =>
                policy.Requirements.Add(new DeviceTypeRequirement(new List<DeviceType> { DeviceType.Mobile })));

            // Policy for mobile and tablet access
            options.AddPolicy("MobileAndTablet", policy =>
                policy.Requirements.Add(new DeviceTypeRequirement(new List<DeviceType>
                    { DeviceType.Mobile, DeviceType.Tablet })));

            // Policy for desktop-only access
            options.AddPolicy("DesktopOnly", policy =>
                policy.Requirements.Add(new DeviceTypeRequirement(new List<DeviceType> { DeviceType.Desktop })));

            // Policy for all known device types (excludes Unknown)
            options.AddPolicy("KnownDevicesOnly", policy =>
                policy.Requirements.Add(new DeviceTypeRequirement(new List<DeviceType>
                    { DeviceType.Mobile, DeviceType.Tablet, DeviceType.Desktop, DeviceType.Web })));
        });

        return services;
    }
}