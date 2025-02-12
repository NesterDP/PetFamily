using CSharpFunctionalExtensions;
using PetFamily.Domain.PetContext.ValueObjects.PetVO;
using PetFamily.Domain.Shared.SharedVO;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;

namespace PetFamily.Domain.PetContext.Entities;

public class Volunteer : Entity
{
    public VolunteerId Id { get; private set; }
    public FullName FullName { get; private set; }
    public Email Email { get; private set; }
    public Description Description { get; private set; }
    public Experience Experience { get; private set; }

    public Phone PhoneNumber { get; private set; }
    public SocialNetworksList SocialNetworkList { get; private set; }
    public TransferDetailsList TransferDetailsList { get; private set; }

    private readonly List<Pet> _pets = [];
    public IReadOnlyCollection<Pet> GetAllOwnedPets => _pets;

    public int PetsFoundHome() => GetAllOwnedPets.Count(p => p.HelpStatus.Value == PetStatus.FoundHome);
    public int PetsSeekingHome() => GetAllOwnedPets.Count(p => p.HelpStatus.Value == PetStatus.SeekingHome);
    public int PetsUnderTreatment() => GetAllOwnedPets.Count(p => p.HelpStatus.Value == PetStatus.NeedHelp);

    public Volunteer(
        VolunteerId id,
        FullName fullName,
        Email email,
        Description description,
        Experience experience,
        Phone phoneNumber,
        SocialNetworksList socialNetworkList,
        TransferDetailsList transferDetailsList)
    {
        Id = id;
        FullName = fullName;
        Email = email;
        Description = description;
        Experience = experience;
        PhoneNumber = phoneNumber;
        SocialNetworkList = socialNetworkList;
        TransferDetailsList = transferDetailsList;
    }
    
}