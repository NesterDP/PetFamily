using PetFamily.Domain.PetContext.ValueObjects.PetVO;
using PetFamily.Domain.Shared.SharedVO;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.PetContext.Entities;

public sealed class Pet : Shared.Entity<PetId>
{
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
    public TransferDetailsList TransferDetailsList { get; private set; }
    public DateTime CreationDate { get; private set; } = DateTime.Now;
    
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
        TransferDetailsList transferDetailsList
    ) : base(id)
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
        TransferDetailsList = transferDetailsList;
    }
}