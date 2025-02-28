using Microsoft.AspNetCore.Mvc;
using Minio;
using PetFamily.API.Controllers.FilesTest.Requests;
using PetFamily.API.Extensions;
using PetFamily.API.Processors;
using PetFamily.Application.Files.Delete;
using PetFamily.Application.Files.Upload;

namespace PetFamily.API.Controllers.FilesTest;

public class FilesTestController : ApplicationController
{
    private readonly IMinioClient _minioClient;
    private const string bucketName = "TEST";

    public FilesTestController(IMinioClient minioClient)
    {
        _minioClient = minioClient;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Upload(
        [FromForm] UploadFilesRequest request,
        [FromServices] UploadFilesHandler handler,
        CancellationToken cancellationToken)
    {
        await using var fileProcessor = new FormFileProcessor();
        var fileDtos = fileProcessor.Process(request.Files);
        var command = new UploadFileCommand(fileDtos);
        
        var result = await handler.HandleAsync(command, cancellationToken);
        
        if (result.IsFailure)
            return result.Error.ToResponse();

        return result.ToResponse();
    }

    [HttpDelete]
    public async Task<ActionResult<Guid>> Delete(
        [FromForm] DeleteFilesRequest request,
        [FromServices] DeleteFilesHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new DeleteFilesCommand(request.PhotosNames.ToList());

        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return result.ToResponse();
    }
}