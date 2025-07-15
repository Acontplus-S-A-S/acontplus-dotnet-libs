using Acontplus.S3Application.Models;

namespace Acontplus.S3Application.Interfaces;

/// <summary>
/// Defines a contract for AWS S3 storage operations, including async CRUD, retrieval, and presigned URL generation.
/// </summary>
public interface IS3StorageService
{
    /// <summary>
    /// Uploads a new object to S3 asynchronously.
    /// </summary>
    /// <param name="s3ObjectCustom">The S3 object to upload.</param>
    /// <returns>A response with status and metadata.</returns>
    Task<S3Response> UploadAsync(S3ObjectCustom s3ObjectCustom);

    /// <summary>
    /// Updates an existing object in S3 asynchronously.
    /// </summary>
    /// <param name="s3ObjectCustom">The S3 object to update.</param>
    /// <returns>A response with status and metadata.</returns>
    Task<S3Response> UpdateAsync(S3ObjectCustom s3ObjectCustom);

    /// <summary>
    /// Deletes an object from S3 asynchronously.
    /// </summary>
    /// <param name="s3ObjectCustom">The S3 object to delete.</param>
    /// <returns>A response with status and metadata.</returns>
    Task<S3Response> DeleteAsync(S3ObjectCustom s3ObjectCustom);

    /// <summary>
    /// Retrieves an object from S3 asynchronously.
    /// </summary>
    /// <param name="s3ObjectCustom">The S3 object to retrieve.</param>
    /// <returns>A response with file content and metadata.</returns>
    Task<S3Response> GetObjectAsync(S3ObjectCustom s3ObjectCustom);

    /// <summary>
    /// Checks if an object exists in S3 asynchronously.
    /// </summary>
    /// <param name="s3ObjectCustom">The S3 object to check.</param>
    /// <returns>True if the object exists; otherwise, false.</returns>
    Task<bool> DoesObjectExistAsync(S3ObjectCustom s3ObjectCustom);

    /// <summary>
    /// Generates a presigned URL for an S3 object asynchronously.
    /// </summary>
    /// <param name="s3ObjectCustom">The S3 object for which to generate the URL.</param>
    /// <param name="expirationInMinutes">The expiration time in minutes for the URL.</param>
    /// <returns>A response containing the presigned URL.</returns>
    Task<S3Response> GetPresignedUrlAsync(S3ObjectCustom s3ObjectCustom, int expirationInMinutes = 60);
}
