using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace Acontplus.Utilities.IO;

public static class FileExtensions
{
    public static string SanitizeFileName(string fileName)
    {
        var response = Regex.Replace(fileName.Trim(), "[^A-Za-z0-9_. ]+", "");
        return response.Replace(" ", string.Empty);
    }

    public static string GetBase64FromByte(byte[] valueByte)
    {
        if (valueByte == null)
        {
            throw new ArgumentNullException(nameof(valueByte));
        }

        // Using MemoryStream to avoid unnecessary allocations
        using var memoryStream = new MemoryStream();
        // Write the byte array to the MemoryStream
        memoryStream.Write(valueByte, 0, valueByte.Length);
        memoryStream.Seek(0, SeekOrigin.Begin);

        // Convert to Base64 string
        return Convert.ToBase64String(memoryStream.ToArray());
    }

    public static byte[] GetBytes(IFormFile file)
    {
        byte[] fileBytes = null;
        using var ms = new MemoryStream();
        file.CopyTo(ms);
        fileBytes = ms.ToArray();
        return fileBytes;
    }
}
