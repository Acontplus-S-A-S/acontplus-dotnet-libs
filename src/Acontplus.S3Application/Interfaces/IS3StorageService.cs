using Acontplus.S3Application.Models;

namespace Acontplus.S3Application.Interfaces;

public interface IS3StorageService
{
    Task<S3Response> UploadAsync(S3ObjectCustom s3ObjectCustom);
    Task<S3Response> UpdateAsync(S3ObjectCustom s3ObjectCustom);
    Task<S3Response> DeleteAsync(S3ObjectCustom s3ObjectCustom);
    Task<S3Response> GetObjectAsync(S3ObjectCustom s3ObjectCustom);
    Task<bool> DoesObjectExistAsync(S3ObjectCustom s3ObjectCustom);
    Task<S3Response> GetPresignedUrlAsync(S3ObjectCustom s3ObjectCustom, int expirationInMinutes = 60);
}
