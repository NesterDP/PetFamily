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
    public TransferDetailList TransferDetailList { get; private set; }

    private readonly List<Pet> _pets = [];
    public IReadOnlyCollection<Pet> AllOwnedPets => _pets;

    public int PetsFoundHome() => AllOwnedPets.Count(p => p.HelpStatus.Value == PetStatus.FoundHome);
    public int PetsSeekingHome() => AllOwnedPets.Count(p => p.HelpStatus.Value == PetStatus.SeekingHome);
    public int PetsUnderTreatment() => AllOwnedPets.Count(p => p.HelpStatus.Value == PetStatus.NeedHelp);

    // ef
    public Volunteer(VolunteerId id) : base(id) { }

    public Volunteer(
        VolunteerId id,
        FullName fullName,
        Email email,
        Description description,
        Experience experience,
        Phone phoneNumber,
        SocialNetworkList socialNetworkList,
        TransferDetailList transferDetailList) : base(id)
    {
        FullName = fullName;
        Email = email;
        Description = description;
        Experience = experience;
        PhoneNumber = phoneNumber;
        SocialNetworkList = socialNetworkList;
        TransferDetailList = transferDetailList;
    }

    public static Result<Volunteer, Error> Create(
        VolunteerId id,
        FullName fullName,
        Email email,
        Description description,
        Experience experience,
        Phone phoneNumber,
        SocialNetworkList socialNetworkList,
        TransferDetailList transferDetailList)
    {
        return new Volunteer(
            id,
            fullName,
            email,
            description,
            experience, phoneNumber,
            socialNetworkList,
            transferDetailList);
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
    
    public void UpdateTransferDetails(TransferDetailList transferDetailList)
    {
        TransferDetailList = transferDetailList;
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
    
}