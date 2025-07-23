using Acontplus.S3Application.Interfaces;
using Acontplus.S3Application.Models;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace Acontplus.S3Application.Services;

/// <summary>
/// Provides a concrete implementation of <see cref="IS3StorageService"/> for performing AWS S3 storage operations, including async CRUD, retrieval, and presigned URL generation.
/// </summary>
public class S3StorageService : IS3StorageService
{
    /// <summary>
    /// Uploads a new object to S3 asynchronously.
    /// </summary>
    /// <param name="s3ObjectCustom">The S3 object to upload.</param>
    /// <returns>A response with status and metadata.</returns>
    public async Task<S3Response> UploadAsync(S3ObjectCustom s3ObjectCustom)
    {
        ArgumentNullException.ThrowIfNull(s3ObjectCustom);

        var credentials = new BasicAWSCredentials(
            s3ObjectCustom.AwsCredentials?.Key,
            s3ObjectCustom.AwsCredentials?.Secret);

        AWSConfigs.AWSRegion = s3ObjectCustom.Region;
        var response = new S3Response();

        try
        {
            using (var ms = new MemoryStream(s3ObjectCustom.Content))
            {
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = ms,
                    Key = s3ObjectCustom.S3ObjectKey,
                    BucketName = s3ObjectCustom.BucketName,
                    CannedACL = S3CannedACL.NoACL
                };

                if (!string.IsNullOrEmpty(s3ObjectCustom.ContentType))
                {
                    uploadRequest.ContentType = s3ObjectCustom.ContentType;
                }

                using var client = new AmazonS3Client(credentials, RegionEndpoint.GetBySystemName(s3ObjectCustom.Region));
                var transferUtility = new TransferUtility(client);
                await transferUtility.UploadAsync(uploadRequest);
            }

            response.StatusCode = 201;
            response.Message = $"El archivo {s3ObjectCustom.S3ObjectKey} se subió correctamente en Amazon S3";
        }
        catch (AmazonS3Exception s3Ex)
        {
            response.StatusCode = (int)s3Ex.StatusCode;
            response.Message = s3Ex.Message;
        }
        catch (Exception ex)
        {
            response.StatusCode = 500;
            response.Message = ex.Message;
        }

