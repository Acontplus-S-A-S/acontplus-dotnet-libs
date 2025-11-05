using Acontplus.Notifications.Models;

namespace Acontplus.TestApi.Controllers.Demo;

/// <summary>
/// Controller demonstrating basic usage of Acontplus libraries.
/// This controller showcases Logging, Notifications, and Security utilities.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LibraryShowcaseController : ControllerBase
{
    private readonly ILogger<LibraryShowcaseController> _logger;
    private readonly IMailKitService _emailService;
    private readonly IPasswordSecurityService _passwordSecurity;
    private readonly IDataEncryptionService _dataEncryption;

    public LibraryShowcaseController(
        ILogger<LibraryShowcaseController> logger,
        IMailKitService emailService,
        IPasswordSecurityService passwordSecurity,
        IDataEncryptionService dataEncryption)
    {
        _logger = logger;
        _emailService = emailService;
        _passwordSecurity = passwordSecurity;
        _dataEncryption = dataEncryption;
    }

    /// <summary>
    /// Demonstrates structured logging capabilities
    /// </summary>
    [HttpPost("logging")]
    public IActionResult DemonstrateLogging([FromBody] LogRequest request)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["OperationId"] = Guid.NewGuid(),
            ["UserId"] = request.UserId,
            ["Action"] = request.Action
        }))
        {
            _logger.LogInformation("Demonstrating structured logging for user {UserId}", request.UserId);
            _logger.LogDebug("Processing action: {Action}", request.Action);

            return Ok(new
            {
                Message = "Logging demonstration completed",
                UserId = request.UserId,
                Action = request.Action
            });
        }
    }

    /// <summary>
    /// Demonstrates password security utilities
    /// </summary>
    [HttpPost("password-security")]
    public IActionResult DemonstratePasswordSecurity([FromBody] DemoPasswordRequest request)
    {
        // Hash the password
        var hashedPassword = _passwordSecurity.HashPassword(request.Password);

        // Verify the password
        var isValid = _passwordSecurity.VerifyPassword(request.Password, hashedPassword);

        return Ok(new
        {
            Message = "Password security demonstration completed",
            IsValid = isValid,
            HashedLength = hashedPassword.Length
        });
    }

    /// <summary>
    /// Demonstrates data encryption capabilities
    /// </summary>
    [HttpPost("data-encryption")]
    public IActionResult DemonstrateDataEncryption([FromBody] EncryptionRequest request)
    {
        // Encrypt the data
        var encryptedData = _dataEncryption.EncryptToBytes(request.PlainText);

        // Decrypt the data
        var decryptedData = _dataEncryption.DecryptFromBytes(encryptedData);

        // Verify round-trip
        var isRoundTripSuccessful = request.PlainText == decryptedData;

        return Ok(new
        {
            Message = "Data encryption demonstration completed",
            OriginalLength = request.PlainText.Length,
            EncryptedLength = encryptedData.Length,
            IsRoundTripSuccessful = isRoundTripSuccessful
        });
    }

    /// <summary>
    /// Demonstrates email notification capabilities
    /// </summary>
    [HttpPost("email-notification")]
    public async Task<IActionResult> DemonstrateEmailNotification([FromBody] EmailRequest request)
    {
        try
        {
            var emailModel = new EmailModel
            {
                SmtpServer = "smtp.example.com", // Would come from config
                SmtpPort = 587,
                Password = "dummy", // Would come from secure config
                SenderName = "Test API",
                SenderEmail = "test@example.com",
                RecipientEmail = request.To,
                Subject = request.Subject,
                Body = request.Body,
                IsHtml = request.IsHtml
            };

            var result = await _emailService.SendAsync(emailModel);

            return Ok(new
            {
                Message = "Email notification demonstration completed",
                Success = result,
                Recipient = request.To
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email demonstration failed");
            return StatusCode(500, "Email demonstration failed");
        }
    }
}

public class LogRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
}

public class DemoPasswordRequest
{
    public string Password { get; set; } = string.Empty;
}

public class EncryptionRequest
{
    public string PlainText { get; set; } = string.Empty;
}

public class EmailRequest
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; }
}
