using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Core;
using PetFamily.Core.Abstractions;
using PetFamily.Core.CustomErrors;
using PetFamily.Core.Extensions;
using PetFamily.Volunteers.Domain.Entities;
using PetFamily.Volunteers.Domain.ValueObjects.VolunteerVO;
using FileInfo = PetFamily.Core.Files.FilesData.FileInfo;

namespace PetFamily.Volunteers.Application.VolunteersManagement.Commands.DeletePet;

public class HardDeletePetHandler : ICommandHandler<Guid, DeletePetCommand>
{
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ILogger<HardDeletePetHandler> _logger;
    private readonly IValidator<DeletePetCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFilesProvider _filesProvider;
    private readonly string BUCKET_NAME = "photos";

    public HardDeletePetHandler(
        IVolunteersRepository volunteersRepository,
        ILogger<HardDeletePetHandler> logger,
        IValidator<DeletePetCommand> validator,
        IUnitOfWork unitOfWork, IFilesProvider filesProvider)
    {
        _volunteersRepository = volunteersRepository;
        _logger = logger;
        _validator = validator;
        _unitOfWork = unitOfWork;
        _filesProvider = filesProvider;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        DeletePetCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();
        
        var volunteerId = VolunteerId.Create(command.VolunteerId);

        var volunteerResult = await _volunteersRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
            return volunteerResult.Error.ToErrorList();
        
        var pet = volunteerResult.Value.AllOwnedPets.FirstOrDefault(p => p.Id == command.PetId);
        if (pet == null)
            return Errors.General.ValueNotFound(command.PetId).ToErrorList();

        volunteerResult.Value.HardDeletePet(pet);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await DeletePhotosFromFileProvider(pet, cancellationToken);
        
        _logger.LogInformation("Pet was hard deleted, his ID = {ID}", pet.Id);

        return pet.Id.Value;
    }

    private async Task DeletePhotosFromFileProvider(Pet pet, CancellationToken cancellationToken)
    {
        var filesPaths = pet.PhotosList.Select(p => p.PathToStorage); 
        var deleteData = new List<FileInfo>(); 
        foreach (var path in filesPaths)
        {
            deleteData.Add(new FileInfo(path, BUCKET_NAME));
        }
        
        // удаляем из minio
        await _filesProvider.DeleteFiles(deleteData, cancellationToken); 
    }
}