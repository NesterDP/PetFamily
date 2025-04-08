using Amazon.S3;
using Amazon.S3.Model;
using FileService.API;
using FileService.Endpoints;

namespace FileService.Features;

public static class UploadPresignedPartUrl
{
    private record UploadPresignedPartUrlRequest(string UploadId, int PartNumber);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("files/{key}/presigned-part", Handler);
        }
    }

    private static async Task<IResult> Handler(
        string key,
        UploadPresignedPartUrlRequest request,
        IAmazonS3 s3Client,
        CancellationToken cancellationToken = default)
    {
        var presignedRequest = new GetPreSignedUrlRequest
        {
            BucketName = "bucket",
            Key = key,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddHours(24),
            Protocol = Protocol.HTTP,
            UploadId = request.UploadId,
            PartNumber = request.PartNumber
        };

        var presignedUrl = await s3Client.GetPreSignedURLAsync(presignedRequest);

        return CustomResponses.Ok(new
        {
            key,
            url = presignedUrl
        });
    }
}