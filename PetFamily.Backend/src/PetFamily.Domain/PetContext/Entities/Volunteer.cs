using CSharpFunctionalExtensions;
using PetFamily.Domain.PetContext.ValueObjects.PetVO;
using PetFamily.Domain.Shared.SharedVO;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;

namespace PetFamily.Domain.PetContext.Entities;

public class Volunteer : Entity<VolunteerId>
{
    private bool _isDeleted = false;
    public FullName FullName { get; private set; }
    public Email Email { get; private set; }
    public Description Description { get; private set; }
    public Experience Experience { get; private set; }

    public Phone PhoneNumber { get; private set; }
    
    private List<SocialNetwork> _socialNetworks = [];
    public IReadOnlyList<SocialNetwork> SocialNetworksList
    {
        get => _socialNetworks;
        private set => _socialNetworks = value.ToList();
    }
    
    private List<TransferDetail> _transferDetails = [];
    public IReadOnlyList<TransferDetail> TransferDetailsList
    {
        get => _transferDetails;
        private set => _transferDetails = value.ToList();
    }

    private readonly List<Pet> _pets = [];
    public IReadOnlyList<Pet> AllOwnedPets => _pets;

    public int PetsFoundHome() => AllOwnedPets
        .Count(p => p.HelpStatus.Value == PetStatus.FoundHome);
    public int PetsInSearchOfHome() => AllOwnedPets
        .Count(p => p.HelpStatus.Value == PetStatus.InSearchOfHome);
    public int PetsUnderMedicalTreatment() => AllOwnedPets
        .Count(p => p.HelpStatus.Value == PetStatus.UnderMedicalTreatment);

    // ef
    public Volunteer() { }

    public Volunteer(
        VolunteerId id,
        FullName fullName,
        Email email,
        Description description,
        Experience experience,
        Phone phoneNumber,
        IEnumerable<SocialNetwork> socialNetworks,
        IEnumerable<TransferDetail> transferDetails) : base(id)
    {
        FullName = fullName;
        Email = email;
        Description = description;
        Experience = experience;
        PhoneNumber = phoneNumber;
        SocialNetworksList = socialNetworks.ToList();
        TransferDetailsList = transferDetails.ToList();
    }

    public static Result<Volunteer, Error> Create(
        VolunteerId id,
        FullName fullName,
        Email email,
        Description description,
        Experience experience,
        Phone phoneNumber,
        IEnumerable<SocialNetwork> socialNetworks,
        IEnumerable<TransferDetail> transferDetails)
    {
        return new Volunteer(
            id,
            fullName,
            email,
            description,
            experience,
            phoneNumber,
            socialNetworks.ToList(),
            transferDetails.ToList());
    }

    public void UpdateMainInfo(
        FullName fullName,
        Email email,
        Description description,
        Experience experience,
        Phone phoneNumber)
    {
        FullName = fullName;
        Email = email;
        Description = description;
        Experience = experience;
        PhoneNumber = phoneNumber;
    }

    public void UpdateSocialNetworks(IEnumerable<SocialNetwork> socialNetworks)
    {
        SocialNetworksList = socialNetworks.ToList();
    }

    public void UpdateTransferDetails(IEnumerable<TransferDetail> transferDetails)
    {
        TransferDetailsList = transferDetails.ToList();
    }

    public void Delete()
    {
        if (_isDeleted == false)
            _isDeleted = true;

        foreach (var pet in _pets)
        {
            pet.Delete();
        }
    }

    public void UpdatePetPhotos(PetId petId, IEnumerable<Photo> photos)
    {
        var chosenPet = _pets.FirstOrDefault(p => p.Id.Value == petId.Value);
        if (chosenPet != null)
            chosenPet.UpdatePhotos(photos);
    }

