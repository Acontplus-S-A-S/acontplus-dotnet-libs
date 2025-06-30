namespace Acontplus.Utilities.Security.Services;

public class PasswordSecurityService(IDataEncryptionService dataEncryptionService) : IPasswordSecurityService
{
    public string GetDecryptedPassword(byte[] encryptedPassword)
    {
        return dataEncryptionService.DecryptFromBytes(encryptedPassword);
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public (byte[] EncryptedPassword, string PasswordHash) SetPassword(string password)
    {
        var encryptedPassword = dataEncryptionService.EncryptToBytes(password);
        var passwordHash = HashPassword(password);
        return (encryptedPassword, passwordHash);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
