using FileService.API;
using FileService.API.Endpoints;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
using FileService.Core.Models;
using FileService.Infrastructure.Providers;
using FileService.Infrastructure.Repositories;
using FileService.Jobs;
using Hangfire;

namespace FileService.Features;

public static class UploadPresignedUrl
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("files/presigned", Handler);
        }
    }

    private static async Task<IResult> Handler(
        UploadPresignedUrlRequest request,
        IFilesProvider filesProvider,
        IFilesRepository filesRepository,
        CancellationToken cancellationToken = default)
    {
        var providerResponse = await filesProvider.GenerateUploadUrl(request);

        var fileId = Guid.NewGuid();

        CreateJobs(fileId, providerResponse.Key, cancellationToken);

        var fileData = new FileData
        {
            Id = fileId,
            StoragePath = providerResponse.Key,
            Size = request.Size,
            ContentType = request.ContentType,
            UploadDate = DateTime.UtcNow
        };

        await filesRepository.Add(fileData, cancellationToken);

        var response = new UploadPresignedUrlResponse(providerResponse.Key, providerResponse.Url);

        return Results.Ok(response);
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