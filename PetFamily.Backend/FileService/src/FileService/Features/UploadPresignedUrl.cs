using FileService.API;
using FileService.API.Endpoints;
using FileService.Core.Models;
using FileService.Infrastructure.Providers;
using FileService.Infrastructure.Repositories;
using FileService.Jobs;
using Hangfire;

namespace FileService.Features;

public static class UploadPresignedUrl
{
    public record UploadPresignedUrlRequest(
        string FileName,
        string ContentType,
        long Size);

    public record UploadResponse(string Key, string Url);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("files/presigned", Handler);
        }
    }

    private static async Task<IResult> Handler(
        UploadPresignedUrlRequest request,
        IFileProvider fileProvider,
        IFileRepository fileRepository,
        CancellationToken cancellationToken = default)
    {
        var response = await fileProvider.GenerateUploadUrl(request);

        var fileId = Guid.NewGuid();
        
        CreateJobs(fileId, response.Key, cancellationToken);

        var fileData = new FileData
        {
            Id = fileId,
            StoragePath = response.Key,
            Size = request.Size,
            ContentType = request.ContentType,
            UploadDate = DateTime.UtcNow
        };
        
        await fileRepository.Add(fileData, cancellationToken);

        return CustomResponses.Ok(response);
    }

    private static void CreateJobs(Guid fileId, string key, CancellationToken cancellationToken)
    {
        var clearJobId = BackgroundJob.Schedule<StoragesCleanerJob>(j =>
                j.Execute(fileId, key, cancellationToken),
            TimeSpan.FromHours(24));

        BackgroundJob.Schedule<ConfirmConsistencyJob>(j =>
                j.Execute(fileId, key, clearJobId, cancellationToken),
            TimeSpan.FromSeconds(60));
    }
}