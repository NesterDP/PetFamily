using FileService.API;
using FileService.API.Endpoints;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
using FileService.Contracts.SubModels;
using FileService.Infrastructure.Providers;
using FileService.Infrastructure.Repositories;
using FileService.Security.Authorization;

namespace FileService.Features;

public static class GetFilesPresignedUrls
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("files/presigned-urls", Handler)
                .RequirePermission("fileservice.GetFilesPresignedUrls");
        }
    }

    private static async Task<IResult> Handler(
        GetFilesPresignedUrlsRequest request,
        IFilesProvider filesProvider,
        IFilesRepository filesRepository,
        CancellationToken cancellationToken = default)
    {
        var filesData = await filesRepository.Get(request.FileIds, cancellationToken);
        if (filesData.IsFailure)
            return CustomResponses.Errors(filesData.Error);

        var providerResponse = await filesProvider
            .GenerateGetUrls(filesData.Value.Select(f => f.StoragePath).ToList(), cancellationToken);
        if (providerResponse.IsFailure)
            return CustomResponses.Errors(providerResponse.Error);

        var extendedFileInfos = filesData.Value.Join(
                providerResponse.Value,
                fileData => fileData.StoragePath,
                providerData => providerData.Key,
                (fileData, providerData) => new ExtendedFileInfo(
                    fileData.Id,
                    providerData.Key,
                    providerData.Url,
                    fileData.UploadDate,
                    fileData.Size,
                    fileData.ContentType))
            .ToList();

        var response = new GetFilesPresignedUrlsResponse(extendedFileInfos);

        return Results.Ok(response);
    }
}