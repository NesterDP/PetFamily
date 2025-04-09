using PetFamily.Core.Dto.Pet;

namespace PetFamily.Accounts.Application.Dto;

public class ParticipantAccountDto
{
    public List<FavoritePetDto> FavoritePets { get; set; } = [];
}