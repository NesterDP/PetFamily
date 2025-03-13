using PetFamily.Core.Dto.Pet;
using PetFamily.Core.Dto.Shared;
using PetFamily.Volunteers.Application.Commands.UpdatePetInfo;

namespace PetFamily.Volunteers.Presentation.Volunteers.Requests;

public record UpdatePetInfoRequest(
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
    IEnumerable<TransferDetailDto> TransferDetailsDto)
{
    public UpdatePetInfoCommand ToCommand(Guid volunteerId, Guid petId) => new(
        volunteerId,
        petId,
        Name,
        Description,
        PetClassificationDto,
        Color,
        HealthInfo,
        AddressDto,
        Weight,
        Height,
        OwnerPhoneNumber,
        IsCastrated,
        DateOfBirth,
        IsVaccinated,
        TransferDetailsDto);
}