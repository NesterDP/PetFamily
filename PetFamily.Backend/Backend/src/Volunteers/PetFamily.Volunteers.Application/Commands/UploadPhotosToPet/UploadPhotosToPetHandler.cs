using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.Structs;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Volunteers.Application.Commands.UploadPhotosToPet;

public class UploadPhotosToPetHandler : ICommandHandler<Guid, UploadPhotosToPetCommand>
{
    private readonly IValidator<UploadPhotosToPetCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ILogger<UploadPhotosToPetHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public UploadPhotosToPetHandler(
        IValidator<UploadPhotosToPetCommand> validator,
        IVolunteersRepository volunteersRepository,
        ILogger<UploadPhotosToPetHandler> logger,
        [FromKeyedServices(UnitOfWorkSelector.Volunteers)]
        IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _volunteersRepository = volunteersRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        UploadPhotosToPetCommand command,
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
        
        // TODO: отправить запрос к FileService на загрузку фото, скорректировать валидатор

        var responseGuids = new List<Guid>(); // имитация ответа от сервиса файлов - список ID загруженных файлов
        
        // из полученной коллекции guid создаем коллекцию value objects (фото)
        var photoFiles = responseGuids
            .Select(id => Photo.Create(FileId.Create(id).Value,Photo.AllowedTypes.First()).Value); 

        // новый список фото должен содержать как добавленные, так и старые фото
        var unionOfPhotos = pet.Value.PhotosList.Union(photoFiles);
        volunteerResult.Value.UpdatePetPhotos(pet.Value.Id, unionOfPhotos);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Successfully uploaded photos to pet with ID = {ID}", petId.Value);

        return petId.Value;
    }
}