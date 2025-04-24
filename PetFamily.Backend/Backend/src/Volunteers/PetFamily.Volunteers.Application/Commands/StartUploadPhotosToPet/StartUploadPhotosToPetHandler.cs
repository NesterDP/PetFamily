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
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Volunteers.Application.Commands.StartUploadPhotosToPet;

public class
    StartUploadPhotosToPetHandler : ICommandHandler<StartMultipartUploadResponse, StartUploadPhotosToPetCommand>
{
    private readonly IValidator<StartUploadPhotosToPetCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ILogger<StartUploadPhotosToPetHandler> _logger;
    private readonly IFileService _fileService;

    public StartUploadPhotosToPetHandler(
        IValidator<StartUploadPhotosToPetCommand> validator,
        IVolunteersRepository volunteersRepository,
        ILogger<StartUploadPhotosToPetHandler> logger,
        IFileService fileService)
    {
        _validator = validator;
        _volunteersRepository = volunteersRepository;
        _logger = logger;
        _fileService = fileService;
    }

    public async Task<Result<StartMultipartUploadResponse, ErrorList>> HandleAsync(
        StartUploadPhotosToPetCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();

        var volunteerId = VolunteerId.Create(command.VolunteerId);
        var volunteerResult = await _volunteersRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
            return volunteerResult.Error.ToErrorList();

        var petId = PetId.Create(command.PetId);
        var pet = volunteerResult.Value.GetPetById(petId);
        if (pet.IsFailure)
            return pet.Error.ToErrorList();

        // межсерверное взаимодействие, получение ссылок на загрузку в S3 хранилище
        var clientInfos = command.FileInfos
            .Select(f => new MultipartStartClientInfo(f.FileName, f.ContentType))
            .ToList();

        var request = new StartMultipartUploadRequest(clientInfos);

        var result = await _fileService.StartMultipartUpload(request, cancellationToken);
        if (result.IsFailure)
            return Errors.General.Failure(result.Error).ToErrorList();

        _logger.LogInformation(
            "Successfully created data for starting multipart upload of photos for pet with ID = {ID}", petId.Value);

        return result.Value;
    }
}