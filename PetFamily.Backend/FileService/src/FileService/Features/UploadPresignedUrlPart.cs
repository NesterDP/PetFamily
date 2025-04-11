using FileService.API;
using FileService.API.Endpoints;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
using FileService.Infrastructure.Providers;

namespace FileService.Features;

public static class UploadPresignedUrlPart
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("files/presigned-part", Handler);
        }
    }

    private static async Task<IResult> Handler(
        string key,
        UploadPresignedUrlPartRequest request,
        IFilesProvider filesProvider,
        CancellationToken cancellationToken = default)
    {
        var providerResponse = await filesProvider.GenerateUploadUrlPart(request, key);

        var response = new UploadPresignedUrlPartResponse(providerResponse.Key, providerResponse.Url);

        return CustomResponses.Ok(response);
    }
}