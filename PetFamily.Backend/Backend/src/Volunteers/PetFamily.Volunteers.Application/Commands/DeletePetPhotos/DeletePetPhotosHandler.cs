using CSharpFunctionalExtensions;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeletePetPhotosHandler> _logger;

    public DeletePetPhotosHandler(
        IValidator<DeletePetPhotosCommand> validator,
        IVolunteersRepository volunteersRepository,
        [FromKeyedServices(UnitOfWorkSelector.Volunteers)]
        IUnitOfWork unitOfWork,
        ILogger<DeletePetPhotosHandler> logger)
    {
        _validator = validator;
        _volunteersRepository = volunteersRepository;
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

        // обновили соответсвующую запись в БД независимо от того, удалятся ли данные в minio
        await _unitOfWork.SaveChangesAsync(cancellationToken);


        // TODO: сделать запрос к FileService на удаление фото, пришедших в команде (из mongoDB и S3 хранилища)

        _logger.LogInformation("Successfully deleted photos for pet with ID = {ID}", pet.Value.Id.Value);

        return pet.Value.Id.Value;
    }
}