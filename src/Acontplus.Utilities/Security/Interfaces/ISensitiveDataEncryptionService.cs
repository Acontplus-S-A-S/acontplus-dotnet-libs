namespace Acontplus.Utilities.Security.Interfaces;

/// <summary>
/// Defines asynchronous methods for encrypting and decrypting sensitive data.
/// </summary>
public interface ISensitiveDataEncryptionService
{
    /// <summary>
    /// Asynchronously encrypts the provided plaintext string using the specified key and returns the encrypted data as a byte array.
    /// </summary>
    /// <param name="passphrase">The encryption key (must be 16, 24, or 32 bytes long).</param>
    /// <param name="data">The plaintext string to encrypt.</param>
    /// <returns>A Task containing a byte array with the IV followed by the encrypted data.</returns>
    Task<byte[]> EncryptToBytesAsync(string passphrase, string data);

    /// <summary>
    /// Asynchronously decrypts the provided byte array using the specified key and returns the decrypted plaintext string.
    /// </summary>
    /// <param name="passphrase">The decryption key (must match the key used for encryption).</param>
    /// <param name="encryptedData">The byte array containing the IV followed by the encrypted data.</param>
    /// <returns>A Task containing the decrypted plaintext string.</returns>
    Task<string> DecryptFromBytesAsync(string passphrase, byte[] encryptedData);
}
