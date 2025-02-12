using CSharpFunctionalExtensions;
using PetFamily.Domain.PetContext.Entities;
using PetFamily.Domain.Shared.SharedVO;
using PetFamily.Domain.SpeciesContext.ValueObjects.SpeciesVO;

namespace PetFamily.Domain.SpeciesContext.Entities;

public class Species
{
    public SpeciesId Id { get; private set; }
    public Name Name { get; private set; }

    private readonly List<Breed> _breeds;

    public IReadOnlyCollection<Breed> Breeds => _breeds;

    private Species(SpeciesId id, Name name, List<Breed> breeds)
    {
        Id = id;
        Name = name;
        _breeds = breeds;
    }

    public static Result<Species> Create(SpeciesId id, Name name, List<Breed> breeds)
    {
        var species = new Species(id, name, breeds);
        
        return Result.Success(species);
    }
}