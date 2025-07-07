namespace Acontplus.Utilities.Security.Helpers;
public static class CryptographyHelper
{
    public static byte[] DeriveKey(string passphrase, int keySize, byte[] salt)
    {
        // Use the updated Rfc2898DeriveBytes constructor
        using var keyDerivation = new Rfc2898DeriveBytes(passphrase, salt, 100000, HashAlgorithmName.SHA256);

        return keyDerivation.GetBytes(keySize / 8); // Key size in bytes
    }

    public static byte[] ComputeHmac(string passphrase, byte[] data)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(passphrase));
        return hmac.ComputeHash(data);
    }
}
