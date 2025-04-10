using Amazon.S3;
using Amazon.S3.Model;
using CSharpFunctionalExtensions;
using FileService.Core.CustomErrors;
using UploadPresignedUrlRequest = FileService.Features.UploadPresignedUrl.UploadPresignedUrlRequest;
using StartMultipartUploadRequest = FileService.Features.StartMultipartUpload.StartMultipartUploadRequest;
using UploadPresignedUrlPartRequest = FileService.Features.UploadPresignedUrlPart.UploadPresignedUrlPartRequest;
using CompleteMultipartRequest = FileService.Features.CompleteMultipartUpload.CompleteMultipartRequest;
using StartMultipartUploadResponse = FileService.Features.StartMultipartUpload.StartMultipartUploadResponse;
using UploadResponse = FileService.Features.UploadPresignedUrl.UploadResponse;
using ProviderGetResponse = FileService.Features.GetPresignedUrl.ProviderGetResponse;

namespace FileService.Infrastructure.Providers;

public class FileProvider : IFileProvider
{
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<FileProvider> _logger;
    private const string BUCKET_NAME = "bucket";
    private const string FILENAME_METADATA = "file-name";
    private const int EXPIRATION_HOURS = 24;
    private const int MAX_DEGREE_OF_PARALLELISM = 10;

    public FileProvider(IAmazonS3 s3Client, ILogger<FileProvider> logger)
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

    public async Task<UploadResponse> GenerateUploadUrl(UploadPresignedUrlRequest request)
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
        return new UploadResponse(key, presignedUrl);
    }

    public async Task<UploadResponse> GenerateUploadUrlPart(
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

        return new UploadResponse(key, presignedUrl);
    }

    public async Task<StartMultipartUploadResponse> GenerateStartingMultipartUploadData(
        StartMultipartUploadRequest request,
        CancellationToken cancellationToken)
    {
        var key = $"{request.ContentType}/{Guid.NewGuid()}";

        var startMultipartRequest = new InitiateMultipartUploadRequest
        {
            BucketName = BUCKET_NAME,
            Key = key,
            ContentType = request.ContentType,
            Metadata =
            {
                [FILENAME_METADATA] = request.FileName
            }
        };

        var response = await _s3Client.InitiateMultipartUploadAsync(
            startMultipartRequest,
            cancellationToken);


        return new StartMultipartUploadResponse(key, response.UploadId);
    }

    public async Task<GetObjectMetadataResponse> GenerateCompeteMultipartUploadData(
        CompleteMultipartRequest request,
        string key,
        CancellationToken cancellationToken)
    {
        var completeRequest = new CompleteMultipartUploadRequest()
        {
            BucketName = BUCKET_NAME,
            Key = key,
            UploadId = request.UploadId,
            PartETags = request.Parts.Select(p => new PartETag(p.PartNumber, p.ETag)).ToList()
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

    public async Task<Result<List<ProviderGetResponse>, Error>> GenerateGetUrls(
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


    private async Task<Result<ProviderGetResponse, Error>> GenerateGetUrl(
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
            return new ProviderGetResponse(key, url);
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
}