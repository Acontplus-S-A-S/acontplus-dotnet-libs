namespace Acontplus.TestApi.Endpoints.Core;

using Acontplus.Core.Dtos;
using Acontplus.Utilities.Security.Interfaces;

public static class EncryptionEndpoints
{
    public static void MapEncryptionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/encryption")
            .WithTags("Encryption");

        group.MapPost("/encrypt", async (ISensitiveDataEncryptionService sensitiveDataEncryptionService, EncryptRequest request) =>
        {
            var encryptedBytes = await sensitiveDataEncryptionService.EncryptToBytesAsync("ivan", request.PlainText!);
            return Results.Ok(ApiResponse.Success(Convert.ToBase64String(encryptedBytes)));
        });

        group.MapPost("/decrypt", async (ISensitiveDataEncryptionService sensitiveDataEncryptionService, DecryptRequest request) =>
        {
            var encryptedBytes = Convert.FromBase64String(request.EncryptedData!);
            var decryptedData = await sensitiveDataEncryptionService.DecryptFromBytesAsync("ivan", encryptedBytes);
            return Results.Ok(ApiResponse.Success(decryptedData));
        });

        group.MapPost("/hash", (IPasswordSecurityService passwordHashingService, HashRequest request) =>
        {
            var hashedPassword = passwordHashingService.HashPassword(request.Password!);
            return Results.Ok(ApiResponse.Success(hashedPassword));
        });

        group.MapPost("/setpassword", (IPasswordSecurityService dataSecurityService, SetPasswordRequest request) =>
        {
            var result = dataSecurityService.SetPassword(request.Password!);
            return Results.Ok(ApiResponse.Success(new { EncryptedPassword = Convert.ToBase64String(result.EncryptedPassword), result.PasswordHash }));
        });

        group.MapPost("/decryptpassword", (IPasswordSecurityService dataSecurityService, EncryptedPasswordRequest request) =>
        {
            var encryptedPasswordBytes = Convert.FromBase64String(request.EncryptedPassword!);
            var decryptedPassword = dataSecurityService.GetDecryptedPassword(encryptedPasswordBytes);
            return Results.Ok(ApiResponse.Success(decryptedPassword));
        });

        group.MapPost("/verifypassword", (IPasswordSecurityService dataSecurityService, VerifyPasswordRequest request) =>
        {
            var isValid = dataSecurityService.VerifyPassword(request.Password!, request.PasswordHash!);
            return Results.Ok(ApiResponse.Success(isValid));
        });
    }
}

public class EncryptRequest
{
    public string? PlainText { get; set; }
}

public class DecryptRequest
{
    public string? EncryptedData { get; set; }
}

public class HashRequest
{
    public string? Password { get; set; }
}

public class SetPasswordRequest
{
    public string? Password { get; set; }
}

public class EncryptedPasswordRequest
{
    public string? EncryptedPassword { get; set; }
}

public class VerifyPasswordRequest
{
    public string? Password { get; set; }
    public string? PasswordHash { get; set; }
}
