namespace Acontplus.Utilities.Security.Helpers;

public static class CryptographyHelper
{
    public static byte[] DeriveKey(string passphrase, int keySize, byte[] salt)
    {
        // Use the new static Pbkdf2 method instead of the obsolete constructor
        return Rfc2898DeriveBytes.Pbkdf2(
            passphrase,
            salt,
            100000,
            HashAlgorithmName.SHA256,
            keySize / 8);
    }

    public static byte[] ComputeHmac(string passphrase, byte[] data)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(passphrase));
        return hmac.ComputeHash(data);
    }
}

