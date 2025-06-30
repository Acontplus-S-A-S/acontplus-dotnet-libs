namespace Acontplus.Utilities.Security.Interfaces;

public interface IDataEncryptionService
{
    byte[] EncryptToBytes(string plainText);
    string DecryptFromBytes(byte[] encryptedData);
}
