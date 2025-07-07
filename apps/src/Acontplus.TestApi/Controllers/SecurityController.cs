﻿namespace Acontplus.TestApi.Controllers
{
    public class SecurityController : BaseApiController
    {
        private readonly IPasswordSecurityService _dataSecurityService;

        public SecurityController(IPasswordSecurityService dataSecurityService)
        {
            _dataSecurityService = dataSecurityService;
        }

        // Endpoint to set password (encrypts and hashes the password)
        [HttpPost("setpassword")]
        public IActionResult SetPassword([FromBody] PasswordRequest request)
        {
            var result = _dataSecurityService.SetPassword(request.Password);
            return Ok(new { EncryptedPassword = Convert.ToBase64String(result.EncryptedPassword), result.PasswordHash });
        }

        // Endpoint to get decrypted password
        [HttpPost("decryptpassword")]
        public IActionResult GetDecryptedPassword([FromBody] EncryptedPasswordRequest request)
        {
            var encryptedPasswordBytes = Convert.FromBase64String(request.EncryptedPassword);
            var decryptedPassword = _dataSecurityService.GetDecryptedPassword(encryptedPasswordBytes);
            return Ok(decryptedPassword);
        }

        // Endpoint to verify password
        [HttpPost("verifypassword")]
        public IActionResult VerifyPassword([FromBody] VerifyPasswordRequest request)
        {
            var isValid = _dataSecurityService.VerifyPassword(request.Password, request.PasswordHash);
            return Ok(isValid);
        }
    }

    public class PasswordRequest
    {
        public string Password { get; set; }
    }

    public class EncryptedPasswordRequest
    {
        public string EncryptedPassword { get; set; }
    }

    public class VerifyPasswordRequest
    {
        public string Password { get; set; }
        public string PasswordHash { get; set; }
    }
}
