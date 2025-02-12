using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.SharedVO;
using PetFamily.Domain.SpeciesContext.ValueObjects.BreedVO;

namespace PetFamily.Domain.SpeciesContext.Entities;

public class Breed
{
    public BreedId Id { get; private set; }
    public Name Name { get; private set; }

    private Breed(BreedId id, Name name)
    {
        Id = id;
        Name = name;
    }

    public static Result<Breed> Create(BreedId id, Name name)
    {
        var breed = new Breed(id, name);
        
        return Result.Success(breed);
    }
}