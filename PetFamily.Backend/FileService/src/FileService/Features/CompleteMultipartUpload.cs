using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using FileService.API;
using FileService.Endpoints;

namespace FileService.Features;

public static class CompleteMultipartUpload
{
    private record PartETagInfo(int PartNumber, string ETag);
    private record CompleteMultipartRequest(string UploadId, List<PartETagInfo> Parts);
    

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("files/{key:guid}/complete-multipart", Handler);
        }
    }

    private static async Task<IResult> Handler(
        Guid key,
        CompleteMultipartRequest request,
        IAmazonS3 s3Client,
        CancellationToken cancellationToken = default)
    {
        var completeRequest = new CompleteMultipartUploadRequest()
        {
            BucketName = "bucket",
            Key = $"videos/{key}",
            UploadId = request.UploadId,
            PartETags = request.Parts.Select(p  => new PartETag(p.PartNumber, p.ETag)).ToList()
        };

        var response = await s3Client.CompleteMultipartUploadAsync(
            completeRequest,
            cancellationToken);

        return CustomResponses.Ok(new
        {
            key,
            location = response.Location
        });
    }
}