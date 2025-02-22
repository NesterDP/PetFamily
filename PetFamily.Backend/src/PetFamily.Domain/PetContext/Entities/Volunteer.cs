using CSharpFunctionalExtensions;
using PetFamily.Domain.PetContext.ValueObjects.PetVO;
using PetFamily.Domain.Shared.SharedVO;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;

namespace PetFamily.Domain.PetContext.Entities;

public class Volunteer : Shared.Entity<VolunteerId>
{
    private bool _isDeleted = false;
    public FullName FullName { get; private set; }
    public Email Email { get; private set; }
    public Description Description { get; private set; }
    public Experience Experience { get; private set; }

    public Phone PhoneNumber { get; private set; }
    public SocialNetworkList SocialNetworkList { get; private set; }
    public TransferDetailsList TransferDetailsList { get; private set; }

    private readonly List<Pet> _pets = [];
    public IReadOnlyList<Pet> AllOwnedPets => _pets;

    public int PetsFoundHome() => AllOwnedPets.Count(p => p.HelpStatus.Value == PetStatus.FoundHome);
    public int PetsSeekingHome() => AllOwnedPets.Count(p => p.HelpStatus.Value == PetStatus.SeekingHome);
    public int PetsUnderTreatment() => AllOwnedPets.Count(p => p.HelpStatus.Value == PetStatus.NeedHelp);

    // ef
    public Volunteer(VolunteerId id) : base(id)
    {
    }

    public Volunteer(
        VolunteerId id,
        FullName fullName,
        Email email,
        Description description,
        Experience experience,
        Phone phoneNumber,
        SocialNetworkList socialNetworkList,
        TransferDetailsList transferDetailsList) : base(id)
    {
        FullName = fullName;
        Email = email;
        Description = description;
        Experience = experience;
        PhoneNumber = phoneNumber;
        SocialNetworkList = socialNetworkList;
        TransferDetailsList = transferDetailsList;
    }

    public static Result<Volunteer, Error> Create(
        VolunteerId id,
        FullName fullName,
        Email email,
        Description description,
        Experience experience,
        Phone phoneNumber,
        SocialNetworkList socialNetworkList,
        TransferDetailsList transferDetailsList)
    {
        return new Volunteer(
            id,
            fullName,
            email,
            description,
            experience,
            phoneNumber,
            socialNetworkList,
            transferDetailsList);
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

    public void UpdateSocialNetworks(SocialNetworkList socialNetworkList)
    {
        SocialNetworkList = socialNetworkList;
    }

    public void UpdateTransferDetails(TransferDetailsList transferDetailsList)
    {
        TransferDetailsList = transferDetailsList;
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
        foreach (var pet in _pets)
        {
            if (pet.Id == id)
                return pet;
        }

        return Errors.General.ValueNotFound();
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
}