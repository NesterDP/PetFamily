using Amazon.S3;
using Amazon.S3.Model;
using CSharpFunctionalExtensions;
using FileService.Contracts.Requests;
using FileService.Contracts.SubModels;
using FileService.Core.CustomErrors;

namespace FileService.Infrastructure.Providers;

public class FilesProvider : IFilesProvider
{
    private const string BUCKET_NAME = "bucket";
    private const string FILENAME_METADATA = "file-name";
    private const int EXPIRATION_HOURS = 24;
    private const int MAX_DEGREE_OF_PARALLELISM = 10;
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<FilesProvider> _logger;

    public FilesProvider(IAmazonS3 s3Client, ILogger<FilesProvider> logger)
    {
        _s3Client = s3Client;
        _logger = logger;
    }

    public async Task ConfirmExistence(string key)
    {
        var request = new GetObjectMetadataRequest { BucketName = BUCKET_NAME, Key = key, };

        await _s3Client.GetObjectMetadataAsync(request);
    }

    public async Task<MinimalFileInfo> GenerateUploadUrl(UploadPresignedUrlRequest request)
    {
        string key = $"{request.ContentType}/{Guid.NewGuid()}";

        var presignedRequest = new GetPreSignedUrlRequest
        {
            BucketName = BUCKET_NAME,
            Key = key,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddHours(EXPIRATION_HOURS),
            ContentType = request.ContentType,
            Protocol = Protocol.HTTP,
            Metadata = { [FILENAME_METADATA] = request.FileName }
        };

        string? presignedUrl = await _s3Client.GetPreSignedURLAsync(presignedRequest);
        return new MinimalFileInfo(key, presignedUrl);
    }

    public async Task<MinimalFileInfo> GenerateUploadUrlPart(
        UploadPresignedUrlPartRequest request,
        string key)
    {
        var presignedRequest = new GetPreSignedUrlRequest
        {
            BucketName = BUCKET_NAME,
            Key = key,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddHours(EXPIRATION_HOURS),
            Protocol = Protocol.HTTP,
            UploadId = request.UploadId,
            PartNumber = request.PartNumber
        };

        string? presignedUrl = await _s3Client.GetPreSignedURLAsync(presignedRequest);

        return new MinimalFileInfo(key, presignedUrl);
    }

    public async Task<List<string>> DeleteFiles(
        List<string> keys,
        CancellationToken cancellationToken)
    {
        var request = new DeleteObjectsRequest { BucketName = BUCKET_NAME };

        foreach (string key in keys)
        {
            request.AddKey(key);
        }

        await _s3Client.DeleteObjectsAsync(request, cancellationToken);

        return keys;
    }

    public async Task<Result<List<MinimalFileInfo>, Error>> GenerateGetUrls(
        List<string> keys,
        CancellationToken cancellationToken)
    {
        var semaphoreSlim = new SemaphoreSlim(MAX_DEGREE_OF_PARALLELISM);

        var tasks = keys.Select(
            async file =>
                await GenerateGetUrl(file, semaphoreSlim, cancellationToken));

        var taskResults = await Task.WhenAll(tasks);

        if (taskResults.Any(p => p.IsFailure))
            return taskResults.First().Error;

        var result = taskResults.Select(p => p.Value).ToList();

        return result;
    }

    public async Task<Result<List<MultipartStartProviderInfo>, Error>> GenerateStartingMultipartUploadData(
        List<MultipartStartClientInfo> clientInfos,
        CancellationToken cancellationToken)
    {
        var semaphoreSlim = new SemaphoreSlim(MAX_DEGREE_OF_PARALLELISM);

        var tasks = clientInfos.Select(
            async clientInfo =>
                await GenerateMultipartStartProviderInfo(clientInfo, semaphoreSlim, cancellationToken));

        var tasksResults = await Task.WhenAll(tasks);

        if (tasksResults.Any(p => p.IsFailure))
            return tasksResults.First().Error;

        var result = tasksResults.Select(p => p.Value).ToList();

        return result;
    }

