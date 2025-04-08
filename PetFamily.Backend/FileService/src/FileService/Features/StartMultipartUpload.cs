using Amazon.S3;
using Amazon.S3.Model;
using FileService.API;
using FileService.Endpoints;

namespace FileService.Features;

public static class StartMultipartUpload
{
    private record StartMultipartUploadRequest(
        string FileName,
        string ContentType,
        long Size);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("files/multipart", Handler);
        }
    }

    private static async Task<IResult> Handler(
        StartMultipartUploadRequest request,
        IAmazonS3 s3Client,
        CancellationToken cancellationToken = default)
    {
        var key = $"{request.ContentType}/{Guid.NewGuid()}";

        var startMultipartRequest = new InitiateMultipartUploadRequest
        {
            BucketName = "bucket",
            Key = key,
            ContentType = request.ContentType,
            Metadata =
            {
                ["file-name"] = request.FileName
            }
        };

        var response = await s3Client.InitiateMultipartUploadAsync(
            startMultipartRequest,
            cancellationToken);

        return CustomResponses.Ok(new
        {
            key,
            uploadId = response.UploadId
        });
    }
}