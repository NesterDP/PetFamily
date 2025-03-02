﻿using CSharpFunctionalExtensions;
using PetFamily.Domain.PetContext.ValueObjects.PetVO;
using PetFamily.Domain.Shared.SharedVO;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Domain.PetContext.Entities;

public sealed class Pet : Entity<PetId>
{
    private bool _isDeleted = false;
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

    public void UpdatePhotos(IEnumerable<Photo> photos)
    {
        _photos = photos.ToList();
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