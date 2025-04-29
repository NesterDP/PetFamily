using FileService.API.Endpoints;
using FileService.Contracts.SubModels;
using FileService.Core.Models;
using FileService.Infrastructure.Providers;
using FileService.Infrastructure.Repositories;
using FileService.Jobs;
using FileService.Security.Authorization;
using Hangfire;
using CompleteMultipartUploadRequest = FileService.Contracts.Requests.CompleteMultipartUploadRequest;
using CompleteMultipartUploadResponse = FileService.Contracts.Responses.CompleteMultipartUploadResponse;

namespace FileService.Features;

public class CompleteMultipartUpload
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("files/complete-multipart", Handler)
                .RequirePermission("fileservice.CompleteMultipartUpload");
        }
    }

    private static async Task<IResult> Handler(
        CompleteMultipartUploadRequest request,
        IFilesProvider filesProvider,
        IFilesRepository filesRepository,
        ILogger<CompleteMultipartUpload> logger,
        CancellationToken cancellationToken = default)
    {
        var fileInfos = await filesProvider
            .GenerateCompeteMultipartUploadData(request.ClientInfos, cancellationToken);

        var response = new CompleteMultipartUploadResponse([]);

        foreach (var fileInfo in fileInfos)
        {
            var fileId = Guid.NewGuid();

            var fileData = new FileData
            {
                Id = fileId,
                StoragePath = fileInfo.Key,
                Size = fileInfo.ContentLength,
                ContentType = fileInfo.ContentType,
                UploadDate = DateTime.UtcNow
            };

            string? clearJobId = BackgroundJob.Schedule<StoragesCleanerJob>(
                j =>
                    j.Execute(fileId, fileInfo.Key, cancellationToken),
                TimeSpan.FromHours(24));

            try
            {
                await filesRepository.Add(fileData, cancellationToken);

                BackgroundJob.Delete(clearJobId);

                response.MultipartCompleteInfos.Add(
                    new MultipartCompleteFileInfo(
                        fileId,
                        fileInfo.Key,
                        fileInfo.ContentType,
                        fileInfo.ContentLength));
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
            }
        }

        // Вернет Id и метаданные загруженных файлов
        // Если не удалось загрузить ни одного файла - вернет пустой список
        return Results.Ok(response);
    }
}