using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using PetFamily.Application.FilesProvider.FilesData;
using PetFamily.Application.Providers;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;
using PetFamily.Infrastructure.Options;

namespace PetFamily.Infrastructure.Providers;

public class MinioProvider : IFilesProvider
{
    private readonly IMinioClient _minioClient;
    private readonly ILogger<MinioOptions> _logger;

    private const int MAX_DEGREE_OF_PARALLELISM = 10;

    public MinioProvider(
        IMinioClient minioClient,
        ILogger<MinioOptions> logger)
    {
        _minioClient = minioClient;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<FilePath>, Error>> DeleteFile(
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
                .WithObject(deleteData.FilePath);
            var objectStat = await _minioClient.StatObjectAsync(objExist, cancellationToken);

            if (objectStat.Size == 0)
                return Error.Failure("error.object.exist", "No such object exists");

            var args = new RemoveObjectArgs()
                .WithBucket(deleteData.BucketName)
                .WithObject(deleteData.FilePath);
            await _minioClient.RemoveObjectAsync(args, cancellationToken);

            _logger.LogInformation("File in bucket = {Bucket} with name = {Name} was deleted",
                deleteData.BucketName, deleteData.FilePath);

            return new List<FilePath>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove file");
            return Error.Failure("error.file.remove", "Failed to remove file");
        }
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

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Fail to upload files in minio, files amount: {amount}", filesList.Count);

            return Error.Failure("file.upload", "Fail to upload files in minio");
        }
    }
    
    public async Task<Result<IReadOnlyList<string>, Error>> DeleteFiles(
        IEnumerable<DeleteData> filesData,
        CancellationToken cancellationToken = default)
    {
        var semaphoreSlim = new SemaphoreSlim(MAX_DEGREE_OF_PARALLELISM);
        var filesList = filesData.ToList();

        try
        {
            var tasks = filesList.Select(async file =>
                await RemoveObject(file, semaphoreSlim, cancellationToken));

            var pathsResult = await Task.WhenAll(tasks);

            if (pathsResult.Any(p => p.IsFailure))
                return pathsResult.First().Error;

            var results = pathsResult.Select(p => p.Value).ToList();

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to delete files in minio, files amount: {amount}", filesList.Count);

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
            .WithBucket(fileData.BucketName)
            .WithStreamData(fileData.Stream)
            .WithObjectSize(fileData.Stream.Length)
            .WithObject(fileData.FilePath.Path);

        try
        {
            await _minioClient
                .PutObjectAsync(putObjectArgs, cancellationToken);

            return fileData.FilePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to upload file in minio with path = {path} in bucket = {bucket}",
                fileData.FilePath.Path,
                fileData.BucketName);

            return Error.Failure("file.upload", "Failed to upload file in minio");
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }
    
    private async Task<Result<string, Error>> RemoveObject(
        DeleteData fileData,
        SemaphoreSlim semaphoreSlim,
        CancellationToken cancellationToken)
    {
        await semaphoreSlim.WaitAsync(cancellationToken);

        var removeObjectArgs = new RemoveObjectArgs()
            .WithBucket(fileData.BucketName)
            .WithObject(fileData.FilePath);
        
        try
        {
            await _minioClient.RemoveObjectAsync(removeObjectArgs, cancellationToken);

            return fileData.FilePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to delete file in minio with path = {path} in bucket = {bucket}",
                fileData.FilePath,
                fileData.BucketName);

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
        HashSet<string> bucketNames = [..filesData.Select(file => file.BucketName)];

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