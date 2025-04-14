using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts.Requests;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.Structs;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Volunteers.Application.Commands.DeletePetPhotos;

public class DeletePetPhotosHandler : ICommandHandler<Guid, DeletePetPhotosCommand>
{
    private readonly IValidator<DeletePetPhotosCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly IFileService _fileService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeletePetPhotosHandler> _logger;

    public DeletePetPhotosHandler(
        IValidator<DeletePetPhotosCommand> validator,
        IVolunteersRepository volunteersRepository,
        IFileService fileService,
        [FromKeyedServices(UnitOfWorkSelector.Volunteers)]
        IUnitOfWork unitOfWork,
        ILogger<DeletePetPhotosHandler> logger)
    {
        _validator = validator;
        _volunteersRepository = volunteersRepository;
        _fileService = fileService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        DeletePetPhotosCommand command,
        CancellationToken cancellationToken)
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

        // новый список фото питомцев = разность между текущим списком фото питомца и списком удаляемых фото
        var updateList = pet.Value.PhotosList.Select(p => p.CreateCopy()).ToList();
        updateList.RemoveAll(photo => command.PhotosIds.Contains(photo.Id.Value));

        // обновили фото питомца
        volunteerResult.Value.UpdatePetPhotos(pet.Value.Id, updateList);

        // межпроцессное взаимодействие, удаление из данных из Mongo и S3 хранилища
        var result = await _fileService.DeleteFilesByIds(new DeleteFilesByIdsRequest(command.PhotosIds), cancellationToken);
        if (result.IsFailure)
            return Errors.General.Failure(result.Error).ToErrorList();
        
        // сохранение изменений в БД модуля
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully deleted photos for pet with ID = {ID}", pet.Value.Id.Value);

        return pet.Value.Id.Value;
    }
}