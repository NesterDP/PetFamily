using FileService.API;
using FileService.API.Endpoints;
using FileService.Core.Models;
using FileService.Infrastructure.Providers;
using FileService.Infrastructure.Repositories;
using FileService.Jobs;
using Hangfire;
using CompleteMultipartUploadRequest = FileService.Contracts.Requests.CompleteMultipartUploadRequest;
using CompleteMultipartUploadResponse = FileService.Contracts.Responses.CompleteMultipartUploadResponse;

namespace FileService.Features;

public static class CompleteMultipartUpload
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("files/complete-multipart", Handler);
        }
    }

    private static async Task<IResult> Handler(
        string key,
        CompleteMultipartUploadRequest uploadRequest,
        IFilesProvider filesProvider,
        IFilesRepository filesRepository,
        CancellationToken cancellationToken = default)
    {
        var metadata = await filesProvider.GenerateCompeteMultipartUploadData(uploadRequest, key, cancellationToken);

        var fileId = Guid.NewGuid();

        var fileData = new FileData
        {
            Id = fileId,
            StoragePath = key,
            Size = metadata.Headers.ContentLength,
            ContentType = metadata.Headers.ContentType,
            UploadDate = DateTime.UtcNow
        };

        var clearJobId = BackgroundJob.Schedule<StoragesCleanerJob>(j =>
                j.Execute(fileId, key, cancellationToken),
            TimeSpan.FromHours(24));

        await filesRepository.Add(fileData, cancellationToken);

        BackgroundJob.Delete(clearJobId);

        var response = new CompleteMultipartUploadResponse(key);

        return CustomResponses.Ok(response);
    }
}