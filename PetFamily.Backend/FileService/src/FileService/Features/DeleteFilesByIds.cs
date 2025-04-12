using FileService.API;
using FileService.API.Endpoints;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
using FileService.Infrastructure.Providers;
using FileService.Infrastructure.Repositories;

namespace FileService.Features;

public static class DeleteFilesByIds
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("files/deletion", Handler);
        }
    }

    private static async Task<IResult> Handler(
        DeleteFilesByIdsRequest request,
        IFilesProvider filesProvider,
        IFilesRepository filesRepository,
        CancellationToken cancellationToken = default)
    {
        var filesData = await filesRepository.Get(request.Ids, cancellationToken);
        if (filesData.IsFailure)
            return CustomResponses.Errors(filesData.Error);

        await filesRepository.DeleteMany(request.Ids, cancellationToken);

        var providerResponse = await filesProvider
            .DeleteFiles(filesData.Value.Select(f => f.StoragePath).ToList(), cancellationToken);

        var response = new DeleteFilesByIdsResponse(providerResponse);

        return Results.Ok(response);
    }
}