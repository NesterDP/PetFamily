using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Database;
using PetFamily.Application.Extensions;
using PetFamily.Application.Files;
using PetFamily.Domain.PetContext.ValueObjects.PetVO;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;
using FileInfo = PetFamily.Application.Files.FilesData.FileInfo;

namespace PetFamily.Application.Volunteers.Commands.DeletePetPhotos;

public class DeletePetPhotosHandler
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
        IUnitOfWork unitOfWork,
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
        var paths = pet.Value.PhotosList.Select(photo => photo.PathToStorage.Path).ToList();
        var intermediateCollection = paths.Except(command.PhotosNames);
        var updateList = intermediateCollection.Select(s => new Photo(FilePath.Create(s).Value)).ToList();
        
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