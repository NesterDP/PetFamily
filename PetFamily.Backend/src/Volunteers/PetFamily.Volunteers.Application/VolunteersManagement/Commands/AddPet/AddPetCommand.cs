using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Pet;
using PetFamily.Core.Dto.Shared;

namespace PetFamily.Volunteers.Application.VolunteersManagement.Commands.AddPet;

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
    IEnumerable<TransferDetailDto> TransferDetailsDto) : ICommand;
