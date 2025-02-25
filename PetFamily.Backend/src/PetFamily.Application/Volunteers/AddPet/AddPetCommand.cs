using PetFamily.Application.Dto.Pet;
using PetFamily.Application.Dto.Shared;
using PetFamily.Domain.PetContext.ValueObjects.PetVO;

namespace PetFamily.Application.Volunteers.AddPet;

public record AddPetCommand(
    Guid VolunteerId,
    string Name,
    string Description,
    PetClassificationDto PetClassificationDto,
    string Color,
    string HealthInfo,
    AddressDto AddressDto,
    float Weight,
    float Height,
    string OwnerPhoneNumber,
    bool IsCastrated,
    DateTime DateOfBirth,
    bool IsVaccinated,
    string HelpStatus,
    IEnumerable<TransferDetailDto> TransferDetailsDto);