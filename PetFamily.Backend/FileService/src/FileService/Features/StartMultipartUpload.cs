using FileService.API;
using FileService.API.Endpoints;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
using FileService.Infrastructure.Providers;
using FileService.Security.Authorization;

namespace FileService.Features;

public static class StartMultipartUpload
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("files/start-multipart", Handler)
                .RequirePermission("fileservice.StartMultipartUpload");
        }
    }

    private static async Task<IResult> Handler(
        StartMultipartUploadRequest request,
        IFilesProvider filesProvider,
        CancellationToken cancellationToken = default)
    {
        var providerResponse = await filesProvider
            .GenerateStartingMultipartUploadData(request.StartMultipartClientInfos, cancellationToken);

        if (providerResponse.IsFailure)
            return CustomResponses.Errors(providerResponse.Error);

        var response = new StartMultipartUploadResponse(providerResponse.Value);

        return Results.Ok(response);
    }
}