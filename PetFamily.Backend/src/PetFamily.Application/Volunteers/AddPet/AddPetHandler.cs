using System.Runtime.InteropServices.JavaScript;
using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Database;
using PetFamily.Application.Extensions;
using PetFamily.Application.FilesProvider.FilesData;
using PetFamily.Application.Providers;
using PetFamily.Application.SpeciesRepositoryInterface;
using PetFamily.Domain.PetContext.Entities;
using PetFamily.Domain.PetContext.ValueObjects.PetVO;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;
using PetFamily.Domain.SpeciesContext.Entities;
using PetFamily.Domain.SpeciesContext.ValueObjects.BreedVO;
using PetFamily.Domain.SpeciesContext.ValueObjects.SpeciesVO;

namespace PetFamily.Application.Volunteers.AddPet;

public class AddPetHandler
{
    private const string BUCKET_NAME = "files";
    private readonly IValidator<AddPetCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ISpeciesRepository _speciesRepository;
    private readonly ILogger<AddPetHandler> _logger;
    private readonly IFilesProvider _filesProvider;
    private readonly IUnitOfWork _unitOfWork;

    public AddPetHandler(
        IValidator<AddPetCommand> validator,
        IVolunteersRepository volunteersRepository,
        ISpeciesRepository speciesRepository,
        ILogger<AddPetHandler> logger,
        IFilesProvider filesProvider,
        IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _volunteersRepository = volunteersRepository;
        _speciesRepository = speciesRepository;
        _logger = logger;
        _filesProvider = filesProvider;
        _unitOfWork = unitOfWork;
    }

    private async Task<PetClassification> GetPetClassification(
        string clientSpeciesName,
        string clientBreedName,
        CancellationToken cancellationToken)
    {
        clientSpeciesName = clientSpeciesName.ToLower().Trim();
        clientBreedName = clientBreedName.ToLower().Trim();
        var speciesResult = await _speciesRepository.GetByNameAsync(clientSpeciesName, cancellationToken);
        if (speciesResult.IsFailure)
        {
            var speciesName = Name.Create(clientSpeciesName).Value;
            var species = new Species(SpeciesId.NewSpeciesId(), speciesName, new List<Breed>());

            var breedName = Name.Create(clientBreedName).Value;
            var breed = new Breed(BreedId.NewBreedId(), breedName);

            species.AddPet(breed);
            await _speciesRepository.AddAsync(species, cancellationToken);

            return PetClassification.Create(species.Id.Value, breed.Id.Value).Value;
        }
        else
        {
            var existedBreed = speciesResult.Value.Breeds.FirstOrDefault(b => b.Name.Value == clientBreedName);

            if (existedBreed != null)
                return PetClassification.Create(speciesResult.Value.Id.Value, existedBreed.Id.Value).Value;
            
            var breedName = Name.Create(clientBreedName).Value;
            var breed = new Breed(BreedId.NewBreedId(), breedName);

            speciesResult.Value.AddPet(breed);
            await _speciesRepository.SaveAsync(speciesResult.Value, cancellationToken);

            return PetClassification.Create(speciesResult.Value.Id.Value, breed.Id.Value).Value;

        }
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        AddPetCommand command,
        CancellationToken cancellationToken = default)
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

            var petId = PetId.NewPetId();
            var name = Name.Create(command.Name).Value;
            var description = Description.Create(command.Description).Value;

            var petClassification = await GetPetClassification(
                command.PetClassificationDto.SpeciesName,
                command.PetClassificationDto.BreedName,
                cancellationToken);

            var color = Color.Create(command.Color).Value;
            var healthInfo = HealthInfo.Create(command.HealthInfo).Value;
            var address = Address.Create(
                command.AddressDto.City,
                command.AddressDto.House,
                command.AddressDto.Apartment).Value;
            var weight = Weight.Create(command.Weight).Value;
            var height = Height.Create(command.Height).Value;
            var ownerPhoneNUmber = Phone.Create(command.OwnerPhoneNumber).Value;
            var isCastrated = IsCastrated.Create(command.IsCastrated).Value;
            var dateOfBirth = DateOfBirth.Create(command.DateOfBirth.ToUniversalTime()).Value;
            var isVaccinated = IsVaccinated.Create(command.IsVaccinated).Value;

            Enum.TryParse(command.HelpStatus, out PetStatus status);
            var helpStatus = HelpStatus.Create(status).Value;

            List<TransferDetail> transferDetailsList = [];
            foreach (var transferDetail in command.TransferDetailsDto)
            {
                var result = TransferDetail.Create(transferDetail.Name, transferDetail.Description);
                transferDetailsList.Add(result.Value);
            }

            var transferDetailsListResult = TransferDetailsList.Create(transferDetailsList).Value;

            
            // работа с файлами

            List<FileData> filesData = [];
            foreach (var file in command.Files)
            {
                var extension = Path.GetExtension(file.FileName);

                var filePath = FilePath.Create(Guid.NewGuid(), extension);
                if (filePath.IsFailure)
                    return filePath.Error.ToErrorList();

                var fileContent = new FileData(file.Content, filePath.Value, BUCKET_NAME);

                filesData.Add(fileContent);
            }

            var issueFiles = filesData
                .Select(f => f.FilePath)
                .Select(f => new Photo(f))
                .ToList();

            var photosListResult = PhotosList.Create(issueFiles).Value;

            var pet = new Pet(
                petId,
                name,
                description,
                petClassification,
                color,
                healthInfo,
                address,
                weight,
                height,
                ownerPhoneNUmber,
                isCastrated,
                dateOfBirth,
                isVaccinated,
                helpStatus,
                transferDetailsListResult,
                photosListResult);

            volunteerResult.Value.AddPet(pet);

            await _unitOfWork.SaveChanges(cancellationToken);

            var uploadResult = await _filesProvider.UploadFiles(filesData, cancellationToken);

            if (uploadResult.IsFailure)
                return uploadResult.Error.ToErrorList();

            transaction.Commit();
            
            _logger.LogInformation(
                "Successfully added pet with ID = {ID} to volunteer with ID = {ID}", pet.Id.Value, volunteerId.Value);

            return pet.Id.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "failed to add pet to volunteer with Id = {id} in transaction", command.VolunteerId);

            transaction.Rollback();

            return Error.Failure(
                "volunteer.pet.failure", "failed to add pet to volunteer").ToErrorList();
        }
    }
}