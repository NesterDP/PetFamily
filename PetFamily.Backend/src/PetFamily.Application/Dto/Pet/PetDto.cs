using PetFamily.Application.Dto.Shared;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Dto.Pet;

public class PetDto
{
    public Guid Id { get; init; }
    
}

public class PhotoDto
{
    public string PathToStorage { get; set; } = string.Empty;
}