    public UnitResult<Error> UpdatePetHelpStatus(
        PetId petId,
        HelpStatus helpStatus)
    {
        var chosenPet = _pets.FirstOrDefault(p => p.Id.Value == petId.Value);
        if (chosenPet == null)
            return Errors.General.ValueNotFound(petId.Value);
        
        chosenPet.UpdateHelpStatus(helpStatus);

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error>  UpdatePetInfo(
        PetId petId,
        Name name,
        Description description,
        PetClassification petClassification,
        Color color,
        HealthInfo healthInfo,
        Address address,
        Weight weight,
        Height height,
        Phone ownerPhoneNumber,
        IsCastrated isCastrated,
        DateOfBirth dateOfBirth,
        IsVaccinated isVaccinated,
        List<TransferDetail> transferDetails)
    {
        var chosenPet = _pets.FirstOrDefault(p => p.Id.Value == petId.Value);
        if (chosenPet == null)
            return Errors.General.ValueNotFound(petId.Value);
        
        chosenPet.UpdateName(name);
        chosenPet.UpdateDescription(description);
        chosenPet.UpdatePetClassification(petClassification);
        chosenPet.UpdateColor(color);
        chosenPet.UpdateHealthInfo(healthInfo);
        chosenPet.UpdateAddress(address);
        chosenPet.UpdateWeight(weight);
        chosenPet.UpdateHeight(height);
        chosenPet.UpdateOwnerPhoneNumber(ownerPhoneNumber);
        chosenPet.UpdateIsCastrated(isCastrated);
        chosenPet.UpdateDateOfBirth(dateOfBirth);
        chosenPet.UpdateIsVaccinated(isVaccinated);
        chosenPet.UpdateTransferDetails(transferDetails);

        return UnitResult.Success<Error>();
    }
    

    public void Restore()
    {
        if (_isDeleted)
            _isDeleted = false;

        foreach (var pet in _pets)
        {
            pet.Restore();
        }
    }

    public Result<Pet, Error> GetPetById(PetId id)
    {
        var pet = _pets.FirstOrDefault(p => p.Id.Value == id.Value);
        if (pet == null)
            return Errors.General.ValueNotFound(id.Value);
        return pet;
    }

    public UnitResult<Error> AddPet(Pet pet)
    {
        var positionResult = Position.Create(_pets.Count + 1);
        if (positionResult.IsFailure)
            return positionResult.Error;

        pet.SetPosition(positionResult.Value);

        _pets.Add(pet);
        return Result.Success<Error>();
    }
    
    public UnitResult<Error> HardDeletePet(Pet pet)
    {
        _pets.Remove(pet);
        foreach (var p in _pets.Where(p => p.Position.Value > pet.Position.Value))
        {
            var newPosition = (Position.Create(p.Position.Value - 1).Value);
            p.SetPosition(newPosition);
        }
        return Result.Success<Error>();
    }
    
    public UnitResult<Error> SoftDeletePet(Pet pet)
    {
        pet.Delete();
        return Result.Success<Error>();
    }

    public UnitResult<Error> MovePet(Pet pet, Position newPosition)
    {
        var currentPosition = pet.Position;

        if (currentPosition == newPosition || _pets.Count == 1)
            return Result.Success<Error>();

        var adjustedPosition = AdjustNewPositionIfOutOfRange(newPosition);
        if (adjustedPosition.IsFailure)
            return adjustedPosition.Error;

        newPosition = adjustedPosition.Value;
        
        var result = AdjustPositionWithinBorders(newPosition, currentPosition);
        if (result.IsFailure)
            return result.Error;
        
        pet.Move(newPosition);
        
        return Result.Success<Error>();
    }

    private UnitResult<Error> AdjustPositionWithinBorders(Position newPosition, Position currentPosition)
    {
        if (newPosition.Value < currentPosition.Value)
        {
            var petsToMove = _pets.Where(p => p.Position.Value >= newPosition.Value
                                              && p.Position.Value < currentPosition.Value);

            foreach (var petToMove in petsToMove)
            {
                var result = petToMove.MoveForward();
                if (result.IsFailure)
                    return result.Error;
            }
        }

        else if (newPosition.Value > currentPosition.Value)
        {
            var petsToMove = _pets.Where(p => p.Position.Value > currentPosition.Value
                                              && p.Position.Value <= newPosition.Value);

            foreach (var petToMove in petsToMove)
            {
                var result = petToMove.MoveBackward();
                if (result.IsFailure)
                    return result.Error;
            }
        }
        
        return Result.Success<Error>();
    }

    private Result<Position, Error> AdjustNewPositionIfOutOfRange(Position newPosition)
    {
        if (newPosition.Value <= _pets.Count)
            return newPosition;


        var lastPosition = Position.Create(_pets.Count);
        if (lastPosition.IsFailure)
            return lastPosition.Error;

        return lastPosition.Value;
    }
}