namespace Acontplus.Utilities.Services;

public static class FileModelBuilder
{
    public static FileModel Create(byte[] content, string contentType, string? fileName = null)
    {
        if (content == null) throw new ArgumentNullException(nameof(content));

        return new FileModel
        {
            Content = content,
            ContentType = contentType ?? "application/octet-stream",
            FileName = fileName
        };
    }

    public static FileModel CreateBase64(byte[] content, string contentType, string? fileName = null)
    {
        if (content == null) throw new ArgumentNullException(nameof(content));

        return new FileModel
        {
            ContentType = contentType ?? "application/octet-stream",
            FileName = fileName,
            Base64 = Convert.ToBase64String(content)
        };
    }

    public static async Task<FileModel> CreateCompressedAsync(IFormFile file, Func<byte[], byte[]> compressor)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);

        return new FileModel
        {
            FileName = FileExtensions.SanitizeFileName(file.FileName),
            ContentType = file.ContentType,
            Content = compressor(ms.ToArray())
        };
    }
}
