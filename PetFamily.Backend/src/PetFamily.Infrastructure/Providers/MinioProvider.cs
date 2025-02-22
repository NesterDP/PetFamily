using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using PetFamily.Application.FilesManagement.FilesData;
using PetFamily.Application.Providers;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Infrastructure.Options;

namespace PetFamily.Infrastructure.Providers;

public class MinioProvider : IFilesProvider
{
    private readonly IMinioClient _minioClient;
    private readonly ILogger<MinioOptions> _logger;

    public MinioProvider(
        IMinioClient minioClient,
        ILogger<MinioOptions> logger)
    {
        _minioClient = minioClient;
        _logger = logger;
    }

    public async Task<Result<string, Error>> DeleteFile(
        DeleteData deleteData,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var bucketExistArgs = new BucketExistsArgs()
                .WithBucket(deleteData.BucketName);

            var bucketExist = await _minioClient.BucketExistsAsync(bucketExistArgs, cancellationToken);

            if (bucketExist == false)
                return Error.Failure("error.bucket.exists", "No such bucket exists");
            
            var objExist = new StatObjectArgs()
                .WithBucket(deleteData.BucketName)
                .WithObject(deleteData.ObjectName);
            var objectStat = await _minioClient.StatObjectAsync(objExist, cancellationToken);
            
            if (objectStat.Size == 0)
                return Error.Failure("error.object.exist", "No such object exists");
            
            var args = new RemoveObjectArgs()
                .WithBucket(deleteData.BucketName)
                .WithObject(deleteData.ObjectName);
            await _minioClient.RemoveObjectAsync(args, cancellationToken);
            
            _logger.LogInformation("File in bucket = {Bucket} with name = {Name} was deleted",
                deleteData.BucketName, deleteData.ObjectName);
            
            return  deleteData.ObjectName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove file");
            return Error.Failure("error.file.remove", "Failed to remove file");
        }
    }

    public async Task<Result<string, Error>> GetPresignedUrl(
        GetPresignedUrlData getPresignedUrlData,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var bucketExistArgs = new BucketExistsArgs()
                .WithBucket(getPresignedUrlData.BucketName);

            var bucketExist = await _minioClient.BucketExistsAsync(bucketExistArgs, cancellationToken);

            if (bucketExist == false)
                return Error.Failure("error.bucket.exists", "No such bucket exists");
            
            var objExist = new StatObjectArgs()
                .WithBucket(getPresignedUrlData.BucketName)
                .WithObject(getPresignedUrlData.ObjectName);
            var objectStat = await _minioClient.StatObjectAsync(objExist, cancellationToken);
            
            if (objectStat.Size == 0)
                return Error.Failure("error.object.exist", "No such object exists");
            
            var args = new PresignedGetObjectArgs()
                .WithBucket(getPresignedUrlData.BucketName)
                .WithObject(getPresignedUrlData.ObjectName)
                .WithExpiry(60 * 60 * 24);

            var result = await _minioClient.PresignedGetObjectAsync(args);
            
            _logger.LogInformation("Url for file in bucket = {Bucket} with name = {Name} was created",
                getPresignedUrlData.BucketName, getPresignedUrlData.ObjectName);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get presigned url from minio");
            return Error.Failure("error.get.url", "Failed to create presigned url");
        }
    }

    public async Task<Result<string, Error>> UploadFile(
        UploadData uploadData,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var bucketExistArgs = new BucketExistsArgs()
                .WithBucket(uploadData.BucketName);

            var bucketExist = await _minioClient.BucketExistsAsync(bucketExistArgs, cancellationToken);

            if (bucketExist == false)
            {
                var makeBucketArgs = new MakeBucketArgs()
                    .WithBucket(uploadData.BucketName);

                await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
            }
            
            StatObjectArgs statObjectArgs = new StatObjectArgs()
                .WithBucket(uploadData.BucketName)
                .WithObject(uploadData.ObjectName);
            
            var objectStat = await _minioClient.StatObjectAsync(statObjectArgs, cancellationToken);
            if (objectStat.Size != 0)
                return Error.Failure("error.object.exist", "such object already exists");

            var args = new PutObjectArgs()
                .WithBucket(uploadData.BucketName)
                .WithStreamData(uploadData.Stream)
                .WithObjectSize(uploadData.Stream.Length)
                .WithObject(uploadData.ObjectName);

            var result = await _minioClient.PutObjectAsync(args, cancellationToken);
            
            _logger.LogInformation("File in bucket = {Bucket} with name = {Name} was added",
                uploadData.BucketName, uploadData.ObjectName);

            return result.ObjectName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file to minio");
            return Error.Failure("error.file.upload", "Failed to upload file to minio");
        }
    }
}