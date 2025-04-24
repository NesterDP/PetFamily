using System.Text.Json.Serialization;
using PetFamily.Core.Dto.Shared;

namespace PetFamily.Core.Dto.Pet;

public class PetDto
{
    public Guid Id { get; init; }

    public Guid OwnerId { get; init; }

    public string Name { get; init; } = null!;

    public string Description { get; init; } = null!;

    public Guid SpeciesId { get; init; }

    public Guid BreedId { get; init; }

    public string Color { get; init; } = null!;

    public string HealthInfo { get; init; } = null!;

    public string City { get; init; } = null!;

    public string House { get; init; } = null!;

    public string? Apartment { get; init; }

    public float Weight { get; init; }

    public float Height { get; init; }

    public string OwnerPhoneNumber { get; init; } = null!;

    public bool IsCastrated { get; init; }

    public DateTime DateOfBirth { get; init; }

    public bool IsVaccinated { get; init; }

    public string HelpStatus { get; init; } = null!;

    public TransferDetailDto[] TransferDetails { get; init; } = [];

    public PhotoDto[] Photos { get; set; } = [];

    public DateTime CreationDate { get; init; }

    public int Position { get; init; }

    [JsonIgnore]
    public bool IsDeleted { get; init; }
}