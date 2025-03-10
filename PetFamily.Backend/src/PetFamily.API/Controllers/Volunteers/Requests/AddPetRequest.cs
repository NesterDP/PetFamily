using PetFamily.Application.Dto.Pet;
using PetFamily.Application.Dto.Shared;
using PetFamily.Application.Volunteers.Commands.AddPet;

namespace PetFamily.API.Controllers.Volunteers.Requests;

public record AddPetRequest(
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
    IEnumerable<TransferDetailDto> TransferDetailsDto)
{
    public AddPetCommand ToCommand(Guid id) => new(
        id,
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
        HelpStatus,
        TransferDetailsDto);
}