namespace Acontplus.Utilities.Security.Interfaces;

public interface IPasswordSecurityService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
    (byte[] EncryptedPassword, string PasswordHash) SetPassword(string password);
    string GetDecryptedPassword(byte[] encryptedPassword);
}
