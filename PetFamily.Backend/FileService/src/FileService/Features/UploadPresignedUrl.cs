using FileService.API;
using FileService.Core;
using FileService.Endpoints;
using FileService.Infrastructure.Providers;
using FileService.Infrastructure.Repositories;
using FileService.Jobs;
using Hangfire;

namespace FileService.Features;

public static class UploadPresignedUrl
{
    public record UploadPresignedUrlRequest(
        string FileName,
        string ContentType,
        long Size);
    
    public record UploadResponse(string Key, string Url);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("files/presigned", Handler);
        }
    }

    private static async Task<IResult> Handler(
        UploadPresignedUrlRequest request,
        IFileProvider fileProvider,
        IFileRepository fileRepository,
        CancellationToken cancellationToken = default)
    {
        var response = await fileProvider.GenerateUploadUrl(request);
        
        var fileId = Guid.NewGuid();

        var fileData = new FileData
        {
            Id = fileId,
            StoragePath = response.Key,
            Size = request.Size,
            ContentType = request.ContentType,
            UploadDate = DateTime.UtcNow
        };

        await fileRepository.Add(fileData, cancellationToken);
        
        // TODO: place job that checks if file was actually uploaded in s3 storage, if not - delete record from mongoDB
        
        return CustomResponses.Ok(response);
    }
}