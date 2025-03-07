using PetFamily.Application.Abstractions;
using PetFamily.Application.Dto.Pet;
using PetFamily.Application.Dto.Shared;

namespace PetFamily.Application.Volunteers.Commands.AddPet;

public record AddPetCommand : ICommand
{
    public AddPetCommand(
        Guid volunteerId,
        string name,
        string description,
        PetClassificationDto petClassificationDto,
        string color,
        string healthInfo,
        AddressDto addressDto,
        float weight,
        float height,
        string ownerPhoneNumber,
        bool isCastrated,
        DateTime dateOfBirth,
        bool isVaccinated,
        string helpStatus,
        IEnumerable<TransferDetailDto> transferDetailsDto)
    {
        VolunteerId = volunteerId;
        Name = name;
        Description = description;
        PetClassificationDto = petClassificationDto;
        Color = color;
        HealthInfo = healthInfo;
        AddressDto = addressDto;
        Weight = weight;
        Height = height;
        OwnerPhoneNumber = ownerPhoneNumber;
        IsCastrated = isCastrated;
        DateOfBirth = dateOfBirth;
        IsVaccinated = isVaccinated;
        HelpStatus = helpStatus;
        TransferDetailsDto = transferDetailsDto;
    }

    public Guid VolunteerId { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public PetClassificationDto PetClassificationDto { get; set; }
    public string Color { get; init; }
    public string HealthInfo { get; init; }
    public AddressDto AddressDto { get; init; }
    public float Weight { get; init; }
    public float Height { get; init; }
    public string OwnerPhoneNumber { get; init; }
    public bool IsCastrated { get; init; }
    public DateTime DateOfBirth { get; init; }
    public bool IsVaccinated { get; init; }
    public string HelpStatus { get; init; }

    public IEnumerable<TransferDetailDto> TransferDetailsDto;
}