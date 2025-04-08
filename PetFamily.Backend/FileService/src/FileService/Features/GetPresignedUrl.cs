using FileService.API;
using FileService.Endpoints;
using FileService.Infrastructure.Providers;
using FileService.Infrastructure.Repositories;

namespace FileService.Features;

public static class GetPresignedUrl
{
    private record ExtendedFileInfo(
        Guid Id,
        string Key,
        string Url,
        DateTime UploadDate,
        long Size,
        string ContentType);
    
    public record ProviderGetResponse(string Key, string Url);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("files/presigned", Handler);
        }
    }

    private static async Task<IResult> Handler(
        Guid[] fileIds,
        IFileProvider fileProvider,
        IFileRepository fileRepository,
        CancellationToken cancellationToken = default)
    {
        var filesData = await fileRepository.Get(fileIds, cancellationToken);
        if (filesData.IsFailure)
            return CustomResponses.Errors(filesData.Error);

        var providerResponse = await fileProvider
            .GenerateGetUrls(filesData.Value.Select(f => f.StoragePath).ToList(), cancellationToken);
        if (providerResponse.IsFailure)
            return CustomResponses.Errors(providerResponse.Error);

        var response = filesData.Value.Join(
            providerResponse.Value,
            fileData => fileData.StoragePath,
            providerData => providerData.Key,
            (file, provider) => new ExtendedFileInfo(
                file.Id,
                provider.Key,
                provider.Url,
                file.UploadDate,
                file.Size,
                file.ContentType
            )
        ).ToList();

        return CustomResponses.Ok(response);
    }
}