using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using FileService.API;
using FileService.Core;
using FileService.Endpoints;
using FileService.Infrastructure.Repositories;

namespace FileService.Features;

public static class CompleteMultipartUpload
{
    private record PartETagInfo(int PartNumber, string ETag);

    private record CompleteMultipartRequest(string UploadId, List<PartETagInfo> Parts);


    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("files/{key}/complete-multipart", Handler);
        }
    }

    private static async Task<IResult> Handler(
        string key,
        CompleteMultipartRequest request,
        IAmazonS3 s3Client,
        IFileRepository fileRepository,
        CancellationToken cancellationToken = default)
    {
        var fileId = Guid.NewGuid();

        var completeRequest = new CompleteMultipartUploadRequest()
        {
            BucketName = "bucket",
            Key = key,
            UploadId = request.UploadId,
            PartETags = request.Parts.Select(p => new PartETag(p.PartNumber, p.ETag)).ToList()
        };

        var response = await s3Client.CompleteMultipartUploadAsync(
            completeRequest,
            cancellationToken);

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
            location = response.Location
        });
    }
}