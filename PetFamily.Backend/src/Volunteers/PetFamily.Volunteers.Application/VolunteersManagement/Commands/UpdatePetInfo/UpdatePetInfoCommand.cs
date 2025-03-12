using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Pet;
using PetFamily.Core.Dto.Shared;

namespace PetFamily.Volunteers.Application.VolunteersManagement.Commands.UpdatePetInfo;

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