namespace Acontplus.TestApi.Controllers.Core;

public class EncryptionController(
    IDataEncryptionService dataEncryptionService,
    ISensitiveDataEncryptionService sensitiveDataEncryptionService,
    IPasswordSecurityService passwordHashingService) : BaseApiController
{
    [HttpPost("encrypt")]
    public async Task<IActionResult> EncryptData([FromBody] EncryptRequest request)
    {
        // var encryptedBytes = dataEncryptionService.EncryptToBytes(request.PlainText);
        // var encrypted =
        var encryptedBytes = await sensitiveDataEncryptionService.EncryptToBytesAsync("ivan", request.PlainText);

        // Store encryptedData and extracted salt in DB

        return Ok(Convert.ToBase64String(encryptedBytes));
    }

    [HttpPost("decrypt")]
    public async Task<IActionResult> DecryptData([FromBody] DecryptRequest request)
    {
        var encryptedBytes = Convert.FromBase64String(request.EncryptedData);
        // var decryptedText = dataEncryptionService.DecryptFromBytes(encryptedBytes);
        // Retrieve encryptedData and salt from DB
        var decryptedData = await sensitiveDataEncryptionService.DecryptFromBytesAsync("ivan", encryptedBytes);

        return Ok(decryptedData);
    }

    [HttpPost("hash")]
    public IActionResult HashPassword([FromBody] HashRequest request)
    {
        var hashedPassword = passwordHashingService.HashPassword(request.Password);
        return Ok(hashedPassword);
    }

    [HttpPost("verify")]
    public IActionResult VerifyPassword([FromBody] VerifyRequest request)
    {
        var isValid = passwordHashingService.VerifyPassword(request.Password, request.HashedPassword);
        return Ok(isValid);
    }
}

public class EncryptRequest
{
    public string PlainText { get; set; }
}

public class DecryptRequest
{
    public string EncryptedData { get; set; }
}

public class HashRequest
{
    public string Password { get; set; }
}

public class VerifyRequest
{
    public string Password { get; set; }
    public string HashedPassword { get; set; }
}

