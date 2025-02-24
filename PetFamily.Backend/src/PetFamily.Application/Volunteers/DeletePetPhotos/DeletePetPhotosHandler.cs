using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Database;
using PetFamily.Application.Extensions;
using PetFamily.Application.FilesProvider.FilesData;
using PetFamily.Application.Providers;
using PetFamily.Domain.PetContext.Entities;
using PetFamily.Domain.PetContext.ValueObjects.PetVO;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Volunteers.DeletePetPhotos;

public class DeletePetPhotosHandler
{
    private readonly IValidator<DeletePetPhotosCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeletePetPhotosHandler> _logger;
    private readonly IFilesProvider _filesProvider;
    private const string BUCKET_NAME = "files";

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

        var transaction = await _unitOfWork.BeginTransaction(cancellationToken);
        try
        {
            var volunteerId = VolunteerId.Create(command.VolunteerId);
            var volunteerResult = await _volunteersRepository.GetByIdAsync(volunteerId, cancellationToken);
            if (volunteerResult.IsFailure)
                return volunteerResult.Error.ToErrorList();

            var pet = volunteerResult.Value.AllOwnedPets.FirstOrDefault(p => p.Id.Value == command.PetId);
            if (pet == null)
                return Errors.General.ValueNotFound("pet").ToErrorList();
            
            
            var paths = pet.PhotosList.Photos.Select(photo => photo.PathToStorage.Path).ToList();
            var intermediateCollection = paths.Except(command.PhotosNames);
            var updateList = intermediateCollection.Select(s => new Photo(FilePath.Create(s).Value)).ToList();
            volunteerResult.Value.UpdatePetPhotos(pet.Id, PhotosList.Create(updateList).Value);
            
            
            List<DeleteData> deleteData = new List<DeleteData>();
            foreach (var path in command.PhotosNames)
            {
                deleteData.Add(new DeleteData(path, BUCKET_NAME));
            }
            
            await _unitOfWork.SaveChanges(cancellationToken);
            
            var deleteResult = await _filesProvider.DeleteFiles(deleteData, cancellationToken);
            
            if (deleteResult.IsFailure)
                return deleteResult.Error.ToErrorList();

            transaction.Commit();
            
            _logger.LogInformation("Successfully deleted some photos for pet with ID = {ID}", pet.Id.Value);

            return pet.Id.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Can not delete pet photos - {id} in transaction", command.PetId);

            transaction.Rollback();

            return Error.Failure(
                "volunteer.pet.failure", "could not delete pet photos)").ToErrorList();
        }
    }
}