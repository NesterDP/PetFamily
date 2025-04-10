using FileService.API;
using FileService.API.Endpoints;
using FileService.Core.Models;
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

        var (clearJobId, consistencyJobId) = CreateJobs(key, fileId, cancellationToken);

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
        
        BackgroundJob.Delete(consistencyJobId);
        BackgroundJob.Delete(clearJobId);
        
        return CustomResponses.Ok(key);
    }

    private static (string clearJobId, string consistencyJobId) CreateJobs(
        string key,
        Guid fileId,
        CancellationToken cancellationToken)
    {
        var clearJobId = BackgroundJob.Schedule<StoragesCleanerJob>(j =>
                j.Execute(fileId, key, cancellationToken), 
            TimeSpan.FromHours(24));

        var consistencyJobId = BackgroundJob.Schedule<ConfirmConsistencyJob>(j =>
                j.Execute(fileId, key, clearJobId, cancellationToken),
            TimeSpan.FromSeconds(60));
        
        return (clearJobId, consistencyJobId);
    }
}