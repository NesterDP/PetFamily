using FileService.API;
using FileService.API.Endpoints;
using FileService.Infrastructure.Providers;

namespace FileService.Features;

public static class UploadPresignedUrlPart
{
    public record UploadPresignedUrlPartRequest(string UploadId, int PartNumber);

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
        IFileProvider fileProvider,
        CancellationToken cancellationToken = default)
    {

        var response = await fileProvider.GenerateUploadUrlPart(request, key);

        return CustomResponses.Ok(response);
    }
}