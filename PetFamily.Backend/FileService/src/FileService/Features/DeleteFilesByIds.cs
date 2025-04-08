using FileService.API;
using FileService.Core;
using FileService.Endpoints;
using FileService.Infrastructure.Providers;
using FileService.Infrastructure.Repositories;
using FileService.Jobs;
using Hangfire;

namespace FileService.Features;

public static class DeleteFilesByIds
{
    public record DeletionRequest(Guid[] Ids);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("files/deletion", Handler);
        }
    }

    private static async Task<IResult> Handler(
        DeletionRequest request,
        IFileProvider fileProvider,
        IFileRepository fileRepository,
        CancellationToken cancellationToken = default)
    {
        var filesData = await fileRepository.Get(request.Ids, cancellationToken);
        if (filesData.IsFailure)
            return CustomResponses.Errors(filesData.Error);

        // TODO: schedule some job to check if every file was deleted from both mongoDB and S3 storage

        await fileRepository.DeleteMany(request.Ids, cancellationToken);

        var response = await fileProvider
            .DeleteFiles(filesData.Value.Select(f => f.StoragePath).ToList(), cancellationToken);

        return CustomResponses.Ok(response);
    }
}