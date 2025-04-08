using Amazon.S3;
using Amazon.S3.Model;
using FileService.API;
using FileService.Core;
using FileService.Endpoints;
using FileService.Infrastructure.Providers;
using FileService.Infrastructure.Repositories;
using FileService.Jobs;
using Hangfire;

namespace FileService.Features;

public static class CompleteMultipartUpload
{
    public record PartETagInfo(int PartNumber, string ETag);

    public record CompleteMultipartRequest(string UploadId, List<PartETagInfo> Parts);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("files/complete-multipart", Handler);
        }
    }

    private static async Task<IResult> Handler(
        string key,
        CompleteMultipartRequest request,
        IFileProvider fileProvider,
        IFileRepository fileRepository,
        CancellationToken cancellationToken = default)
    {
        var fileId = Guid.NewGuid();

        // TODO: replace with job that actually deletes file from s3 storage
        var jobId = BackgroundJob.Schedule<ConfirmConsistencyJob>(j =>
            j.Execute(fileId, key), TimeSpan.FromSeconds(5));

        var metadata = await fileProvider.GenerateCompeteMultipartUploadData(request, key, cancellationToken);

        var fileData = new FileData
        {
            Id = fileId,
            StoragePath = key,
            Size = metadata.Headers.ContentLength,
            ContentType = metadata.Headers.ContentType,
            UploadDate = DateTime.UtcNow
        };

        await fileRepository.Add(fileData, cancellationToken);

        BackgroundJob.Delete(jobId);

        return CustomResponses.Ok(key);
    }
}