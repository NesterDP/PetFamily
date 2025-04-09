using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using PetFamily.Core;
using PetFamily.Core.Files.FilesData;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.Volunteers.Infrastructure.Options;
using FileInfo = PetFamily.Core.Files.FilesData.FileInfo;

namespace PetFamily.Volunteers.Infrastructure.Providers;

public class MinioProvider : IFilesProvider
{
    private readonly IMinioClient _minioClient;
    private readonly ILogger<MinioProvider> _logger;

    private const int MAX_DEGREE_OF_PARALLELISM = 10;

    public MinioProvider(
        IMinioClient minioClient,
        ILogger<MinioProvider> logger)
    {
        _minioClient = minioClient;
        _logger = logger;
    }
    
    public async Task<Result<IReadOnlyList<FilePath>, Error>> UploadFiles(
        IEnumerable<FileData> filesData,
        CancellationToken cancellationToken = default)
    {
        var semaphoreSlim = new SemaphoreSlim(MAX_DEGREE_OF_PARALLELISM);
        var filesList = filesData.ToList();

        try
        {
            await IfBucketsNotExistCreateBucket(filesList, cancellationToken);


            var tasks = filesList.Select(async file =>
                await PutObject(file, semaphoreSlim, cancellationToken));

            var pathsResult = await Task.WhenAll(tasks);


            if (pathsResult.Any(p => p.IsFailure))
                return pathsResult.First().Error;

            var results = pathsResult.Select(p => p.Value).ToList();
            
            _logger.LogInformation("Uploaded files to Minio: {files}", results.Select(f => f.Path));

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to upload files in minio, files amount: {amount}", filesList.Count);

            return Error.Failure("file.upload", "Failed to upload files in minio");
        }
    }
    
    public async Task<Result<IReadOnlyList<FilePath>, Error>> DeleteFiles(
        IEnumerable<FileInfo> filesInfos,
        CancellationToken cancellationToken = default)
    {
        var semaphoreSlim = new SemaphoreSlim(MAX_DEGREE_OF_PARALLELISM);
        var filesInfosList = filesInfos.ToList();

        try
        {
            var tasks = filesInfosList.Select(async fileInfo =>
                await RemoveObject(fileInfo, semaphoreSlim, cancellationToken));

            var pathsResult = await Task.WhenAll(tasks);

            if (pathsResult.Any(p => p.IsFailure))
                return pathsResult.First().Error;

            var results = pathsResult.Select(p => p.Value).ToList();
            
            _logger.LogInformation("Deleted files in Minio: {files}", results.Select(f => f.Path).ToList());

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to delete files in minio, files amount: {amount}", filesInfosList.Count);

            return Error.Failure("file.delete", "Failed to delete files in minio");
        }
    }

    private async Task<Result<FilePath, Error>> PutObject(
        FileData fileData,
        SemaphoreSlim semaphoreSlim,
        CancellationToken cancellationToken)
    {
        await semaphoreSlim.WaitAsync(cancellationToken);

        var putObjectArgs = new PutObjectArgs()
            .WithBucket(fileData.Info.BucketName)
            .WithStreamData(fileData.Stream)
            .WithObjectSize(fileData.Stream.Length)
            .WithObject(fileData.Info.FilePath.Path);

        try
        {
            await _minioClient
                .PutObjectAsync(putObjectArgs, cancellationToken);

            return fileData.Info.FilePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to upload file in minio with path = {path} in bucket = {bucket}",
                fileData.Info.FilePath.Path,
                fileData.Info.BucketName);

            return Error.Failure("file.upload", "Failed to upload file in minio");
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }
    
    private async Task<Result<FilePath, Error>> RemoveObject(
        FileInfo fileInfo,
        SemaphoreSlim semaphoreSlim,
        CancellationToken cancellationToken)
    {
        await semaphoreSlim.WaitAsync(cancellationToken);

        var removeObjectArgs = new RemoveObjectArgs()
            .WithBucket(fileInfo.BucketName)
            .WithObject(fileInfo.FilePath.Path);
        
        try
        {
            await _minioClient.RemoveObjectAsync(removeObjectArgs, cancellationToken);

            return fileInfo.FilePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to delete file in minio with path = {path} in bucket = {bucket}",
                fileInfo.FilePath.Path,
                fileInfo.BucketName);

            return Error.Failure("file.delete", "Failed to delete file in minio");
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    
    private async Task IfBucketsNotExistCreateBucket(
        IEnumerable<FileData> filesData,
        CancellationToken cancellationToken)
    {
        HashSet<string> bucketNames = [..filesData.Select(file => file.Info.BucketName)];

        foreach (var bucketName in bucketNames)
        {
            var bucketExistArgs = new BucketExistsArgs()
                .WithBucket(bucketName);

            var bucketExist = await _minioClient
                .BucketExistsAsync(bucketExistArgs, cancellationToken);

            if (bucketExist == false)
            {
                var makeBucketArgs = new MakeBucketArgs()
                    .WithBucket(bucketName);

                await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
            }
        }
    }
}