        return response;
    }

    /// <summary>
    /// Updates an existing object in S3 asynchronously.
    /// </summary>
    /// <param name="s3ObjectCustom">The S3 object to update.</param>
    /// <returns>A response with status and metadata.</returns>
    public async Task<S3Response> UpdateAsync(S3ObjectCustom s3ObjectCustom)
    {
        if (s3ObjectCustom == null) throw new ArgumentNullException(nameof(s3ObjectCustom));

        var credentials = new BasicAWSCredentials(
            s3ObjectCustom.AwsCredentials.Key,
            s3ObjectCustom.AwsCredentials.Secret);

        AWSConfigs.AWSRegion = s3ObjectCustom.Region;
        var response = new S3Response();

        try
        {
            using var client = new AmazonS3Client(credentials, RegionEndpoint.GetBySystemName(s3ObjectCustom.Region));
            using (var ms = new MemoryStream(s3ObjectCustom.Content))
            {
                var request = new PutObjectRequest
                {
                    BucketName = s3ObjectCustom.BucketName,
                    Key = s3ObjectCustom.S3ObjectKey,
                    InputStream = ms,
                    CannedACL = S3CannedACL.NoACL
                };

                if (!string.IsNullOrEmpty(s3ObjectCustom.ContentType))
                {
                    request.ContentType = s3ObjectCustom.ContentType;
                }

                var s3Response = await client.PutObjectAsync(request);
            }

            response.StatusCode = 200;
            response.Message = $"El archivo {s3ObjectCustom.S3ObjectKey} se actualizó correctamente en Amazon S3";
        }
        catch (AmazonS3Exception s3Ex)
        {
            response.StatusCode = (int)s3Ex.StatusCode;
            response.Message = s3Ex.Message;
        }
        catch (Exception ex)
        {
            response.StatusCode = 500;
            response.Message = ex.Message;
        }

        return response;
    }

    /// <summary>
    /// Deletes an object from S3 asynchronously.
    /// </summary>
    /// <param name="s3ObjectCustom">The S3 object to delete.</param>
    /// <returns>A response with status and metadata.</returns>
    public async Task<S3Response> DeleteAsync(S3ObjectCustom s3ObjectCustom)
    {
        if (s3ObjectCustom == null) throw new ArgumentNullException(nameof(s3ObjectCustom));

        var credentials = new BasicAWSCredentials(
            s3ObjectCustom.AwsCredentials.Key,
            s3ObjectCustom.AwsCredentials.Secret);

        AWSConfigs.AWSRegion = s3ObjectCustom.Region;
        var response = new S3Response();

        try
        {
            using var client = new AmazonS3Client(credentials, RegionEndpoint.GetBySystemName(s3ObjectCustom.Region));
            var request = new DeleteObjectRequest
            {
                BucketName = s3ObjectCustom.BucketName,
                Key = s3ObjectCustom.S3ObjectKey
            };

            var s3Response = await client.DeleteObjectAsync(request);

            response.StatusCode = 200;
            response.Message = $"El archivo {s3ObjectCustom.S3ObjectKey} se eliminó correctamente de Amazon S3";
        }
        catch (AmazonS3Exception s3Ex)
        {
            response.StatusCode = (int)s3Ex.StatusCode;
            response.Message = s3Ex.Message;
        }
        catch (Exception ex)
        {
            response.StatusCode = 500;
            response.Message = ex.Message;
        }

        return response;
    }

    /// <summary>
    /// Retrieves an object from S3 asynchronously.
    /// </summary>
    /// <param name="s3ObjectCustom">The S3 object to retrieve.</param>
    /// <returns>A response with file content and metadata.</returns>
    public async Task<S3Response> GetObjectAsync(S3ObjectCustom s3ObjectCustom)
    {
        if (s3ObjectCustom == null) throw new ArgumentNullException(nameof(s3ObjectCustom));

        var credentials = new BasicAWSCredentials(
            s3ObjectCustom.AwsCredentials.Key,
            s3ObjectCustom.AwsCredentials.Secret);

        AWSConfigs.AWSRegion = s3ObjectCustom.Region;
        var response = new S3Response();

        try
        {
            using var client = new AmazonS3Client(credentials, RegionEndpoint.GetBySystemName(s3ObjectCustom.Region));
            var request = new GetObjectRequest
            {
                BucketName = s3ObjectCustom.BucketName,
                Key = s3ObjectCustom.S3ObjectKey
            };

            using var s3Response = await client.GetObjectAsync(request);
            using var memoryStream = new MemoryStream();

            await s3Response.ResponseStream.CopyToAsync(memoryStream);

            response.StatusCode = 200;
            response.Message = $"El archivo {s3ObjectCustom.S3ObjectKey} se obtuvo correctamente de Amazon S3";
            response.Content = memoryStream.ToArray();
            response.ContentType = s3Response.Headers.ContentType;
            response.FileName = Path.GetFileName(s3ObjectCustom.S3ObjectKey);
        }
        catch (AmazonS3Exception s3Ex)
        {
            response.StatusCode = (int)s3Ex.StatusCode;
            response.Message = s3Ex.Message;
        }
        catch (Exception ex)
        {
            response.StatusCode = 500;
            response.Message = ex.Message;
        }

        return response;
    }

    /// <summary>
    /// Checks if an object exists in S3 asynchronously.
    /// </summary>
    /// <param name="s3ObjectCustom">The S3 object to check.</param>
    /// <returns>True if the object exists; otherwise, false.</returns>
    public async Task<bool> DoesObjectExistAsync(S3ObjectCustom s3ObjectCustom)
    {
        if (s3ObjectCustom == null) throw new ArgumentNullException(nameof(s3ObjectCustom));

        var credentials = new BasicAWSCredentials(
            s3ObjectCustom.AwsCredentials.Key,
            s3ObjectCustom.AwsCredentials.Secret);

        AWSConfigs.AWSRegion = s3ObjectCustom.Region;

        try
        {
            using var client = new AmazonS3Client(credentials, RegionEndpoint.GetBySystemName(s3ObjectCustom.Region));
            var request = new GetObjectMetadataRequest
            {
                BucketName = s3ObjectCustom.BucketName,
                Key = s3ObjectCustom.S3ObjectKey
            };

            await client.GetObjectMetadataAsync(request);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
        catch
        {
            // Si hay cualquier otro error, consideramos que no podemos determinar si existe
            return false;
        }
    }

    /// <summary>
    /// Generates a presigned URL for an S3 object asynchronously.
    /// </summary>
    /// <param name="s3ObjectCustom">The S3 object for which to generate the URL.</param>
    /// <param name="expirationInMinutes">The expiration time in minutes for the URL.</param>
    /// <returns>A response containing the presigned URL.</returns>
    public async Task<S3Response> GetPresignedUrlAsync(S3ObjectCustom s3ObjectCustom, int expirationInMinutes = 60)
    {
        if (s3ObjectCustom == null) throw new ArgumentNullException(nameof(s3ObjectCustom));

        var credentials = new BasicAWSCredentials(
            s3ObjectCustom.AwsCredentials.Key,
            s3ObjectCustom.AwsCredentials.Secret);

        AWSConfigs.AWSRegion = s3ObjectCustom.Region;
        var response = new S3Response();

        try
        {
            using var client = new AmazonS3Client(credentials, RegionEndpoint.GetBySystemName(s3ObjectCustom.Region));
            var request = new GetPreSignedUrlRequest
            {
                BucketName = s3ObjectCustom.BucketName,
                Key = s3ObjectCustom.S3ObjectKey,
                Expires = DateTime.UtcNow.AddMinutes(expirationInMinutes)
            };

            var presignedUrl = await client.GetPreSignedURLAsync(request);

            response.StatusCode = 200;
            response.Message = $"URL prefirmada generada correctamente para {s3ObjectCustom.S3ObjectKey}";
            response.FileName = presignedUrl;
        }
        catch (AmazonS3Exception s3Ex)
        {
            response.StatusCode = (int)s3Ex.StatusCode;
            response.Message = s3Ex.Message;
        }
        catch (Exception ex)
        {
            response.StatusCode = 500;
            response.Message = ex.Message;
        }

        return response;
    }
}
