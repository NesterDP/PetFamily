using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Core;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.Structs;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.Volunteers.Domain.ValueObjects.PetVO;
using PetFamily.Volunteers.Domain.ValueObjects.VolunteerVO;
using FileInfo = PetFamily.Core.Files.FilesData.FileInfo;

namespace PetFamily.Volunteers.Application.Commands.DeletePetPhotos;

public class DeletePetPhotosHandler : ICommandHandler<Guid, DeletePetPhotosCommand>
{
    private readonly IValidator<DeletePetPhotosCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeletePetPhotosHandler> _logger;
    private readonly IFilesProvider _filesProvider;
    private const string BUCKET_NAME = "photos";

    public DeletePetPhotosHandler(
        IValidator<DeletePetPhotosCommand> validator,
        IVolunteersRepository volunteersRepository,
        [FromKeyedServices(UnitOfWorkSelector.Volunteers)] IUnitOfWork unitOfWork,
        ILogger<DeletePetPhotosHandler> logger,
        IFilesProvider filesProvider)
    {
        _validator = validator;
        _volunteersRepository = volunteersRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _filesProvider = filesProvider;
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
        
        // новый список фото питомцев = разность между текущим списком фото питомца и списком удаленных фото
        var updateList = pet.Value.PhotosList.Select(p => p.CreateCopy()).ToList();
        updateList.RemoveAll(photo => command.PhotosNames.Contains(photo.PathToStorage.Path));
        
        // обновили фото питомца
        volunteerResult.Value.UpdatePetPhotos(pet.Value.Id, updateList);
        
        // обновили соответсвующую запись в БД независимо от того, удалятся ли данные в minio
        await _unitOfWork.SaveChangesAsync(cancellationToken); 
        
        // формируем данные для удаления
        var deleteData = new List<FileInfo>(); 
        foreach (var path in command.PhotosNames)
        {
            deleteData.Add(new FileInfo(FilePath.Create(path).Value, BUCKET_NAME));
        }
        
        // удаляем из minio
        var deleteResult = await _filesProvider.DeleteFiles(deleteData, cancellationToken); 
        if (deleteResult.IsFailure)
            return deleteResult.Error.ToErrorList();
        
        
        _logger.LogInformation("Successfully deleted photos for pet with ID = {ID}", pet.Value.Id.Value);

        return pet.Value.Id.Value;
    }
}