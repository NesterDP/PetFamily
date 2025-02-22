using Microsoft.AspNetCore.Mvc;
using Minio;
using PetFamily.API.Extensions;
using PetFamily.Application.FilesManagement.Delete;
using PetFamily.Application.FilesManagement.FilesData;
using PetFamily.Application.FilesManagement.GetPresignedUrl;
using PetFamily.Application.FilesManagement.Upload;
using PetFamily.Application.TestControllers.DeleteFIle;
using PetFamily.Application.TestControllers.GetPresignedUrl;
using PetFamily.Application.TestControllers.UploadFile;

namespace PetFamily.API.Controllers;

public class FilesController : ApplicationController
{
    private readonly IMinioClient _minioClient;
    private const string bucketName = "photos"; // пока грузим в одно и то же место

    public FilesController(IMinioClient minioClient)
    {
        _minioClient = minioClient;
    }

    [HttpPost]
    public async Task<IActionResult> Upload(
        IFormFile file,
        [FromServices] UploadFileHandler handler,
        CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();
        var objectName = Guid.NewGuid().ToString();

        var uploadData = new UploadData(stream, bucketName, objectName);
        var request = new UploadFileRequest(uploadData);

        var result = await handler.HandleAsync(request, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return result.ToResponse();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        [FromServices] DeleteFileHandler handler,
        CancellationToken cancellationToken)
    {
        var objectName = id.ToString();
        var deleteData = new DeleteData(bucketName, objectName);
        var request = new DeleteFileRequest(deleteData);

        var result = await handler.HandleAsync(request, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return result.ToResponse();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> PresignedUrl(
        [FromRoute] Guid id,
        [FromServices] GetPresignedUrlHandler handler,
        CancellationToken cancellationToken)
    {
        var objectName = id.ToString();
        var getPresignedUrlData = new GetPresignedUrlData(bucketName, objectName);
        var request = new GetPresignedUrlRequest(getPresignedUrlData);

        var result = await handler.HandleAsync(request, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return result.ToResponse();
    }
}