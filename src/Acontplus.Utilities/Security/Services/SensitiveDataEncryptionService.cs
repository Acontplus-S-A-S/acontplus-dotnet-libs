using System.Security.Cryptography;

namespace Acontplus.Utilities.Security.Services;

public class SensitiveDataEncryptionService : ISensitiveDataEncryptionService
{
    /// <summary>
    /// Encrypts the provided data using AES encryption and returns the encrypted byte array.
    /// </summary>
    /// <param name="passphrase">The encryption key (must be 16, 24, or 32 bytes long).</param> 
    /// <param name="data">The plaintext data to encrypt.</param>
    /// <returns>A byte array containing the IV followed by the encrypted data.</returns>
    public async Task<byte[]> EncryptToBytesAsync(string passphrase, string data)
    {
        if (string.IsNullOrWhiteSpace(passphrase)) throw new ArgumentException("Passphrase cannot be null or empty.", nameof(passphrase));
        if (string.IsNullOrWhiteSpace(data)) throw new ArgumentException("Data cannot be null or empty.", nameof(data));

        // Generate random salt (16 bytes)
        var salt = RandomNumberGenerator.GetBytes(16);

        // Derive key from passphrase and salt
        var key = CryptographyHelper.DeriveKey(passphrase, 256, salt);

        using var aes = Aes.Create();
        aes.Key = key;
        aes.GenerateIV(); // Generate random IV
        var iv = aes.IV;

        using var memoryStream = new MemoryStream();

        // Write Salt + IV to the beginning of the stream
        await memoryStream.WriteAsync(salt);
        await memoryStream.WriteAsync(iv);

        using (var encryptor = aes.CreateEncryptor(aes.Key, iv))
        await using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
        await using (var streamWriter = new StreamWriter(cryptoStream))
        {
            await streamWriter.WriteAsync(data);
        }

        var encryptedData = memoryStream.ToArray();

        // Clear sensitive data
        Array.Clear(key, 0, key.Length);

        return encryptedData; // Salt + IV + Ciphertext
    }


    /// <summary>
    /// Decrypts the provided byte array using AES encryption and returns the plaintext string.
    /// </summary>
    /// <param name="passphrase">The decryption key (must match the key used for encryption).</param>
    /// <param name="encryptedData">The byte array containing the IV followed by the encrypted data.</param>
    /// <returns>The decrypted plaintext string.</returns>
    public async Task<string> DecryptFromBytesAsync(string passphrase, byte[] encryptedData)
    {
        if (string.IsNullOrWhiteSpace(passphrase)) throw new ArgumentException("Passphrase cannot be null or empty.", nameof(passphrase));
        if (encryptedData == null || encryptedData.Length == 0) throw new ArgumentException("Encrypted data cannot be null or empty.", nameof(encryptedData));

        // Define sizes
        const int saltSize = 16; // Salt size
        const int ivSize = 16;   // AES block size in bytes

        // Ensure the data length is sufficient
        if (encryptedData.Length <= saltSize + ivSize)
            throw new ArgumentException("Encrypted data is invalid or corrupted.", nameof(encryptedData));

        using var memoryStream = new MemoryStream(encryptedData);

        // Read Salt from the beginning
        var salt = new byte[saltSize];
        await memoryStream.ReadExactlyAsync(salt, 0, salt.Length);

        // Derive key from passphrase and salt
        var key = CryptographyHelper.DeriveKey(passphrase, 256, salt);

        // Read IV
        var iv = new byte[ivSize];
        await memoryStream.ReadExactlyAsync(iv, 0, iv.Length);

        // Read Ciphertext
        var ciphertextLength = encryptedData.Length - saltSize - ivSize;
        var ciphertext = new byte[ciphertextLength];
        await memoryStream.ReadExactlyAsync(ciphertext, 0, ciphertext.Length);

        // Initialize AES
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        // Decrypt
        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        await using var cryptoStream = new CryptoStream(new MemoryStream(ciphertext), decryptor, CryptoStreamMode.Read);
        using var streamReader = new StreamReader(cryptoStream);

        // Read and return the plaintext
        var plaintext = await streamReader.ReadToEndAsync();

        // Clear sensitive data
        Array.Clear(key, 0, key.Length);

        return plaintext;
    }
}
