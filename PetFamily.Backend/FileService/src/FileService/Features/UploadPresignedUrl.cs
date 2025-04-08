using Amazon.S3;
using Amazon.S3.Model;
using FileService.API;
using FileService.Core;
using FileService.Endpoints;
using FileService.Infrastructure.Repositories;

namespace FileService.Features;

public static class UploadPresignedUrl
{
    private record UploadPresignedUrlRequest(
        string FileName,
        string ContentType);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("files/presigned", Handler);
        }
    }

    private static async Task<IResult> Handler(
        UploadPresignedUrlRequest request,
        IAmazonS3 s3Client,
        IFileRepository fileRepository,
        CancellationToken cancellationToken = default)
    {
        var fileId = Guid.NewGuid();
        
        var key = $"{request.ContentType}/{Guid.NewGuid()}";

        var presignedRequest = new GetPreSignedUrlRequest
        {
            BucketName = "bucket",
            Key = key,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddHours(24),
            ContentType = request.ContentType,
            Protocol = Protocol.HTTP,
            Metadata =
            {
                ["file-name"] = request.FileName
            }
        };

        var presignedUrl = await s3Client.GetPreSignedURLAsync(presignedRequest);
        
        var metaDataRequest = new GetObjectMetadataRequest()
        {
            BucketName = "bucket",
            Key = key
        };

        var metadata = await s3Client.GetObjectMetadataAsync(metaDataRequest, cancellationToken);
        
        var fileData = new FileData
        {
            Id = fileId,
            StoragePath = key,
            Size = metadata.Headers.ContentLength,
            ContentType = metadata.Headers.ContentType,
            UploadDate = DateTime.UtcNow
        };

        await fileRepository.Add(fileData, cancellationToken);

        return CustomResponses.Ok(new
        {
            key,
            url = presignedUrl
        });
    }
}