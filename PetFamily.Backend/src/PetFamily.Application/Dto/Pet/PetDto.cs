using System.Text.Json.Serialization;
using PetFamily.Application.Dto.Shared;
using PetFamily.Domain.PetContext.ValueObjects.PetVO;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Dto.Pet;

public class PetDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid SpeciesId { get; set; }
    public Guid BreedId { get; set; }
    public string Color { get; set; }
    public string HealthInfo { get; set; }
    public string City { get; set;}
    public string House { get; set;}
    public string? Apartment { get; set; }
    public float Weight { get; set; }
   public float Height { get; set; }
    public string OwnerPhoneNumber { get; set; }
    public bool IsCastrated { get; set; }
    public DateTime DateOfBirth { get; set; }
    public bool IsVaccinated { get; set; }
    public string HelpStatus { get; set; }
    public TransferDetailDto[] TransferDetails { get; set; } = [];
    public PhotoDto[] Photos { get; set; } = [];
    public DateTime CreationDate { get; set; }
    public int Position { get; set; }
    
    [JsonIgnore]
    public bool IsDeleted { get; set; }
}