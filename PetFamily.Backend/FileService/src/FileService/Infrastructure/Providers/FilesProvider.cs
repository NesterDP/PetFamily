using Amazon.S3;
using Amazon.S3.Model;
using CSharpFunctionalExtensions;
using FileService.Contracts.Requests;
using FileService.Contracts.SubModels;
using FileService.Core.CustomErrors;
using CompleteMultipartUploadRequest = FileService.Contracts.Requests.CompleteMultipartUploadRequest;

namespace FileService.Infrastructure.Providers;

public class FilesProvider : IFilesProvider
{
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<FilesProvider> _logger;
    private const string BUCKET_NAME = "bucket";
    private const string FILENAME_METADATA = "file-name";
    private const int EXPIRATION_HOURS = 24;
    private const int MAX_DEGREE_OF_PARALLELISM = 10;

    public FilesProvider(IAmazonS3 s3Client, ILogger<FilesProvider> logger)
    {
        _s3Client = s3Client;
        _logger = logger;
    }

    public async Task ConfirmExistence(string key)
    {
        var request = new GetObjectMetadataRequest
        {
            BucketName = BUCKET_NAME,
            Key = key,
        };
        
        await _s3Client.GetObjectMetadataAsync(request);
    }

    public async Task<MinimalFileInfo> GenerateUploadUrl(UploadPresignedUrlRequest request)
    {
        var key = $"{request.ContentType}/{Guid.NewGuid()}";

        var presignedRequest = new GetPreSignedUrlRequest
        {
            BucketName = BUCKET_NAME,
            Key = key,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddHours(EXPIRATION_HOURS),
            ContentType = request.ContentType,
            Protocol = Protocol.HTTP,
            Metadata =
            {
                [FILENAME_METADATA] = request.FileName
            }
        };

        var presignedUrl = await _s3Client.GetPreSignedURLAsync(presignedRequest);
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

        var presignedUrl = await _s3Client.GetPreSignedURLAsync(presignedRequest);

        return new MinimalFileInfo(key, presignedUrl);
    }
    
    public async Task<GetObjectMetadataResponse> GenerateCompeteMultipartUploadData(
        CompleteMultipartUploadRequest uploadRequest,
        string key,
        CancellationToken cancellationToken)
    {
        var completeRequest = new Amazon.S3.Model.CompleteMultipartUploadRequest()
        {
            BucketName = BUCKET_NAME,
            Key = key,
            UploadId = uploadRequest.UploadId,
            PartETags = uploadRequest.Parts.Select(p => new PartETag(p.PartNumber, p.ETag)).ToList()
        };

        await _s3Client.CompleteMultipartUploadAsync(completeRequest, cancellationToken);

        var metaDataRequest = new GetObjectMetadataRequest()
        {
            BucketName = BUCKET_NAME,
            Key = key
        };

        var metadata = await _s3Client.GetObjectMetadataAsync(metaDataRequest, cancellationToken);

        return metadata;
    }

    public async Task<List<string>> DeleteFiles(
        List<string> keys,
        CancellationToken cancellationToken)
    {
        var request = new DeleteObjectsRequest();
        request.BucketName = BUCKET_NAME;

        foreach (var key in keys)
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

        var tasks = keys.Select(async file =>
            await GenerateGetUrl(file, semaphoreSlim, cancellationToken));

        var urlsResult = await Task.WhenAll(tasks);

        if (urlsResult.Any(p => p.IsFailure))
            return urlsResult.First().Error;

        var results = urlsResult.Select(p => p.Value).ToList();

        return results;
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
            var url = await _s3Client.GetPreSignedURLAsync(presignedRequest);
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
    
    public async Task<Result<List<MultipartStartProviderInfo>, Error>> GenerateStartingMultipartUploadData(
        List<MultipartStartClientInfo> clientInfos,
        CancellationToken cancellationToken)
    {
        var semaphoreSlim = new SemaphoreSlim(MAX_DEGREE_OF_PARALLELISM);

        var tasks = clientInfos.Select(async clientInfo =>
            await GenerateMultipartStartProviderInfo(clientInfo, semaphoreSlim, cancellationToken));

        var infosResult = await Task.WhenAll(tasks);

        if (infosResult.Any(p => p.IsFailure))
            return infosResult.First().Error;

        var results = infosResult.Select(p => p.Value).ToList();

        return results;
    }
    
    private async Task<Result<MultipartStartProviderInfo, Error>> GenerateMultipartStartProviderInfo(
        MultipartStartClientInfo clientInfo,
        SemaphoreSlim semaphoreSlim,
        CancellationToken cancellationToken)
    {
        await semaphoreSlim.WaitAsync(cancellationToken);

        var key = $"{clientInfo.ContentType}/{Guid.NewGuid()}";

        var startMultipartRequest = new InitiateMultipartUploadRequest
        {
            BucketName = BUCKET_NAME,
            Key = key,
            ContentType = clientInfo.ContentType,
            Metadata =
            {
                [FILENAME_METADATA] = clientInfo.FileName
            }
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