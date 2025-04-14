using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
using FileService.Contracts.SubModels;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Accounts.Application.Commands.StartUploadAvatar;

public class StartUploadAvatarHandler : ICommandHandler<StartMultipartUploadResponse, StartUploadAvatarCommand>
{
    private readonly IValidator<StartUploadAvatarCommand> _validator;
    private readonly ILogger<StartUploadAvatarHandler> _logger;
    private readonly IFileService _fileService;

    public StartUploadAvatarHandler(
        IValidator<StartUploadAvatarCommand> validator,
        ILogger<StartUploadAvatarHandler> logger,
        IFileService fileService)
    {
        _validator = validator;
        _logger = logger;
        _fileService = fileService;
    }

    public async Task<Result<StartMultipartUploadResponse, ErrorList>> HandleAsync(
        StartUploadAvatarCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();

        // межсерверное взаимодействие, получение ссылки на загрузку в S3 хранилище
        var clientInfo = new MultipartStartClientInfo(command.FileInfo.FileName, command.FileInfo.ContentType);
        var request = new StartMultipartUploadRequest([clientInfo]);

        var result = await _fileService.StartMultipartUpload(request, cancellationToken);
        if (result.IsFailure)
            return Errors.General.Failure(result.Error).ToErrorList();

        _logger.LogInformation(
            "Successfully created data for starting multipart upload of avatar for user with ID = {ID}",
            command.UserId);

        return result.Value;
    }
}