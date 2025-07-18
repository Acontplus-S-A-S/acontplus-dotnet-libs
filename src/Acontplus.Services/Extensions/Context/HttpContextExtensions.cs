namespace Acontplus.Services.Extensions.Context;

public static class HttpContextExtensions
{
    private const string RequestIdKey = "RequestId";
    private const string CorrelationIdKey = "CorrelationId";
    private const string TenantIdKey = "TenantId";
    private const string ClientIdKey = "ClientId";
    private const string IssuerKey = "Issuer";
    private const string DeviceTypeKey = "DeviceType";
    private const string IsMobileRequestKey = "IsMobileRequest";

    public static void SetRequestId(this HttpContext context, string requestId) =>
        context.Items[RequestIdKey] = requestId;

    public static string? GetRequestId(this HttpContext context) =>
        context.Items.TryGetValue(RequestIdKey, out var value) ? value as string : null;

    public static void SetCorrelationId(this HttpContext context, string correlationId) =>
        context.Items[CorrelationIdKey] = correlationId;

    public static string? GetCorrelationId(this HttpContext context) =>
        context.Items.TryGetValue(CorrelationIdKey, out var value) ? value as string : null;

    public static void SetTenantId(this HttpContext context, string tenantId) =>
        context.Items[TenantIdKey] = tenantId;

    public static string? GetTenantId(this HttpContext context) =>
        context.Items.TryGetValue(TenantIdKey, out var value) ? value as string : null;

    public static void SetClientId(this HttpContext context, string? clientId) =>
        context.Items[ClientIdKey] = clientId;

    public static string? GetClientId(this HttpContext context) =>
        context.Items.TryGetValue(ClientIdKey, out var value) ? value as string : null;

    public static void SetIssuer(this HttpContext context, string? issuer) =>
        context.Items[IssuerKey] = issuer;

    public static string? GetIssuer(this HttpContext context) =>
        context.Items.TryGetValue(IssuerKey, out var value) ? value as string : null;

    public static void SetDeviceType(this HttpContext context, DeviceType deviceType) =>
        context.Items[DeviceTypeKey] = deviceType.ToString().ToLowerInvariant();

    public static string? GetDeviceTypeString(this HttpContext context) =>
        context.Items.TryGetValue(DeviceTypeKey, out var value) ? value as string : null;

    public static DeviceType? GetDeviceType(this HttpContext context)
    {
        if (context.Items.TryGetValue(DeviceTypeKey, out var value) && value is string typeString)
        {
            return Enum.TryParse<DeviceType>(typeString, ignoreCase: true, out var deviceType) ? deviceType : null;
        }

        return null;
    }

    public static void SetIsMobileRequest(this HttpContext context, bool isMobileRequest)
    {
        context.Items["FromMobile"] = isMobileRequest; // Legacy
        context.Items[IsMobileRequestKey] = isMobileRequest;
    }

    public static bool? GetIsMobileRequest(this HttpContext context) =>
        context.Items.TryGetValue(IsMobileRequestKey, out var value) ? value as bool? : null;
}