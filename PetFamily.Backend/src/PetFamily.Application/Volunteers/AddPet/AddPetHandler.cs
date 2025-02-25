using System.Runtime.InteropServices.JavaScript;
using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Database;
using PetFamily.Application.Extensions;
using PetFamily.Application.FilesProvider;
using PetFamily.Application.FilesProvider.FilesData;
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
    private readonly IValidator<AddPetCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly ISpeciesRepository _speciesRepository;
    private readonly ILogger<AddPetHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public AddPetHandler(
        IValidator<AddPetCommand> validator,
        IVolunteersRepository volunteersRepository,
        ISpeciesRepository speciesRepository,
        ILogger<AddPetHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _volunteersRepository = volunteersRepository;
        _speciesRepository = speciesRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        AddPetCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToErrorList();


        var volunteerId = VolunteerId.Create(command.VolunteerId);
        var volunteerResult = await _volunteersRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
            return volunteerResult.Error.ToErrorList();

        var petClassification = await GetPetClassification(
            command.PetClassificationDto.SpeciesName,
            command.PetClassificationDto.BreedName,
            cancellationToken);

        var pet = CreatePet(command, petClassification);

        volunteerResult.Value.AddPet(pet);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Successfully added pet with ID = {ID} to volunteer with ID = {ID}", pet.Id.Value, volunteerId.Value);

        return pet.Id.Value;
    }

    private Pet CreatePet(AddPetCommand command, PetClassification petClassification)
    {
        var petId = PetId.NewPetId();
        
        var name = Name.Create(command.Name).Value;
        
        var description = Description.Create(command.Description).Value;
        
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

        var photosListResult = PhotosList.Create([]).Value;

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

        return pet;
    }

    private async Task<PetClassification> GetPetClassification(
        string clientSpeciesName,
        string clientBreedName,
        CancellationToken cancellationToken)
    {
        // приводим полученные строки к единому в БД виду
        clientSpeciesName = clientSpeciesName.ToLower().Trim();
        clientBreedName = clientBreedName.ToLower().Trim();
        
        // ищем species по поле "имя" в БД
        var speciesResult = await _speciesRepository.GetByNameAsync(clientSpeciesName, cancellationToken);

        // не нашли такой species
        if (speciesResult.IsFailure)
        {
            // создаем species и breed
            var speciesName = Name.Create(clientSpeciesName).Value;
            var species = new Species(SpeciesId.NewSpeciesId(), speciesName, new List<Breed>());

            var breedName = Name.Create(clientBreedName).Value;
            var breed = new Breed(BreedId.NewBreedId(), breedName);
            
            // добавляем breed к species через метод species
            species.AddBreed(breed);
            await _speciesRepository.AddAsync(species, cancellationToken);

            return PetClassification.Create(species.Id.Value, breed.Id.Value).Value;
        }
        else
        {
            // ищем у найденого species breed с таким же именем, как передал клиент
            var breedResult = speciesResult.Value.GetBreedByName(clientBreedName);
            
            // если нашли - не трогаем БД, просто возвращаем Value object для Pet
            if (breedResult.IsSuccess)
                return PetClassification.Create(speciesResult.Value.Id.Value, breedResult.Value.Id.Value).Value;
            
            // если не нашли - создаем breed 
            var breedName = Name.Create(clientBreedName).Value;
            var breed = new Breed(BreedId.NewBreedId(), breedName);
            
            // добавляем его в существующему species через метод species
            speciesResult.Value.AddBreed(breed);
            _speciesRepository.Save(speciesResult.Value, cancellationToken);

            return PetClassification.Create(speciesResult.Value.Id.Value, breed.Id.Value).Value;
        }
    }
}