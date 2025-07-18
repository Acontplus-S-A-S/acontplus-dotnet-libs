using System.Security.Cryptography;

namespace Acontplus.Services.Middleware;


public class CspNonceMiddleware
{
    private readonly RequestDelegate _next;

    public CspNonceMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Generate a unique nonce for this request
        var nonce = GenerateNonce();
        context.Items["csp-nonce"] = nonce;

        await _next(context);
    }

    private static string GenerateNonce()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[16];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}

public static class CspNonceExtensions
{
    public static string GetCspNonce(this HttpContext context)
    {
        return context.Items["csp-nonce"]?.ToString() ?? string.Empty;
    }
}