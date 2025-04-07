using Amazon.S3;
using Amazon.S3.Model;
using FileService.API;
using FileService.Endpoints;

namespace FileService.Features;

public static class GetPresignedUrl
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("files/{key:guid}/presigned", Handler);
        }
    }

    private static async Task<IResult> Handler(
        Guid key,
        IAmazonS3 s3Client,
        CancellationToken cancellationToken = default)
    {
        var presignedRequest = new GetPreSignedUrlRequest
        {
            BucketName = "bucket",
            Key = $"videos/{key}",
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.AddHours(24),
            Protocol = Protocol.HTTP
        };

        var presignedUrl = await s3Client.GetPreSignedURLAsync(presignedRequest);

        return CustomResponses.Ok(new
        {
            key,
            url = presignedUrl
        });
    }
}