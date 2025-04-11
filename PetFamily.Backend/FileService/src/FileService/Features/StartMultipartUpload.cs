using FileService.API;
using FileService.API.Endpoints;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
using FileService.Infrastructure.Providers;

namespace FileService.Features;

public static class StartMultipartUpload
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("files/multipart", Handler);
        }
    }

    private static async Task<IResult> Handler(
        StartMultipartUploadRequest request,
        IFilesProvider filesProvider,
        CancellationToken cancellationToken = default)
    {
        var providerResponse = await filesProvider.GenerateStartingMultipartUploadData(request, cancellationToken);

        var response = new StartMultipartUploadResponse(providerResponse.Key, providerResponse.UploadId);

        return CustomResponses.Ok(response);
    }
}