using PetFamily.Application.Abstractions;
using PetFamily.Application.Dto.Pet;
using PetFamily.Application.Dto.Shared;

namespace PetFamily.Application.Volunteers.Commands.UpdatePetInfo;

public record UpdatePetInfoCommand(
    Guid VolunteerId,
    Guid PetId,
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
    IEnumerable<TransferDetailDto> TransferDetailsDto) : ICommand;