namespace Acontplus.Utilities.Security.Services;

public class DataEncryptionService : IDataEncryptionService
{
    private readonly IDataProtector _protector;
    public DataEncryptionService(IDataProtectionProvider provider, IConfiguration configuration)
    {
        var protectorKey = configuration["DataProtection:ProtectorKey"];
        _protector = provider.CreateProtector(protectorKey ?? throw new InvalidOperationException());
    }

    public byte[] EncryptToBytes(string plainText)
    {
        return _protector.Protect(Encoding.UTF8.GetBytes(plainText));
    }

    public string DecryptFromBytes(byte[] encryptedData)
    {
        return Encoding.UTF8.GetString(_protector.Unprotect(encryptedData));
    }
}
