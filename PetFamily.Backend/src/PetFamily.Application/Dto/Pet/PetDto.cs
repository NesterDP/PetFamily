using PetFamily.Application.Dto.Shared;
using PetFamily.Domain.PetContext.ValueObjects.PetVO;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Dto.Pet;

public class PetDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public Guid SpeciesId { get; init; }
    public Guid BreedId { get; init; }
    public string Color { get; init; }
    public string HealthInfo { get; init; }
    public string City { get; init;}
    public string House { get; init;}
    public string? Apartment { get; init; }
    public float Weight { get; init; }
  public float Height { get; init; }
    public string OwnerPhoneNumber { get; init; }
    public bool IsCastrated { get; init; }
    public DateTime DateOfBirth { get; init; }
    public bool IsVaccinated { get; init; }
    public string HelpStatus { get; init; }
    public TransferDetailDto[] TransferDetails { get; set; } = [];
    //public List<PhotoDto> _photos = [];
    public DateTime CreationDate { get; init; }
    public int Position { get; init; }
}

public class PhotoDto
{
    public string PathToStorage { get; set; } = string.Empty;
}