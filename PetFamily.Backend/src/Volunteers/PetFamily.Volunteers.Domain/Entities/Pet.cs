using CSharpFunctionalExtensions;
using PetFamily.Core.CustomErrors;
using PetFamily.Core.SharedVO;
using PetFamily.Volunteers.Domain.ValueObjects.PetVO;

namespace PetFamily.Volunteers.Domain.Entities;

public sealed class Pet : Entity<PetId>
{
    public bool _isDeleted { get; private set; }
    public Name Name { get; private set; }
    public Description Description { get; private set; }
    public PetClassification PetClassification { get; private set; }
    public Color Color { get; private set; }
    public HealthInfo HealthInfo { get; private set; }
    public Address Address { get; private set; }
    public Weight Weight { get; private set; }
    public Height Height { get; private set; }
    public Phone OwnerPhoneNumber { get; private set; }
    public IsCastrated IsCastrated { get; private set; }
    public DateOfBirth DateOfBirth { get; private set; }
    public IsVaccinated IsVaccinated { get; private set; }
    public HelpStatus HelpStatus { get; private set; }
    
    private List<TransferDetail> _transferDetails = [];
    
    public IReadOnlyList<TransferDetail> TransferDetailsList
    {
        get => _transferDetails;
        private set => _transferDetails = value.ToList();
    }
    
    private List<Photo> _photos = [];

    public IReadOnlyList<Photo> PhotosList
    {
        get => _photos;
        private set => _photos = value.ToList();
    }
    
    public DateTime CreationDate { get; private set; } = DateTime.UtcNow;
    public Position Position { get; private set; }
    

    //ef
    private Pet(PetId id) : base(id) { }

    public Pet(
        PetId id,
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
        HelpStatus helpStatus,
        IEnumerable<TransferDetail> transferDetails,
        IEnumerable<Photo> photos) : base(id)
    {
        Name = name;
        PetClassification = petClassification;
        Description = description;
        Color = color;
        HealthInfo = healthInfo;
        Address = address;
        Height = height;
        Weight = weight;
        OwnerPhoneNumber = ownerPhoneNumber;
        IsCastrated = isCastrated;
        DateOfBirth = dateOfBirth;
        IsVaccinated = isVaccinated;
        HelpStatus = helpStatus;
        TransferDetailsList = transferDetails.ToList();
        _photos = photos.ToList();
    }
    
    public void UpdateName(Name name)
    {
        Name = name;
    }
    
    public void UpdateDescription(Description description)
    {
        Description = description;
    }
    
    public void UpdatePetClassification(PetClassification petClassification)
    {
        PetClassification = petClassification;
    }
    
    public void UpdateColor(Color color)
    {
        Color = color;
    }
    public void UpdateHealthInfo(HealthInfo healthInfo)
    {
        HealthInfo = healthInfo;
    }
    public void UpdateAddress(Address address)
    {
        Address = address;
    }
    public void UpdateWeight(Weight weight)
    {
        Weight = weight;
    }
    public void UpdateHeight(Height height)
    {
        Height = height;
    }
    public void UpdateOwnerPhoneNumber(Phone ownerPhoneNumber)
    {
        OwnerPhoneNumber = ownerPhoneNumber;
    }
    public void UpdateIsCastrated(IsCastrated isCastrated)
    {
        IsCastrated = isCastrated;
    }
    public void UpdateDateOfBirth(DateOfBirth dateOfBirth)
    {
        DateOfBirth = dateOfBirth;
    }
    public void UpdateIsVaccinated(IsVaccinated isVaccinated)
    {
        IsVaccinated = isVaccinated;
    }
    public void UpdateHelpStatus(HelpStatus helpStatus)
    {
        HelpStatus = helpStatus;
    }
    public void UpdateTransferDetails(IEnumerable<TransferDetail> transferDetails)
    {
        TransferDetailsList = transferDetails.ToList();
    }
    
    
    public void UpdatePhotos(IEnumerable<Photo> photos)
    {
        _photos = photos.ToList();
    }

    public void UpdateMainPhoto(Photo mainPhoto)
    {
        var updatedPhotosList = new List<Photo>();
        foreach (var photo in _photos)
        {
            if (photo.PathToStorage.Path != mainPhoto.PathToStorage.Path)
                updatedPhotosList.Add(photo.CreateCopy(false));
        }
        updatedPhotosList.Add(mainPhoto.CreateCopy(true));
        _photos = updatedPhotosList;
    }

    public void Delete()
    {
        if (_isDeleted == false)
            _isDeleted = true;
    }

    public void Restore()
    {
        if (_isDeleted)
            _isDeleted = false;
    }

    public void SetPosition(Position position) => Position = position;

    public UnitResult<Error> MoveForward()
    {
        var newPosition = Position.Forward();
        if (newPosition.IsFailure)
            return newPosition.Error;

        Position = newPosition.Value;
        
        return Result.Success<Error>();
    }
    
    public UnitResult<Error> MoveBackward()
    {
        var newPosition = Position.Backward();
        if (newPosition.IsFailure)
            return newPosition.Error;

        Position = newPosition.Value;
        return Result.Success<Error>();
    }
    
    public void Move(Position newPosition)
    {
        Position = newPosition;
    }
}