    public async Task<List<MultipartCompleteProviderInfo>> GenerateCompeteMultipartUploadData(
        List<MultipartCompleteClientInfo> clientInfos,
        CancellationToken cancellationToken)
    {
        var semaphoreSlim = new SemaphoreSlim(MAX_DEGREE_OF_PARALLELISM);

        var tasks = clientInfos.Select(
            async clientInfo =>
                await GenerateMultipartCompleteProviderInfo(clientInfo, semaphoreSlim, cancellationToken));

        var tasksResults = await Task.WhenAll(tasks);

        var result = new List<MultipartCompleteProviderInfo>();

        foreach (var taskResult in tasksResults)
        {
            if (taskResult.IsSuccess)
                result.Add(taskResult.Value);
        }

        return result;
    }

    private async Task<Result<MultipartCompleteProviderInfo, Error>> GenerateMultipartCompleteProviderInfo(
        MultipartCompleteClientInfo clientInfo,
        SemaphoreSlim semaphoreSlim,
        CancellationToken cancellationToken)
    {
        await semaphoreSlim.WaitAsync(cancellationToken);

        var completeRequest = new Amazon.S3.Model.CompleteMultipartUploadRequest()
        {
            BucketName = BUCKET_NAME,
            Key = clientInfo.Key,
            UploadId = clientInfo.UploadId,
            PartETags = clientInfo.Parts.Select(p => new PartETag(p.PartNumber, p.ETag)).ToList()
        };

        try
        {
            await _s3Client.CompleteMultipartUploadAsync(completeRequest, cancellationToken);

            var metaDataRequest = new GetObjectMetadataRequest() { BucketName = BUCKET_NAME, Key = clientInfo.Key };

            var metadata = await _s3Client.GetObjectMetadataAsync(metaDataRequest, cancellationToken);

            var providerInfo = new MultipartCompleteProviderInfo(
                clientInfo.Key,
                metadata.Headers.ContentLength,
                metadata.Headers.ContentType);

            return providerInfo;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to generate complete multipart upload data");
            return Error.Failure("file.get", "Failed to generate starting multipart upload data");
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    private async Task<Result<MinimalFileInfo, Error>> GenerateGetUrl(
        string key,
        SemaphoreSlim semaphoreSlim,
        CancellationToken cancellationToken)
    {
        await semaphoreSlim.WaitAsync(cancellationToken);

        var presignedRequest = new GetPreSignedUrlRequest
        {
            BucketName = BUCKET_NAME,
            Key = key,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.AddHours(EXPIRATION_HOURS),
            Protocol = Protocol.HTTP
        };

        try
        {
            string? url = await _s3Client.GetPreSignedURLAsync(presignedRequest);
            return new MinimalFileInfo(key, url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate url for file with key = {key}", key);
            return Error.Failure("file.get", "Failed generate url");
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    private async Task<Result<MultipartStartProviderInfo, Error>> GenerateMultipartStartProviderInfo(
        MultipartStartClientInfo clientInfo,
        SemaphoreSlim semaphoreSlim,
        CancellationToken cancellationToken)
    {
        await semaphoreSlim.WaitAsync(cancellationToken);

        string key = $"{clientInfo.ContentType}/{Guid.NewGuid()}";

        var startMultipartRequest = new InitiateMultipartUploadRequest
        {
            BucketName = BUCKET_NAME,
            Key = key,
            ContentType = clientInfo.ContentType,
            Metadata = { [FILENAME_METADATA] = clientInfo.FileName }
        };

        try
        {
            var response = await _s3Client.InitiateMultipartUploadAsync(startMultipartRequest, cancellationToken);

            return new MultipartStartProviderInfo(key, response.UploadId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate starting multipart upload data");
            return Error.Failure("file.get", "Failed to generate starting multipart upload data");
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }
}