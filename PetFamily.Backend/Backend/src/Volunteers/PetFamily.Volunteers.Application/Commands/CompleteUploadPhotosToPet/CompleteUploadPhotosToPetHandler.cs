using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
using FileService.Contracts.SubModels;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.Structs;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Volunteers.Application.Commands.CompleteUploadPhotosToPet;

public class CompleteUploadPhotosToPetHandler 
    : ICommandHandler<CompleteMultipartUploadResponse, CompleteUploadPhotosToPetCommand>
{
    private readonly IValidator<CompleteUploadPhotosToPetCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ILogger<CompleteUploadPhotosToPetHandler> _logger;
    private readonly FileHttpClient _httpClient;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteUploadPhotosToPetHandler(
        IValidator<CompleteUploadPhotosToPetCommand> validator,
        IVolunteersRepository volunteersRepository,
        ILogger<CompleteUploadPhotosToPetHandler> logger,
        FileHttpClient httpClient,
        [FromKeyedServices(UnitOfWorkSelector.Volunteers)]
        IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _volunteersRepository = volunteersRepository;
        _logger = logger;
        _httpClient = httpClient;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CompleteMultipartUploadResponse, ErrorList>> HandleAsync(
        CompleteUploadPhotosToPetCommand command,
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

        // создание данных для межпроцессного взаимодействия
        var clientInfos = command.FileInfos
            .Select(f => new MultipartCompleteClientInfo(
                f.Key,
                f.UploadId,
                f.Parts.Select(p => new PartETagInfo(p.PartNumber, p.ETag)).ToList()))
            .ToList();

        var request = new CompleteMultipartUploadRequest(clientInfos);

        // межсерверное взаимодействие, подтверждение загрузки в S3 и создание записи о файлах в Mongo
        var result = await _httpClient.CompleteMultipartUpload(request, cancellationToken);
        if (result.IsFailure)
            return Errors.General.Failure(result.Error).ToErrorList();
        
        // из полученной коллекции создаем коллекцию value objects (фото)
        var photoFiles = result.Value.MultipartCompleteInfos
            .Select(fileInfo => Photo.Create(FileId.Create(fileInfo.FileId).Value, fileInfo.ContentType).Value);

        // новый список фото должен содержать как добавленные, так и старые фото
        var unionOfPhotos = pet.Value.PhotosList.Union(photoFiles);
        volunteerResult.Value.UpdatePetPhotos(pet.Value.Id, unionOfPhotos);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Successfully completed multipart upload and added photos to pet with ID = {ID}", petId.Value);

        return result.Value;
    }
}