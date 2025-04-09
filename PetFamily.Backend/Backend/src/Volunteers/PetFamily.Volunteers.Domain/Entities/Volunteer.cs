using CSharpFunctionalExtensions;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.Volunteers.Domain.ValueObjects.PetVO;
using PetFamily.Volunteers.Domain.ValueObjects.VolunteerVO;

namespace PetFamily.Volunteers.Domain.Entities;

public class Volunteer : SoftDeletableEntity<VolunteerId>
{
    public FullName FullName { get; private set; }
    public Email Email { get; private set; }
    public Description Description { get; private set; }
    public Experience Experience { get; private set; }

    public Phone PhoneNumber { get; private set; }
    
    private readonly List<Pet> _pets = [];
    public IReadOnlyList<Pet> AllOwnedPets => _pets;

    public int PetsFoundHome() => AllOwnedPets
        .Count(p => p.HelpStatus.Value == PetStatus.FoundHome);

    public int PetsInSearchOfHome() => AllOwnedPets
        .Count(p => p.HelpStatus.Value == PetStatus.InSearchOfHome);

    public int PetsUnderMedicalTreatment() => AllOwnedPets
        .Count(p => p.HelpStatus.Value == PetStatus.UnderMedicalTreatment);

    // ef core
    private Volunteer(VolunteerId id) : base(id)
    {
    }

    public Volunteer(
        VolunteerId id,
        FullName fullName,
        Email email,
        Description description,
        Experience experience,
        Phone phoneNumber) : base(id)
    {
        FullName = fullName;
        Email = email;
        Description = description;
        Experience = experience;
        PhoneNumber = phoneNumber;
    }

    public static Result<Volunteer, Error> Create(
        VolunteerId id,
        FullName fullName,
        Email email,
        Description description,
        Experience experience,
        Phone phoneNumber)
    {
        return new Volunteer(
            id,
            fullName,
            email,
            description,
            experience,
            phoneNumber);
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
    
    public void UpdatePetPhotos(PetId petId, IEnumerable<Photo> photos)
    {
        var chosenPet = _pets.FirstOrDefault(p => p.Id.Value == petId.Value);
        if (chosenPet != null)
            chosenPet.UpdatePhotos(photos);
    }

    public UnitResult<Error> UpdatePetMainPhoto(PetId petId, string path)
    {
        var chosenPet = _pets.FirstOrDefault(p => p.Id.Value == petId.Value);
        if (chosenPet == null)
            return Errors.General.ValueNotFound(petId);

        var photo = chosenPet.PhotosList.FirstOrDefault(p => p.PathToStorage.Path == path);
        if (photo == null)
            return Errors.General.ValueNotFound();
        
        chosenPet.UpdateMainPhoto(photo);
        return UnitResult.Success<Error>();
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

    public UnitResult<Error> UpdatePetInfo(
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
    
    public override void Delete()
    {
        base.Delete();

        foreach (var pet in _pets)
        {
            pet.Delete();
        }
    }

    public override void Restore()
    {
        base.Restore();

        foreach (var pet in _pets)
        {
            pet.Restore();
        }
    }

    public void HardDeletePet(Pet pet)
    {
        _pets.Remove(pet);
        foreach (var p in _pets.Where(p => p.Position.Value > pet.Position.Value))
        {
            var newPosition = (Position.Create(p.Position.Value - 1).Value);
            p.SetPosition(newPosition);
        }
    }

    public void SoftDeletePet(Pet pet)
    {
        pet.Delete();
    }

    public void DeleteExpiredPets(int lifetimeAfterDeletion)
    {
        var petsToDelete = _pets.Where(pet => 
                pet.DeletionDate != null && 
                DateTime.UtcNow >= pet.DeletionDate.Value.AddDays(lifetimeAfterDeletion))
                //DateTime.UtcNow >= pet.DeletionDate.Value.AddMinutes(lifetimeAfterDeletion))
            .ToList();

        foreach (var pet in petsToDelete)
        {
            HardDeletePet(pet);
        }
    }
}