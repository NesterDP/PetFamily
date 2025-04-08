using FileService.API;
using FileService.API.Endpoints;
using FileService.Infrastructure.Providers;

namespace FileService.Features;

public static class StartMultipartUpload
{
    public record StartMultipartUploadRequest(string FileName, string ContentType);
    
    public record StartMultipartUploadResponse(string Key, string UploadId);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("files/multipart", Handler);
        }
    }

    private static async Task<IResult> Handler(
        StartMultipartUploadRequest request,
        IFileProvider fileProvider,
        CancellationToken cancellationToken = default)
    {
        var response = await fileProvider.GenerateStartingMultipartUploadData(request, cancellationToken);

        return CustomResponses.Ok(response);
    }
}